using Security.Data.Model;

/// <summary>
/// Authentication response payload
/// </summary>
public record AuthPayload(User? User, string Message);