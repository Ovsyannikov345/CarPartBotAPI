using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;

namespace CarPartBotApi.Application.Accessors;

public interface ITelegramContextAccessor
{
    public ChatContext ChatContext { get; set; }

    public UserContext UserContext { get; set; }

    public IReadOnlyList<Command> Commands { get; set; }
}

public sealed class TelegramContextAccessor : ITelegramContextAccessor
{
    private UserContext? _userContext;

    private ChatContext? _chatContext;

    public UserContext UserContext
    {
        get => _userContext ?? throw new InvalidOperationException("User context has not been initialized");
        set => _userContext = value;
    }

    public ChatContext ChatContext
    {
        get => _chatContext ?? throw new InvalidOperationException("Chat context has not been initialized");
        set => _chatContext = value;
    }

    public IReadOnlyList<Command> Commands { get; set; } = [];
}
