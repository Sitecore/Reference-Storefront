//-----------------------------------------------------------------------
// <copyright file="GetUser.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline the retrieves the user.</summary>
//-----------------------------------------------------------------------
// Copyright 2015 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront.Connect.Pipelines
{
    using Sitecore.Commerce.Data.Customers;
    using Sitecore.Commerce.Pipelines.Customers.GetUser;
    using Sitecore.Commerce.Services.Customers;
    using Sitecore.Diagnostics;
    using Sitecore.Security;
    using Sitecore.Security.Accounts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetUser class.
    /// </summary>
    public class GetUser : GetUserFromSitecore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUser"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public GetUser(IUserRepository userRepository)
            : base(userRepository)
        {
        }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            base.Process(args);

            GetUserRequest request = (GetUserRequest)args.Request;
            GetUserResult result = (GetUserResult)args.Result;

            if (result.CommerceUser == null)
            {
                return;
            }

            // if we found a user, add some addition info
            var userProfile = GetUserProfile(result.CommerceUser.UserName);
            Assert.IsNotNull(userProfile, "profile");

            UpdateCustomer(result.CommerceUser, userProfile);
        }

        /// <summary>
        /// Updates the customer with some additional data.
        /// </summary>
        /// <param name="commerceUser">The commerce user to update.</param>
        /// <param name="userProfile">The user profile of the user.</param>
        protected void UpdateCustomer(Sitecore.Commerce.Entities.Customers.CommerceUser commerceUser, Sitecore.Security.UserProfile userProfile)
        {
            commerceUser.ExternalId = userProfile["user_id"];
            Assert.IsNotNullOrEmpty(commerceUser.ExternalId, "commerceUser.ExternalId");

            if (commerceUser.Customers == null || commerceUser.Customers.Count == 0)
            {
                var customers = new List<string>() { commerceUser.ExternalId };
                commerceUser.Customers = customers.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the user profile from the external system.
        /// </summary>
        /// <param name="userName">The username of the profile to retrieve.</param>
        /// <returns>The profile of the user</returns>
        protected UserProfile GetUserProfile(string userName)
        {
            return User.FromName(userName, true).Profile;
        }
    }
}