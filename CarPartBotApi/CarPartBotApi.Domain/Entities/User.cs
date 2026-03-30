using CarPartBotApi.Domain.Interfaces.Entities;

namespace CarPartBotApi.Domain.Entities;

public sealed class User : EntityBase, ISoftDeletable
{
    public long TelegramId { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime? DeletedAt { get; set; }

    public UserInteractionState UserInteractionState { get; set; } = null!;

    public List<Car> Cars { get; set; } = [];
}
