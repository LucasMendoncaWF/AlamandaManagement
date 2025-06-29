using Microsoft.EntityFrameworkCore;
using System.Data;
using AlamandaApi.Data;
using MySqlConnector;

namespace AlamandaApi.Services.FieldsSchema {
  public enum FieldDataType {
    String = 0,
    Number = 1,
    Date = 2,
    Image = 3,
    ImageArray = 4,
    OptionsArray = 5,
    Options = 6
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

    public async Task<List<FieldInfo>> GetFieldTypes(string tableName) {
      var fields = new List<FieldInfo>();
      var foreignKeys = new Dictionary<string, string>();
      var foundJunctions = new Dictionary<string, string>();

      await using var conn = _context.Database.GetDbConnection();
      if (conn.State != ConnectionState.Open)
        await conn.OpenAsync();

      await using (var relCmd = conn.CreateCommand()) {
        relCmd.CommandText = @"
          SELECT rel.TABLE_NAME, rel.COLUMN_NAME, rel.REFERENCED_TABLE_NAME
          FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE rel
          WHERE rel.REFERENCED_TABLE_NAME = @currentTable AND rel.TABLE_SCHEMA = DATABASE();";

        var relParam = relCmd.CreateParameter();
        relParam.ParameterName = "@currentTable";
        relParam.Value = tableName;
        relCmd.Parameters.Add(relParam);

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
            c.COLUMN_NAME, 
            c.DATA_TYPE, 
            c.CHARACTER_MAXIMUM_LENGTH, 
            c.COLUMN_TYPE, 
            c.IS_NULLABLE,
            k.REFERENCED_TABLE_NAME
          FROM INFORMATION_SCHEMA.COLUMNS c
          LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
            ON c.TABLE_NAME = k.TABLE_NAME 
            AND c.COLUMN_NAME = k.COLUMN_NAME 
            AND k.TABLE_SCHEMA = c.TABLE_SCHEMA
          WHERE c.TABLE_NAME = @table AND c.TABLE_SCHEMA = DATABASE();";

        var param = cmd.CreateParameter();
        param.ParameterName = "@table";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
          var name = reader.GetString(0);
          var sqlType = reader.GetString(1).ToLower();
          var maxLength = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString();
          var columnType = reader.GetString(3).ToLower();
          var isNullable = reader.GetString(4).Equals("YES", StringComparison.OrdinalIgnoreCase);
          var fkTable = reader.IsDBNull(5) ? null : reader.GetString(5);
          var isForeignKey = fkTable != null;
          var isJunction = fkTable != null && junctionTables.Contains(fkTable.ToLower());

          var inferredType = InferType(name.ToLower(), sqlType, columnType, isForeignKey, isJunction);

          if (name.ToLower() != "id") {
            fields.Add(new FieldInfo {
              FieldName = name.ToLower(),
              DataType = inferredType,
              FieldMaxSize = maxLength,
              OptionsArray = null,
              IsRequired = !isNullable
            });
          }

          if ((inferredType == FieldDataType.Options || inferredType == FieldDataType.OptionsArray) && fkTable != null)
            foreignKeys[name.ToLower()] = fkTable;
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
          SELECT COLUMN_NAME, REFERENCED_TABLE_NAME
          FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
          WHERE TABLE_NAME = @junction AND TABLE_SCHEMA = DATABASE() AND REFERENCED_TABLE_NAME != @currentTable;";

        var j1 = otherFkCmd.CreateParameter();
        j1.ParameterName = "@junction";
        j1.Value = junction;
        otherFkCmd.Parameters.Add(j1);

        var j2 = otherFkCmd.CreateParameter();
        j2.ParameterName = "@currentTable";
        j2.Value = tableName;
        otherFkCmd.Parameters.Add(j2);

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
      if (sqlType.Contains("int") || sqlType is "decimal" or "float" or "double") return FieldDataType.Number;
      if (sqlType.Contains("date") || sqlType.Contains("time")) return FieldDataType.Date;
      return FieldDataType.String;
    }

    private async Task<List<Option>> GetOptions(string table) {
      var result = new List<Option>();
      var connStr = _context.Database.GetConnectionString();

      await using var conn = new MySqlConnection(connStr);
      await conn.OpenAsync();

      await using var cmd = conn.CreateCommand();
      cmd.CommandText = $"SELECT * FROM `{table}` LIMIT 100;";
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
