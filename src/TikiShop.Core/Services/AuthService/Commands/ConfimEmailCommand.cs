using Microsoft.AspNetCore.WebUtilities;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ConfirmEmailCommand(string Email, string Code) : IRequest<ResultObject<int>>;

internal class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ResultObject<int>>
{
    private readonly ILogService<ConfirmEmailCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(ILogService<ConfirmEmailCommandHandler> logService, UserManager<User> userManager)
    {
        _logService = logService;
        _userManager = userManager;
    }

    public async Task<ResultObject<int>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Confirming email for {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Invalid email confirmation attempt for {request.Email}");
            return ResultObject<int>.Failed("Invalid Email");
        }

        var encode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        var identityResult = await _userManager.ConfirmEmailAsync(user, encode);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(i => i.Description);
            _logService.LogError($"Email confirmation failed for {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        _logService.LogInformation($"Email confirmed for {request.Email}");
        return ResultObject<int>.Success();
    }
}