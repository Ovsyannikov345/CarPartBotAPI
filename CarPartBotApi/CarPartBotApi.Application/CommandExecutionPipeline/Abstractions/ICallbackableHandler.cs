using CarPartBotApi.Application.Dto;
using CarPartBotApi.Application.Interfaces;
using Utilities;

namespace CarPartBotApi.Application.CommandExecutionPipeline.Abstractions;

public interface ICallbackableHandler : ICommandHandler
{
    public bool CanHandleCallback(TelegramCallbackPayload callbackPayload);

    public Task<Result> HandleCallback(TelegramCallbackQuery callbackQuery, CancellationToken ct);
}
