using CarPartBotApi.Application.Contexts;
using CarPartBotApi.Application.Dto;

namespace CarPartBotApi.Application.Interfaces;

public interface ICommandDataReader
{
    public ChatContext ExtractChatContext();

    public UserContext ExtractUserContext();

    public List<Command> GetCommands();

    public string GetMessageText();

    public bool HasCommands();
}
