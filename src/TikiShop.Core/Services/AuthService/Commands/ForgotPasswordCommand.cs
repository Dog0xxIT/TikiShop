using TikiShop.Core.Services.EmailService;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<ResultObject<int>>;

internal class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResultObject<int>>
{
    private readonly IEmailSender<User> _emailSender;
    private readonly ILogService<ForgotPasswordCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public ForgotPasswordCommandHandler(ILogService<ForgotPasswordCommandHandler> logService, IEmailSender<User> emailSender,
        UserManager<User> userManager)
    {
        _logService = logService;
        _emailSender = emailSender;
        _userManager = userManager;
    }

    public async Task<ResultObject<int>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Initiating password reset for email: {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logService.LogWarning($"Password reset attempt for invalid email: {request.Email}");
            return ResultObject<int>.Failed("Invalid Email");
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResetLink = EmailSenderGenerateLink.GenerateResetLink(code, request.Email);
        await _emailSender.SendPasswordResetLinkAsync(user, request.Email, passwordResetLink);

        _logService.LogInformation($"Password reset link sent to email: {request.Email}");
        return ResultObject<int>.Success();
    }
}