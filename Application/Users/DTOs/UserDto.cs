namespace Application.Users.DTOs;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
}