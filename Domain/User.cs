﻿using NpgsqlTypes;

namespace Domain;

/// <summary>
/// Represents a user/member in the system.
/// </summary>
public class User : TrackedEntity
{
    public long Id { get; set; }
    public required string DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public NpgsqlTsVector DisplayNameSearchVector { get; set; } = null!;
}
