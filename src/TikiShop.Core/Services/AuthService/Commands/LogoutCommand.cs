using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record LogoutCommand(string Email) : IRequest<ResultObject<int>>;

internal class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResultObject<int>>
{
    private readonly ILogService<LogoutCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public LogoutCommandHandler(ILogService<LogoutCommandHandler> logService, UserManager<User> userManager)
    {
        _logService = logService;
        _userManager = userManager;
    }

    public async Task<ResultObject<int>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Logging out user with email {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Logout attempt for invalid email {request.Email}");
            return ResultObject<int>.Failed("Request Invalid");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(i => i.Description);
            _logService.LogError($"Logout failed for {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        _logService.LogInformation($"User logged out successfully: {request.Email}");
        return ResultObject<int>.Success();
    }
}