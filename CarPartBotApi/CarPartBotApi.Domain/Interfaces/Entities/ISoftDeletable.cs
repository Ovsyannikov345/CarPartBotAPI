namespace CarPartBotApi.Domain.Interfaces.Entities;

public interface ISoftDeletable
{
    public DateTime? DeletedAt { get; set; }
}
