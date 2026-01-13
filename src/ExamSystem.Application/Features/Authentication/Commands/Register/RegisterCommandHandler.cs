using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterCommandHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService, UserManager<ApplicationUser> userManager)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
            if (isEmailExist)
                return Result<string>.Fail(Error.Validation("Email Already Exists", "This email is already in use. try anther email."));

            ApplicationUser user;
            if (request.Role == DTOs.RoleDto.Doctor)
                user = new Doctor { Email = request.Email, FullName = request.FullName, UserName = Guid.NewGuid().ToString() };
            else
                user = new Student { Email = request.Email, FullName = request.FullName, UserName = Guid.NewGuid().ToString() };

            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (!createUserResult.Succeeded)
                return Result<string>.Fail(createUserResult.Errors.Select(e => Error.Validation("User Creation Failed", e.Description)).ToList());

            var addToToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());
            if (!addToToRoleResult.Succeeded)
                return Result<string>.Fail(addToToRoleResult.Errors.Select(x => Error.Validation("Role Assignment Failed", x.Description)).ToList());


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForConfirmEmailAsync(user, token));
            return Result<string>.Ok("Registration successful. Please confirm your email & login.");
        }
    }
}
