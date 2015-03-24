//-----------------------------------------------------------------------
// <copyright file="TranslateCommerceAddressProfileToEntityRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Request class for the pipeline responsible for translating a Commerce Server address to a Party.</summary>
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
    /// Defines the TranslateCommerceAddressProfileToEntityRequest class.
    /// </summary>
    public class TranslateCommerceAddressProfileToEntityRequest : CommerceRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateCommerceAddressProfileToEntityRequest"/> class.
        /// </summary>
        /// <param name="sourceProfile">The source profile.</param>
        /// <param name="destinationParty">The destination party.</param>
        public TranslateCommerceAddressProfileToEntityRequest([NotNull] Profile sourceProfile, [NotNull] RefSFModels.CommerceParty destinationParty)
        {
            Assert.ArgumentNotNull(sourceProfile, "commerceProfile");
            Assert.ArgumentNotNull(destinationParty, "customerParty");

            this.SourceProfile = sourceProfile;
            this.DestinationParty = destinationParty;
        }

        /// <summary>
        /// Gets or sets the source profile.
        /// </summary>
        /// <value>
        /// The source profile.
        /// </value>
        public Profile SourceProfile { get; set; }

        /// <summary>
        /// Gets or sets the destination party.
        /// </summary>
        /// <value>
        /// The destination party.
        /// </value>
        public RefSFModels.CommerceParty DestinationParty { get; set; }
    }
}