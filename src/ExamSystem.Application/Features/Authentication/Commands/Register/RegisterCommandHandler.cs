using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ExamSystem.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
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

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
            if (isEmailExist)
                return Result.Fail(Error.Validation("Email Already Exists", "This email is already in use. try anther email."));

            ApplicationUser user;
            if (request.Role == DTOs.RoleDto.Doctor)
                user = new Doctor { Email = request.Email, FullName = request.FullName, UserName = Guid.NewGuid().ToString() };
            else
                user = new Student { Email = request.Email, FullName = request.FullName, UserName = Guid.NewGuid().ToString() };

            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (!createUserResult.Succeeded)
                return Result.Fail(createUserResult.Errors.Select(e => Error.Validation("User Creation Failed", e.Description)).ToList());

            var addToToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());
            if (!addToToRoleResult.Succeeded)
                return Result.Fail(addToToRoleResult.Errors.Select(x => Error.Validation("Role Assignment Failed", x.Description)).ToList());


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForConfirmEmailAsync(user, encodedToken));
            return Result.Ok("Registration successful. Please confirm your email & login.");
        }
    }
}
