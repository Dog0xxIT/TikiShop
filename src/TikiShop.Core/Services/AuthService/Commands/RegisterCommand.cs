using TikiShop.Core.Services.BasketService.Commands;
using TikiShop.Core.Services.EmailService;
using TikiShop.Infrastructure.Common;
using TikiShop.Model.DTO;
using TikiShop.Model.Enums;

namespace TikiShop.Core.Services.AuthService.Commands;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<ResultObject<int>>;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResultObject<int>>
{
    private readonly IEmailSender<User> _emailSender;
    private readonly ILogService<RegisterCommandHandler> _logService;
    private readonly IMediator _mediator;
    private readonly UserManager<User> _userManager;

    public RegisterCommandHandler(
        ILogService<RegisterCommandHandler> logService,
        UserManager<User> userManager,
        IEmailSender<User> emailSender,
        IMediator mediator)
    {
        _logService = logService;
        _userManager = userManager;
        _emailSender = emailSender;
        _mediator = mediator;
    }

    public async Task<ResultObject<int>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logService.LogInformation($"Registering user with email {request.Email}");
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"User registration failed for {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        identityResult = await _userManager.AddToRoleAsync(user, RolesConstant.Customer);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(e => e.Description);
            _logService.LogError($"Failed to add role for {request.Email}: {string.Join(", ", errors)}");
            return ResultObject<int>.Failed(errors);
        }

        var userCreated = await _userManager.Users
            .AsNoTracking()
            .Select(u => new { u.Email, u.Id })
            .SingleAsync(u => u.Email == user.Email);
        var serviceResult = await _mediator.Send(new CreateBasketCommand(Convert.ToInt32(userCreated.Id)));
        if (serviceResult.ResultCode == ResultCode.Failure)
        {
            _logService.LogError(
                $"Failed to create basket for {request.Email}: {serviceResult.Message}");
            return ResultObject<int>.Failed(serviceResult.Message);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = EmailSenderGenerateLink.GenerateConfirmLink(token, request.Email);
        await _emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
        _logService.LogInformation($"Confirmation email sent to {request.Email}");
        return ResultObject<int>.Success();
    }
}