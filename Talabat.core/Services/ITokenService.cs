﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Core.Services
{
    public interface ITokenService
    {
        //Signature for Function To Create Token
        Task<string> CreateTokenAsync(AppUser User, UserManager<AppUser> userManager);
    }
}
