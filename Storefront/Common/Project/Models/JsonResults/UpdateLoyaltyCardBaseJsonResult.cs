//-----------------------------------------------------------------------
// <copyright file="UpdateLoyaltyCardBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the UpdateLoyaltyCardBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The Json result of a request to retrieve the available states.
    /// </summary>
    public class UpdateLoyaltyCardBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLoyaltyCardBaseJsonResult"/> class.
        /// </summary>
        public UpdateLoyaltyCardBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLoyaltyCardBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public UpdateLoyaltyCardBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the loyalty card was successfully updated.
        /// </summary>
        public bool WasUpdated { get; set; }

        /// <summary>
        /// Initializes the specified was updated.
        /// </summary>
        /// <param name="wasUpdated">if set to <c>true</c> [was updated].</param>
        public virtual void Initialize(bool wasUpdated)
        {
            Assert.ArgumentNotNull(wasUpdated, "wasUpdated");

            this.WasUpdated = wasUpdated;
        }
    }
}