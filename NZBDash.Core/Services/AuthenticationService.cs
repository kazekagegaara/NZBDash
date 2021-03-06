﻿#region Copyright
// /************************************************************************
//   Copyright (c) 2015 Jamie Rees
//   File: AuthenticationService.cs
//   Created By: Jamie Rees
//  
//   Permission is hereby granted, free of charge, to any person obtaining
//   a copy of this software and associated documentation files (the
//   "Software"), to deal in the Software without restriction, including
//   without limitation the rights to use, copy, modify, merge, publish,
//   distribute, sublicense, and/or sell copies of the Software, and to
//   permit persons to whom the Software is furnished to do so, subject to
//   the following conditions:
//  
//   The above copyright notice and this permission notice shall be
//   included in all copies or substantial portions of the Software.
//  
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//   OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//   WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ************************************************************************/
#endregion

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;

using NZBDash.Common.Interfaces;
using NZBDash.Core.Interfaces;
using NZBDash.DataAccessLayer.Interfaces;
using NZBDash.DataAccessLayer.Models;
using NZBDash.DataAccessLayer.Store;

namespace NZBDash.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService(ILogger logger, ISqliteConfiguration config)
            : this(new UserManager<User>(new UserStore(logger, config)))
        {
        }

        public AuthenticationService(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        private UserManager<User> UserManager { get; set; }

        public User GetUser(string userName)
        {
            return UserManager.FindByName(userName);
        }

        public IQueryable<User> GetAllUsers()
        {
            return UserManager.Users;
        }
    }
}