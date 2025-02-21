namespace Domain;

public class User
{
    public required Guid Id { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
}