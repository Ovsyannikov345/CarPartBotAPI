using CarPartBotApi.Domain.Interfaces.Entities;

namespace CarPartBotApi.Domain.Entities;

public sealed class Car : EntityBase, ISoftDeletable
{
    public required string Brand { get; set; }

    public required string Model { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid UserId { get; set; }

    public required User User { get; set; }
}
