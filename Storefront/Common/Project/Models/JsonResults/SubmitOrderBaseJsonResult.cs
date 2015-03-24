//-----------------------------------------------------------------------
// <copyright file="SubmitOrderBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Emits the Json result of a Submit Order request.</summary>
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
    using System.Diagnostics.CodeAnalysis;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The Json result of a Submit Order request.
    /// </summary>
    public class SubmitOrderBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitOrderBaseJsonResult"/> class.
        /// </summary>
        public SubmitOrderBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitOrderBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public SubmitOrderBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the order confirmation page URL.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Needs to be a string for json serialization.")]
        public string ConfirmUrl { get; set; }

        /// <summary>
        /// Initializes the specified confirm URL.
        /// </summary>
        /// <param name="confirmUrl">The confirm URL.</param>
        public virtual void Initialize(string confirmUrl)
        {
            Assert.ArgumentNotNullOrEmpty(confirmUrl, "confirmUrl");

            this.ConfirmUrl = confirmUrl;
        }
    }
}