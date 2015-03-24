//-----------------------------------------------------------------------
// <copyright file="UserBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the UserBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Entities.Customers;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Services;

    /// <summary>
    /// Emits the Json result of a get user request.
    /// </summary>
    public class UserBaseJsonResult : BaseJsonResult
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="UserBaseJsonResult"/> class.
        /// </summary>
        public UserBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public UserBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the email result.
        /// </summary>
        /// <value>
        /// The email result.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Initializes this object based on the data contained in the provided cart.
        /// </summary>
        /// <param name="user">The user.</param>
        public virtual void Initialize(CommerceUser user)
        {
            Assert.ArgumentNotNull(user, "user");

            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.FullName = string.Concat(this.FirstName, string.Empty, this.LastName);
            this.Email = user.Email;
        }
    }
}