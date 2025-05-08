namespace Domain;

public class TrackedEntity
{
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public DateTimeOffset DeletedOn { get; set; }
}
