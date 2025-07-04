using Microsoft.EntityFrameworkCore;
using System.Data;
using AlamandaApi.Data;
using Npgsql;

namespace AlamandaApi.Services.FieldsSchema {
  public enum FieldDataType {
    String = 0,
    Number = 1,
    Date = 2,
    Options = 3,
    Image = 4,
    ImageArray = 5,
    OptionsArray = 6,
  }

  public class FieldInfo {
    public string FieldName { get; set; } = null!;
    public FieldDataType DataType { get; set; } = FieldDataType.String;
    public string? FieldMaxSize { get; set; }
    public List<Option>? OptionsArray { get; set; }
    public bool IsRequired { get; set; }
  }

  public class Option {
    public string? Id { get; set; }
    public string? Name { get; set; }
  }

  public class FieldsSchemaService {
    private readonly AppDbContext _context;

    public FieldsSchemaService(AppDbContext context) {
      _context = context;
    }

    public async Task<List<FieldInfo>> GetFieldTypes(string tableName, List<string>? excludedFields = null) {
      var fields = new List<FieldInfo>();
      var foreignKeys = new Dictionary<string, string>();
      var foundJunctions = new Dictionary<string, string>();

      await using var conn = new NpgsqlConnection(_context.Database.GetConnectionString());
      await conn.OpenAsync();

      await using (var relCmd = conn.CreateCommand()) {
        relCmd.CommandText = @"
          SELECT tc.table_name, kcu.column_name, ccu.table_name AS foreign_table_name
          FROM information_schema.table_constraints AS tc
          JOIN information_schema.key_column_usage AS kcu
            ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema
          JOIN information_schema.constraint_column_usage AS ccu
            ON ccu.constraint_name = tc.constraint_name AND ccu.table_schema = tc.table_schema
          WHERE tc.constraint_type = 'FOREIGN KEY' AND ccu.table_name = @currentTable;";

        relCmd.Parameters.AddWithValue("@currentTable", tableName);

        await using var relReader = await relCmd.ExecuteReaderAsync();
        while (await relReader.ReadAsync()) {
          var junction = relReader.GetString(0);
          var fk = relReader.GetString(1);
          foundJunctions.TryAdd(junction, fk);
        }
        relReader.Close();
      }

      var junctionTables = new HashSet<string>(foundJunctions.Keys.Select(k => k.ToLower()));

      await using (var cmd = conn.CreateCommand()) {
        cmd.CommandText = @"
          SELECT
            cols.column_name,
            cols.data_type,
            cols.character_maximum_length,
            cols.udt_name,
            cols.is_nullable,
            ccu.table_name AS referenced_table,
            tc.constraint_type
          FROM information_schema.columns cols
          LEFT JOIN information_schema.key_column_usage kcu
            ON cols.table_name = kcu.table_name AND cols.column_name = kcu.column_name AND cols.table_schema = kcu.table_schema
          LEFT JOIN information_schema.constraint_column_usage ccu
            ON kcu.constraint_name = ccu.constraint_name AND kcu.table_schema = ccu.table_schema
          LEFT JOIN information_schema.table_constraints tc
            ON kcu.constraint_name = tc.constraint_name AND kcu.table_schema = tc.table_schema
          WHERE cols.table_name = @table AND cols.table_schema = 'public';";

        cmd.Parameters.AddWithValue("@table", tableName);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
          var name = reader.GetString(0);
          var sqlType = reader.GetString(1).ToLower();
          var maxLength = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString();
          var columnType = reader.GetString(3).ToLower();
          var isNullable = reader.GetString(4).Equals("YES", StringComparison.OrdinalIgnoreCase);
          var fkTable = reader.IsDBNull(5) ? null : reader.GetString(5);
          var constraintType = reader.IsDBNull(6) ? null : reader.GetString(6);

          var isForeignKey = constraintType == "FOREIGN KEY" && fkTable != null;
          var isJunction = isForeignKey && fkTable != null && junctionTables.Contains(fkTable.ToLower());

          var inferredType = InferType(name.ToLower(), sqlType, columnType, isForeignKey, isJunction);

          var lowerName = name.ToLower();
          if (lowerName != "id" && (excludedFields == null || !excludedFields.Contains(lowerName))) {
            fields.Add(new FieldInfo {
              FieldName = lowerName,
              DataType = inferredType,
              FieldMaxSize = maxLength,
              OptionsArray = null,
              IsRequired = !isNullable
            });
          }

          if ((inferredType == FieldDataType.Options || inferredType == FieldDataType.OptionsArray) && fkTable != null)
            foreignKeys[lowerName] = fkTable;
        }
        reader.Close();
      }

      foreach (var field in fields) {
        if ((field.DataType == FieldDataType.Options || field.DataType == FieldDataType.OptionsArray)
            && foreignKeys.TryGetValue(field.FieldName, out var fkTable)) {
          field.OptionsArray = await GetOptions(fkTable);
        }
      }

      foreach (var junction in foundJunctions.Keys) {
        await using var otherFkCmd = conn.CreateCommand();
        otherFkCmd.CommandText = @"
          SELECT kcu.column_name, ccu.table_name AS referenced_table
          FROM information_schema.table_constraints tc
          JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name
          JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name
          WHERE tc.table_name = @junction AND tc.constraint_type = 'FOREIGN KEY' AND ccu.table_name != @currentTable;";

        otherFkCmd.Parameters.AddWithValue("@junction", junction);
        otherFkCmd.Parameters.AddWithValue("@currentTable", tableName);

        await using var reader2 = await otherFkCmd.ExecuteReaderAsync();
        while (await reader2.ReadAsync()) {
          var otherTable = reader2.GetString(1);
          fields.Add(new FieldInfo {
            FieldName = otherTable.ToLower(),
            DataType = FieldDataType.OptionsArray,
            OptionsArray = await GetOptions(otherTable),
            IsRequired = false
          });
        }
      }

      return fields;
    }

    private static FieldDataType InferType(string name, string sqlType, string columnType, bool isForeignKey, bool isJunction) {
      if (isForeignKey) return isJunction ? FieldDataType.OptionsArray : FieldDataType.Options;
      if (name.Contains("pictures") && columnType.StartsWith("json")) return FieldDataType.ImageArray;
      if (name.Contains("picture")) return FieldDataType.Image;
      if (sqlType.Contains("int") || sqlType is "numeric" or "decimal" or "float" or "double precision") return FieldDataType.Number;
      if (sqlType.Contains("date") || sqlType.Contains("time")) return FieldDataType.Date;
      return FieldDataType.String;
    }

    private async Task<List<Option>> GetOptions(string table) {
      var result = new List<Option>();
      var connStr = _context.Database.GetConnectionString();

      await using var conn = new NpgsqlConnection(connStr);
      await conn.OpenAsync();

      await using var cmd = conn.CreateCommand();
      cmd.CommandText = $"SELECT * FROM \"{table}\" LIMIT 100;";
      await using var reader = await cmd.ExecuteReaderAsync();
      while (await reader.ReadAsync()) {
        var id = Try(reader, "id");
        var name = Try(reader, "name") ?? Try(reader, "title");
        if (id != null && name != null)
          result.Add(new Option { Id = id, Name = name });
      }

      return result;
    }

    private static string? Try(IDataRecord r, string col) {
      try {
        return r[col]?.ToString();
      } catch {
        return null;
      }
    }
  }
}
