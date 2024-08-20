using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration Configuration;
        public TokenService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager)
        {
            //PayLoad
            //1- Private Claims
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName , User.DisplayName),
                new Claim(ClaimTypes.Email, User.Email)
            };
            var UserRoles = await userManager.GetRolesAsync(User);
            foreach (var Role in UserRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));
            }
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]));

            //Register Claims

            var Token = new JwtSecurityToken(
                issuer: Configuration["JWT:ValidIssure"],
                audience: Configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse(Configuration["JWT:DurationInDays"])),
                claims:AuthClaims,
                signingCredentials: new SigningCredentials(AuthKey,SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
    }
}
