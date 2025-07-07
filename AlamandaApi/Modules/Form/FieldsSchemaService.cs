using Microsoft.EntityFrameworkCore;
using System.Data;
using AlamandaApi.Data;
using Npgsql;

namespace AlamandaApi.Services.FieldsSchema {
  public enum FieldDataType {
    Translations = 0,
    String = 1,
    Number = 2,
    Date = 3,
    Boolean = 4,
    Options = 5,
    OptionsArray = 6,
    TextArea = 7,
    Image = 8,
    ImageArray = 9,
  }

  public class FieldInfo {
    public string FieldName { get; set; } = null!;
    public FieldDataType DataType { get; set; } = FieldDataType.String;
    public string? FieldMaxSize { get; set; }
    public List<Option>? OptionsArray { get; set; }
    public bool IsRequired { get; set; }
    public List<TranslationLanguageGroup>? TranslationsGroups { get; set; }
  }
  public class TranslationFieldItem {
    public string FieldName { get; set; } = null!;
    public string DefaultValue { get; set; } = null!;
    public string OriginalColumnName { get; set; } = null!;
    public FieldDataType DataType { get; set; }
    public string? FieldMaxSize { get; set; }
    public bool IsRequired { get; set; }
    public List<Option>? OptionsArray { get; set; }
  }

  public class TranslationLanguageGroup {
    public int LanguageId { get; set; }
    public string LanguageName { get; set; } = null!;
    public int? Id { get; set; }
    public List<TranslationFieldItem> Fields { get; set; } = new();
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

      await LoadJunctionTables(conn, tableName, foundJunctions);

      var junctionTables = new HashSet<string>(foundJunctions.Keys.Select(k => k.ToLower()));

      await LoadColumns(conn, tableName, excludedFields, fields, foreignKeys, junctionTables);

      await LoadOptionsForForeignKeys(fields, foreignKeys);

      await LoadJunctionForeignKeyOptions(conn, foundJunctions, tableName, fields);

      await LoadTranslationFields(tableName, conn, fields);

      return fields;
    }

    private async Task LoadJunctionTables(NpgsqlConnection conn, string tableName, Dictionary<string, string> foundJunctions) {
      await using var relCmd = conn.CreateCommand();
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

    private async Task LoadColumns(NpgsqlConnection conn, string tableName, List<string>? excludedFields, List<FieldInfo> fields, Dictionary<string, string> foreignKeys, HashSet<string> junctionTables) {
      await using var cmd = conn.CreateCommand();
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

        if (name != "Id" && (excludedFields == null || !excludedFields.Contains(name))) {
          fields.Add(new FieldInfo {
            FieldName = ToFirstLowerCase(name),
            DataType = inferredType,
            FieldMaxSize = maxLength,
            OptionsArray = null,
            IsRequired = !isNullable
          });
        }

        if ((inferredType == FieldDataType.Options || inferredType == FieldDataType.OptionsArray) && fkTable != null) {
          string baseName = ToFirstLowerCase(name);
          if (name.EndsWith("id")) {
            baseName = name[..^2];
          }
          foreignKeys[baseName] = fkTable;
        }
      }
      reader.Close();
    }

    private async Task LoadOptionsForForeignKeys(List<FieldInfo> fields, Dictionary<string, string> foreignKeys) {
      foreach (var field in fields) {
        if ((field.DataType == FieldDataType.Options || field.DataType == FieldDataType.OptionsArray)
            && foreignKeys.TryGetValue(field.FieldName, out var fkTable)) {
          field.OptionsArray = await GetOptions(fkTable);
        }
      }
    }

