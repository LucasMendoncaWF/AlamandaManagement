using Npgsql;

public static class ErrorHandler {
  public static void GetFriendlyError(PostgresException? pgEx) {
    var errorMsg = "Unexpected error";
    if (pgEx != null && pgEx.SqlState == "23505") {
      errorMsg = GetFriendlyUniqueErrorMessage(pgEx.MessageText);
    }
    throw new Exception(errorMsg);
  }

  public static string GetFriendlyUniqueErrorMessage(string pgErrorMsg) {
    var constraintMatch = System.Text.RegularExpressions.Regex.Match(pgErrorMsg, @"unique constraint ""(?<constraint>[^""]+)""");
    if (constraintMatch.Success) {
      var constraintName = constraintMatch.Groups["constraint"].Value;

      if (constraintName.Equals("MemberSocial", StringComparison.OrdinalIgnoreCase))
        return "A member with this Social already exists.";

      if (constraintName.Equals("AnotherConstraintName", StringComparison.OrdinalIgnoreCase))
        return "Custom message for another constraint.";

      return $"A record with this value already exists (constraint: {constraintName}).";
    }
    return "A unique constraint violation occurred.";
  }
}