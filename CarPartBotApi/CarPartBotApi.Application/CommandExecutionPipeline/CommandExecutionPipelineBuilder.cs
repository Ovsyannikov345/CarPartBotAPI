using CarPartBotApi.Application.Accessors;
using CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;
using CarPartBotApi.Application.Interfaces;
using CarPartBotApi.Domain.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline;

public interface ICommandExecutionPipelineBuilder
{
    public ICommandExecutionPipelineBuilder WithDataReader(ICommandDataReader reader);

    public ICommandExecutionPipelineBuilder WithValidator(Func<ITelegramContextAccessor, Result> validationFunc);

    public ICommandExecutionPipeline Build();
}

public sealed class CommandExecutionPipelineBuilder(
    IApplicationDbContext _dbContext,
    ITelegramContextAccessor _telegramContextAccessor,
    IEnumerable<ICommandHandler> _commandHandlers,
    IEnumerable<ICallbackableHandler> _callbackHandlers,
    IFailureHandler _failureHandler,
    ILogger<CommandExecutionPipeline> _pipelineLogger)
    : ICommandExecutionPipelineBuilder
{
    private ICommandDataReader? _commandDataReader;

    private readonly List<Func<ITelegramContextAccessor, Result>> _validators = [];

    public ICommandExecutionPipelineBuilder WithDataReader(ICommandDataReader reader)
    {
        _commandDataReader = reader;

        return this;
    }

    public ICommandExecutionPipelineBuilder WithValidator(Func<ITelegramContextAccessor, Result> validationFunc)
    {
        _validators.Add(validationFunc);

        return this;
    }

    public ICommandExecutionPipeline Build()
    {
        if (_commandDataReader is null)
        {
            throw new InvalidOperationException("Command reader must be provided to build a pipeline");
        }

        _telegramContextAccessor.UserContext = _commandDataReader.ExtractUserContext();
        _telegramContextAccessor.ChatContext = _commandDataReader.ExtractChatContext();
        _telegramContextAccessor.Commands = _commandDataReader.GetCommands();
        _telegramContextAccessor.Message = _commandDataReader.GetMessageText();
        _telegramContextAccessor.CallbackQuery = _commandDataReader.GetCallbackQuery();

        return new CommandExecutionPipeline(_dbContext, _telegramContextAccessor, _validators, _commandHandlers, _callbackHandlers, _failureHandler, _pipelineLogger);
    }
}
