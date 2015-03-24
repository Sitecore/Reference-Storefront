//-----------------------------------------------------------------------
// <copyright file="RegisterBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Emits the Json result of a Register request.</summary>
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
    using Sitecore.Commerce.Services.Customers;

    /// <summary>
    /// Defines the RegisterJsonResult class.
    /// </summary>
    public class RegisterBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterBaseJsonResult"/> class.
        /// </summary>
        public RegisterBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public RegisterBaseJsonResult(CreateUserResult result)
            : base(result)
        {            
        }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Initializes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public virtual void Initialize(CommerceUser user)
        {
            this.UserName = user.UserName;                       
        }
    }
}