using CarPartBotApi.Api.Constants;
using CarPartBotApi.Api.Extensions;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace CarPartBotApi.Api.Contollers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController(
    IOptionsSnapshot<TelegramSettings> _options,
    ITelegramService _telegramService) 
    : ControllerBase
{
    // TODO add rate limiter
    [HttpPost("telegram")]
    [AllowAnonymous]
    public async Task<IActionResult> ProcessTelegramWebhook(CancellationToken ct)
    {
        var webhookSecretToken = HttpContext.GetHeaderValueOrNull(HeaderNames.WebhookSecretToken);

        if (webhookSecretToken is null)
        {
            // TODO log.
            return Unauthorized(new { error = "Webhook secret token is missing." });
        }

        if (_options.Value.WebhookSecretToken != webhookSecretToken)
        {
            // TODO log.
            return Unauthorized(new { error = "Webhook secret token is invalid." });
        }

        var rawBody = await HttpContext.ReadRequestBodyAsStringAsync(ct);

        // TODO process asynchronously.
        await _telegramService.ProcessTelegramEvent(rawBody, ct);

        return Ok();
    }
}
