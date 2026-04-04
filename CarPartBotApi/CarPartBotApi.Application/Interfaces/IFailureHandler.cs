using CarPartBotApi.Application.Constants.Enums;
using Utilities;

namespace CarPartBotApi.Application.Interfaces;

public interface IFailureHandler
{
    public Task<Result> Handle(HandlingFailureType failureType, CancellationToken ct);
}
