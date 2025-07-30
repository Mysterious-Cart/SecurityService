using Security.Data.Model;

/// <summary>
/// Authentication response payload
/// </summary>
public record AuthPayload(bool Success,User? User, string Message);
public record RolePayload(bool Success, Role? Role, string Message);