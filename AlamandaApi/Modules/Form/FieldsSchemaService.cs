using Microsoft.EntityFrameworkCore;
using System.Data;
using AlamandaApi.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace AlamandaApi.Services.FieldsSchema {
  public enum FieldDataType {
    String = 0,
    Number = 1,
    Date = 2,
    Image = 3,
    ImageArray = 4,
    OptionsArray = 5
  }

  public class FieldInfo {
    public string FieldName { get; set; } = null!;
    public FieldDataType DataType { get; set; } = FieldDataType.String;
    public string? FieldMaxSize { get; set; }
    public List<Option>? OptionsArray { get; set; }
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

      await using var mainConn = _context.Database.GetDbConnection();
      if (mainConn.State != ConnectionState.Open)
        await mainConn.OpenAsync();

      // Buscar colunas da tabela
      await using (var cmd = mainConn.CreateCommand()) {
        cmd.CommandText = @"
          SELECT 
            c.COLUMN_NAME,
            c.DATA_TYPE,
            c.CHARACTER_MAXIMUM_LENGTH,
            c.COLUMN_TYPE,
            k.REFERENCED_TABLE_NAME
          FROM INFORMATION_SCHEMA.COLUMNS c
          LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE k
            ON c.TABLE_NAME = k.TABLE_NAME 
            AND c.COLUMN_NAME = k.COLUMN_NAME 
            AND k.TABLE_SCHEMA = c.TABLE_SCHEMA
          WHERE c.TABLE_NAME = @table 
            AND c.TABLE_SCHEMA = DATABASE();
        ";

        var tableParam = cmd.CreateParameter();
        tableParam.ParameterName = "@table";
        tableParam.Value = tableName;
        cmd.Parameters.Add(tableParam);

        await using var reader = await cmd.ExecuteReaderAsync();

        var foreignKeys = new Dictionary<string, string>();

        while (await reader.ReadAsync()) {
          var name = reader.GetString(0);
          var sqlType = reader.GetString(1);
          var maxLength = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString();
          var columnType = reader.GetString(3);
          var fkTable = reader.IsDBNull(4) ? null : reader.GetString(4);

          var inferredType = InferType(name, sqlType, columnType, fkTable);

          if (name != "Id") {
            fields.Add(new FieldInfo {
              FieldName = name.ToLower(),
              DataType = inferredType,
              FieldMaxSize = maxLength,
              OptionsArray = null
            });
          }
          if (inferredType == FieldDataType.OptionsArray && fkTable != null)
            foreignKeys[name] = fkTable;
        }
        reader.Close();

        // Preencher opções de FK diretas
        foreach (var field in fields) {
          if (field.DataType == FieldDataType.OptionsArray && foreignKeys.TryGetValue(field.FieldName, out var fkTable)) {
            field.OptionsArray = await GetOptions(fkTable);
          }
        }
      }

      await using (var relCmd = mainConn.CreateCommand()) {
        relCmd.CommandText = @"
          SELECT 
            rel.TABLE_NAME,
            rel.COLUMN_NAME,
            rel.REFERENCED_TABLE_NAME
          FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE rel
          WHERE rel.REFERENCED_TABLE_NAME = @currentTable
            AND rel.TABLE_SCHEMA = DATABASE();
        ";
        var relParam = relCmd.CreateParameter();
        relParam.ParameterName = "@currentTable";
        relParam.Value = tableName;
        relCmd.Parameters.Add(relParam);

        await using var relReader = await relCmd.ExecuteReaderAsync();
        var foundJunctions = new Dictionary<string, string>();

        while (await relReader.ReadAsync()) {
          var junctionTable = relReader.GetString(0);
          var currentTableFk = relReader.GetString(1);
          foundJunctions.TryAdd(junctionTable, currentTableFk);
        }
        relReader.Close();

        // Para cada junction table, buscar a outra FK que NÃO referencia a tabela atual
        foreach (var junction in foundJunctions.Keys) {
          await using var otherFkCmd = mainConn.CreateCommand();
          otherFkCmd.CommandText = @"
            SELECT COLUMN_NAME, REFERENCED_TABLE_NAME
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
            WHERE TABLE_NAME = @junction
              AND TABLE_SCHEMA = DATABASE()
              AND REFERENCED_TABLE_NAME != @currentTable;
          ";

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
              OptionsArray = await GetOptions(otherTable)
            });
          }
        }
      }

      return fields;
    }

    private static FieldDataType InferType(string name, string sqlType, string columnType, string? fkTable) {
      name = name.ToLower();
      sqlType = sqlType.ToLower();

      if (!string.IsNullOrEmpty(fkTable))
        return FieldDataType.OptionsArray;

      if (name.Contains("pictures") && columnType.StartsWith("json"))
        return FieldDataType.ImageArray;

      if (name.Contains("picture"))
        return FieldDataType.Image;

      if (sqlType.Contains("int") || sqlType is "decimal" or "float" or "double")
        return FieldDataType.Number;

      if (sqlType.Contains("date") || sqlType.Contains("time"))
        return FieldDataType.Date;

      return FieldDataType.String;
    }

    private async Task<List<Option>> GetOptions(string tableName) {
      var options = new List<Option>();

      var connectionString = _context.Database.GetConnectionString();

      await using var conn = new MySqlConnection(connectionString);
      await conn.OpenAsync();

      await using var cmd = conn.CreateCommand();
      cmd.CommandText = $"SELECT * FROM `{tableName}` LIMIT 100;";

      await using var reader = await cmd.ExecuteReaderAsync();
      while (await reader.ReadAsync()) {
        var id = Try(reader, "id");
        var name = Try(reader, "name") ?? Try(reader, "title");

        if (id != null && name != null)
          options.Add(new Option { Id = id, Name = name });
      }

      return options;
    }

    private static string? Try(IDataRecord reader, string column) {
      try {
        return reader[column]?.ToString();
      }
      catch {
        return null;
      }
    }
  }
}
