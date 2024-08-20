using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Mohammed Yassen",
                    Email ="mohammedyassen728@gmail.com",
                    UserName = "mohammedYassen",
                    PhoneNumber="01094046114"
                };

                await userManager.CreateAsync(User,"Pa$$w0rd");
            }

        }
    }
}
