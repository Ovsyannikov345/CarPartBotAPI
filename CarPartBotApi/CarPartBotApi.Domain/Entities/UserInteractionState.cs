namespace CarPartBotApi.Domain.Entities;

public class UserInteractionState : EntityBase
{
    public required string ActionState { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
