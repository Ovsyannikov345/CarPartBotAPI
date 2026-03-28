using CarPartBotApi.Domain.Constants.Enums;
using System.Text.Json;

namespace CarPartBotApi.Domain.Entities;

public class UserInteractionState : EntityBase
{
    public ActionType ActionType { get; set; }

    public ActionStep ActionStep { get; set; }

    public required JsonDocument ActionState { get; set; }

    public Guid UserId { get; set; }

    public required User User { get; set; }
}
