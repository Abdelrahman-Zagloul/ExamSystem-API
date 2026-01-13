using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterCommandHandler(IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager)
        {
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
        }
        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
            if (isEmailExist)
                return Result<string>.Fail(Error.Validation("Email Already Exists", "This email is already in use. try anther email."));
            var user = new ApplicationUser { Email = request.Email, FullName = request.FullName, UserName = Guid.NewGuid().ToString() };
            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (!createUserResult.Succeeded)
                return Result<string>.Fail(createUserResult.Errors.Select(e => Error.Validation("User Creation Failed", e.Description)).ToList());

            var addToToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());
            if (!addToToRoleResult.Succeeded)
                return Result<string>.Fail(addToToRoleResult.Errors.Select(x => Error.Validation("Role Assignment Failed", x.Description)).ToList());

            //TODO: Send Confirmation Email
            return Result<string>.Ok("Send Confirm Email successfully");
        }

    }
}
