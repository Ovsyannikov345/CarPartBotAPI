using CarPartBotApi.Domain.Interfaces.Entities;

namespace CarPartBotApi.Domain.Entities;

public sealed class User : EntityBase, ISoftDeletable
{
    public required string Name { get; set; }

    public required string UserName { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime? DeletedAt { get; set; }

    public required UserInteractionState UserInteractionState { get; set; }

    public List<Car> Cars { get; set; } = [];
}