    private async Task LoadJunctionForeignKeyOptions(NpgsqlConnection conn, Dictionary<string, string> foundJunctions, string currentTable, List<FieldInfo> fields) {
      foreach (var junction in foundJunctions.Keys) {
        await using var otherFkCmd = conn.CreateCommand();
        otherFkCmd.CommandText = @"
          SELECT kcu.column_name, ccu.table_name AS referenced_table
          FROM information_schema.table_constraints tc
          JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name
          JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name
          WHERE tc.table_name = @junction AND tc.constraint_type = 'FOREIGN KEY' AND ccu.table_name != @currentTable;";
        otherFkCmd.Parameters.AddWithValue("@junction", junction);
        otherFkCmd.Parameters.AddWithValue("@currentTable", currentTable);

        await using var reader2 = await otherFkCmd.ExecuteReaderAsync();
        while (await reader2.ReadAsync()) {
          var otherTable = reader2.GetString(1);
          if (otherTable.ToLower() == "languages") continue;

          var fieldName = ToFirstLowerCase(otherTable);
          if (!fields.Any(f => f.FieldName == fieldName)) {
            fields.Add(new FieldInfo {
              FieldName = fieldName,
              DataType = FieldDataType.OptionsArray,
              OptionsArray = await GetOptions(otherTable),
              IsRequired = false
            });
          }
        }
      }
    }

    private async Task LoadTranslationFields(string tableName, NpgsqlConnection conn, List<FieldInfo> fields, int? entityId = null) {
      var translationsTable = tableName + "Translations";
      if (!await TableExists(translationsTable, conn)) return;

      var languages = await GetOptions("Languages");

      var fieldCmd = conn.CreateCommand();
      fieldCmd.CommandText = @"
        SELECT column_name, data_type, character_maximum_length, is_nullable
        FROM information_schema.columns
        WHERE table_name = @translationsTable AND table_schema = 'public'";
      fieldCmd.Parameters.AddWithValue("@translationsTable", translationsTable);

      var baseFields = new List<TranslationFieldItem>();
      await using (var reader = await fieldCmd.ExecuteReaderAsync()) {
        while (await reader.ReadAsync()) {
          var colName = reader.GetString(0);
          if (colName.Equals("languageid", StringComparison.OrdinalIgnoreCase) ||
              colName.Equals("id", StringComparison.OrdinalIgnoreCase) ||
              colName.Equals(tableName.ToLower() + "id", StringComparison.OrdinalIgnoreCase)) {
            continue;
          }

          var dataType = reader.GetString(1).ToLower();
          var maxLength = reader.IsDBNull(2) ? null : reader.GetValue(2).ToString();
          var isNullable = reader.GetString(3).Equals("YES", StringComparison.OrdinalIgnoreCase);

          baseFields.Add(new TranslationFieldItem {
            FieldName = ToFirstLowerCase(colName),
            OriginalColumnName = colName,
            DataType = InferType(colName, dataType, "", false, false),
            FieldMaxSize = maxLength,
            IsRequired = !isNullable,
            OptionsArray = null
          });
        }
        await reader.CloseAsync();
      }

      var translationData = new Dictionary<int, (int? Id, Dictionary<string, string>)>();
      if (entityId.HasValue) {
        var valuesCmd = conn.CreateCommand();
        valuesCmd.CommandText = $@"
          SELECT * FROM ""{translationsTable}""
          WHERE ""{tableName}Id"" = @id";
        valuesCmd.Parameters.AddWithValue("@id", entityId.Value);

        await using var reader = await valuesCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
          var langId = Convert.ToInt32(reader["languageId"]);
          int? recordId = reader["Id"] is DBNull ? null : Convert.ToInt32(reader["Id"]);
          var dict = new Dictionary<string, string>();

          foreach (var field in baseFields) {
            var columnName = field.OriginalColumnName ?? field.FieldName;
            if (!reader.IsDBNull(reader.GetOrdinal(columnName))) {
              dict[field.FieldName] = reader[columnName]?.ToString() ?? "";
            }
          }

          translationData[langId] = (recordId, dict);
        }
      }

