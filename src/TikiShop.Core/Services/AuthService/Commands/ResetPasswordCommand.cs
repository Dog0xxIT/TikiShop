using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ResetPasswordCommand(string Email, string ResetCode, string NewPassword) : IRequest<ResultObject<int>>;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResultObject<int>>
{
    private readonly ILogService<ResetPasswordCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public ResetPasswordCommandHandler(ILogService<ResetPasswordCommandHandler> logService, UserManager<User> userManager)
    {
        _logService = logService;
        _userManager = userManager;
    }

    public async Task<ResultObject<int>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Resetting password for user ID: {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Reset password attempt for invalid user ID: {request.Email}");
            return ResultObject<int>.Failed("Invalid Email");
        }

        var identityResult = await _userManager.ResetPasswordAsync(user, request.ResetCode, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"Failed to reset password for user ID {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        _logService.LogInformation($"Password reset successfully for user ID: {request.Email}");
        return ResultObject<int>.Success();
    }
}