using Microsoft.AspNetCore.Authentication;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ExternalLoginCommand(string Provider, string RedirectUrl)
    : IRequest<ResultObject<AuthenticationProperties>>;

internal class
    ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ResultObject<AuthenticationProperties>>
{
    private readonly ILogService<ExternalLoginCommandHandler> _logService;
    private readonly SignInManager<User> _signInManager;

    public ExternalLoginCommandHandler(ILogService<ExternalLoginCommandHandler> logService, SignInManager<User> signInManager)
    {
        _logService = logService;
        _signInManager = signInManager;
    }

    public async Task<ResultObject<AuthenticationProperties>> Handle(ExternalLoginCommand request,
        CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Initiating external login with provider: {request.Provider}");
        var authSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (authSchemes.All(s => s.Name != request.Provider))
        {
            _logService.LogWarning($"Invalid external login provider: {request.Provider}");
            return ResultObject<AuthenticationProperties>.Failed("Provider Invalid");
        }

        var properties =
            _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, request.RedirectUrl);
        _logService.LogInformation($"Configured external authentication properties for provider: {request.Provider}");
        return ResultObject<AuthenticationProperties>.Success(properties);
    }
}