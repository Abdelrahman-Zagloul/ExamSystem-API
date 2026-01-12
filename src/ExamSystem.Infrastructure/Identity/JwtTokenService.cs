using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExamSystem.Infrastructure.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JWTSettings _jwt;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JWTSettings> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthDto> GenerateTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecertKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256);

            var JwtToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: null,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);


            var token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            return new AuthDto(token, roles.FirstOrDefault() ?? "", user.Id, JwtToken.ValidTo);
        }
    }
}
