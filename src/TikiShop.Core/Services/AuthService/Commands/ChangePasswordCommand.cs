using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ChangePasswordCommand(string Email, string OldPassword, string NewPassword) : IRequest<ResultObject<int>>;

internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResultObject<int>>
{
    private readonly ILogService<ChangePasswordCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(ILogService<ChangePasswordCommandHandler> logService,
        UserManager<User> userManager)
    {
        _logService = logService;
        _userManager = userManager;
    }

    public async Task<ResultObject<int>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Changing password for user ID: {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Change password attempt for invalid user ID: {request.Email}");
            return ResultObject<int>.Failed("Invalid User");
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"Failed to change password for user ID {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        _logService.LogInformation($"Password changed successfully for user ID: {request.Email}");
        return ResultObject<int>.Success();
    }
}