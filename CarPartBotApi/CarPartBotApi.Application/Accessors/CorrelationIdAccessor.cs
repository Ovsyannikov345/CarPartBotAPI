namespace CarPartBotApi.Application.Accessors;

public interface ICorrelationIdAccessor
{
    string CorrelationId { get; set; }
}

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private string? _correlationId;

    public string CorrelationId 
    { 
        get => _correlationId ?? throw new InvalidOperationException("Correlation ID has not been set"); 
        set => _correlationId = value; 
    }
}
