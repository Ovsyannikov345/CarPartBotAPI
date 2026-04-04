using CarPartBotApi.Api.Constants;
using CarPartBotApi.Api.Extensions;
using CarPartBotApi.Api.Logging;
using CarPartBotApi.Application.Configuration;
using CarPartBotApi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace CarPartBotApi.Api.Contollers;

[ApiController]
[Route($"api/{WebhookConstants.WebhookControllerPath}")]
public class WebhookController(
    ITelegramService _telegramService,
    ILogger<WebhookController> _logger,
    IOptionsSnapshot<TelegramSettings> _options) 
    : ControllerBase
{
    [HttpPost(WebhookConstants.TelegramWebhookEndpointPath)]
    [AllowAnonymous]
    public async Task<IActionResult> ProcessTelegramWebhook(CancellationToken ct)
    {
        var webhookSecretToken = HttpContext.GetHeaderValueOrNull(HeaderNames.WebhookSecretToken);

        if (webhookSecretToken is null)
        {
            _logger.WebhookSecretTokenIsMissing();

            return Unauthorized(new { error = "Webhook secret token is missing." });
        }

        if (_options.Value.Webhook.SecretToken != webhookSecretToken)
        {
            _logger.WebhookSecretTokenIsInvalid();

            return Unauthorized(new { error = "Webhook secret token is invalid." });
        }

        var rawBody = await HttpContext.ReadRequestBodyAsStringAsync(ct);

        // TODO process asynchronously.
        await _telegramService.ProcessTelegramEvent(rawBody, ct);

        return Ok();
    }
}
