//-----------------------------------------------------------------------
// <copyright file="TranslateEntityToCommerceAddressProfileRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Request class for the pipeline responsible for translating a Party to a Commerce Server address.</summary>
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
    using CommerceServer.Core.Runtime.Profiles;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Diagnostics;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the TranslateEntityToCommerceAddressProfileRequest class.
    /// </summary>
    public class TranslateEntityToCommerceAddressProfileRequest : CommerceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateEntityToCommerceAddressProfileRequest"/> class.
        /// </summary>
        /// <param name="sourceParty">The source party.</param>
        /// <param name="destinationProfile">The destination profile.</param>
        public TranslateEntityToCommerceAddressProfileRequest([NotNull] RefSFModels.CommerceParty sourceParty, [NotNull] Profile destinationProfile)
        {
            Assert.ArgumentNotNull(destinationProfile, "commerceProfile");
            Assert.ArgumentNotNull(sourceParty, "customerParty");

            this.DestinationProfile = destinationProfile;
            this.SourceParty = sourceParty;
        }

        /// <summary>
        /// Gets or sets the destination profile.
        /// </summary>
        /// <value>
        /// The destination profile.
        /// </value>
        public Profile DestinationProfile { get; set; }

        /// <summary>
        /// Gets or sets the source party.
        /// </summary>
        /// <value>
        /// The source party.
        /// </value>
        public RefSFModels.CommerceParty SourceParty { get; set; }
    }
}