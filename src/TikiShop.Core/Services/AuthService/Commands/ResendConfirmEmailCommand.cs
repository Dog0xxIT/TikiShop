using TikiShop.Core.ThirdServices.EmailService;
using TikiShop.Model.DTO;

namespace TikiShop.Core.Services.AuthService.Commands;

public record ResendConfirmEmailCommand(string Email) : IRequest<ResultObject<int>>;

internal class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand, ResultObject<int>>
{
    private readonly IEmailSender<User> _emailSender;
    private readonly ILogService<ResendConfirmEmailCommandHandler> _logService;
    private readonly UserManager<User> _userManager;

    public ResendConfirmEmailCommandHandler(ILogService<ResendConfirmEmailCommandHandler> logService,
        UserManager<User> userManager,
        IEmailSender<User> emailSender)
    {
        _logService = logService;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task<ResultObject<int>> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Resending confirmation email to {request.Email}");
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logService.LogWarning($"Invalid email for confirmation resend: {request.Email}");
            return ResultObject<int>.Failed("Invalid Email");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, request.Email);
        await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
        _logService.LogInformation($"Confirmation email resent to {request.Email}");
        return ResultObject<int>.Success();
    }
}