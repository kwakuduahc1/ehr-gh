namespace ShimsServer.Models.AuthVm;
public record UsersDto(Guid ID, string UserName, string FullName, string Role);

public record UsersDtoVw(Guid ID, string UserName, string FullName, string[] Roles);
