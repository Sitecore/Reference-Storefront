//-----------------------------------------------------------------------
// <copyright file="ForgotPasswordBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Emits the Json result of a reset password.</summary>
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
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Services;

    /// <summary>
    /// Defines the ForgotPasswordBaseJsonResult class.
    /// </summary>
    public class ForgotPasswordBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordBaseJsonResult"/> class.
        /// </summary>
        public ForgotPasswordBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public ForgotPasswordBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {            
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Initializes the specified user name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public virtual void Initialize(string userName)
        {
            Assert.ArgumentNotNullOrEmpty(userName, "userName");

            this.UserName = userName;
        }
    }
}