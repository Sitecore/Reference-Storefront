//-----------------------------------------------------------------------
// <copyright file="GetAvailableRegionsRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The get available regions request.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Arguments
{
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetAvailableRegionsRequest class.
    /// </summary>
    public class GetAvailableRegionsRequest : Sitecore.Commerce.Services.ServiceProviderRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableRegionsRequest"/> class.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        public GetAvailableRegionsRequest(string countryCode)
        {
            Assert.ArgumentNotNullOrEmpty(countryCode, "countryCode");
            this.CountryCode = countryCode;
        }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }
    }
}