      var groups = new List<TranslationLanguageGroup>();
      foreach (var lang in languages) {
        var langId = int.Parse(lang.Id ?? "0");

        var fieldsWithValue = baseFields
          .Where(field => !field.FieldName.Contains("Id"))
          .Select(f => new TranslationFieldItem {
            FieldName = f.FieldName,
            DataType = f.DataType,
            FieldMaxSize = f.FieldMaxSize,
            IsRequired = f.IsRequired,
            OptionsArray = f.OptionsArray
          })
          .ToList();

        if (translationData.TryGetValue(langId, out var val)) {
          foreach (var f in fieldsWithValue) {
            if (val.Item2.TryGetValue(f.FieldName, out var fieldVal)) {
              f.DefaultValue = fieldVal;
            }
          }

          groups.Add(new TranslationLanguageGroup {
            LanguageId = langId,
            LanguageName = lang.Name ?? "",
            Id = val.Id,
            Fields = fieldsWithValue
          });
        }
        else {
          groups.Add(new TranslationLanguageGroup {
            LanguageId = langId,
            LanguageName = lang.Name ?? "",
            Id = null,
            Fields = fieldsWithValue
          });
        }
      }

      fields.Add(new FieldInfo {
        FieldName = "translations",
        DataType = FieldDataType.Translations,
        IsRequired = true,
        TranslationsGroups = groups
      });
    }
    private async Task<bool> TableExists(string tableName, NpgsqlConnection conn) {
      await using var cmd = conn.CreateCommand();
      cmd.CommandText = "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = @name);";
      cmd.Parameters.AddWithValue("@name", tableName);
      var result = await cmd.ExecuteScalarAsync();
      return result is bool b && b;
    }

    private string ToFirstLowerCase(string value) {
      if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
        return value;
      return char.ToLower(value[0]) + value.Substring(1);
    }

    private static FieldDataType InferType(string name, string sqlType, string columnType, bool isForeignKey, bool isJunction) {
      if (isForeignKey) return isJunction ? FieldDataType.OptionsArray : FieldDataType.Options;
      if (name.Contains("pictures") && columnType.StartsWith("json")) return FieldDataType.ImageArray;
      if (name.Contains("picture")) return FieldDataType.Image;
      if (sqlType == "text") return FieldDataType.TextArea;
      if (sqlType == "boolean" || sqlType == "bool") return FieldDataType.Boolean;
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
      try {
        var translationTable = table + "Translations";
        cmd.CommandText = @"
        SELECT EXISTS (
            SELECT 1 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
              AND table_name = @tableName
        );";
        cmd.Parameters.AddWithValue("tableName", translationTable);
        var translationTableResult = await cmd.ExecuteScalarAsync();
        if (translationTableResult is bool exists && exists) {
          cmd.CommandText = $"SELECT * FROM \"{translationTable}\" WHERE \"LanguageId\" = 1; LIMIT 100";
          await using var reader = await cmd.ExecuteReaderAsync();
          while (await reader.ReadAsync()) {
            var idField =
              Try(reader, ToSingular(table) + "Id") ??
              Try(reader, "Id");

            var name = Try(reader, "name") ?? Try(reader, "title") ?? Try(reader, "language");

            if (idField != null && name != null)
              result.Add(new Option { Id = idField, Name = name });
          }
        }
        else {
          throw new Exception("table not found");
        }
      }
      catch {
        cmd.CommandText = $"SELECT * FROM \"{table}\" LIMIT 100;";
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) {
          var id = Try(reader, "id");
          var name = Try(reader, "name") ?? Try(reader, "title") ?? Try(reader, "language");
          if (id != null && name != null)
            result.Add(new Option { Id = id, Name = name });
        }
      }

      return result;
    }

    private static string? Try(IDataRecord r, string col) {
      try {
        return r[col]?.ToString();
      }
      catch {
        return null;
      }
    }
    
    private string ToSingular(string tableName) {
      if (string.IsNullOrEmpty(tableName) || tableName == "Status")
        return tableName;

      if (tableName.EndsWith("ies", StringComparison.OrdinalIgnoreCase)) {
        return tableName.Substring(0, tableName.Length - 3) + "y";
      }

      if (tableName.EndsWith("s", StringComparison.OrdinalIgnoreCase) && !tableName.EndsWith("ss", StringComparison.OrdinalIgnoreCase)) {
        return tableName.Substring(0, tableName.Length - 1);
      }

      return tableName;
    }
  }
}
