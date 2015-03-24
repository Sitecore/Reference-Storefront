//-----------------------------------------------------------------------
// <copyright file="TranslateEntityToCommerceAddressProfile.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline processor used to translate a Party to a Commerce Server address .</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Customers
{
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the TranslateEntityToCommerceAddressProfile class.
    /// </summary>
    public class TranslateEntityToCommerceAddressProfile : CommerceTranslateProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.Request, "args.request");
            Assert.ArgumentNotNull(args.Result, "args.result");
            Assert.ArgumentCondition(args.Request is TranslateEntityToCommerceAddressProfileRequest, "args.Request ", "args.Request is TranslateEntityToCommerceAddressProfileRequest");

            var request = (TranslateEntityToCommerceAddressProfileRequest)args.Request;
            Assert.ArgumentNotNull(request.SourceParty, "request.SourceParty");
            Assert.ArgumentNotNull(request.DestinationProfile, "request.DestinationProfile");

            if (request.SourceParty is RefSFModels.CommerceParty)
            {
                this.TranslateCommerceCustomerParty(request.SourceParty as RefSFModels.CommerceParty, request.DestinationProfile);
            }
            else
            {
                this.TranslateCustomParty(request.SourceParty, request.DestinationProfile);
            }
        }

        /// <summary>
        /// Translates the commerce customer party.
        /// </summary>
        /// <param name="party">The party.</param>
        /// <param name="profile">The profile.</param>
        protected virtual void TranslateCommerceCustomerParty(RefSFModels.CommerceParty party, CommerceServer.Core.Runtime.Profiles.Profile profile)
        {
            profile["GeneralInfo.first_name"].Value = party.FirstName;
            profile["GeneralInfo.last_name"].Value = party.LastName;
            profile["GeneralInfo.address_name"].Value = party.Name;
            profile["GeneralInfo.address_line1"].Value = party.Address1;
            profile["GeneralInfo.address_line2"].Value = party.Address2;
            profile["GeneralInfo.city"].Value = party.City;
            profile["GeneralInfo.region_code"].Value = party.RegionCode;
            profile["GeneralInfo.region_name"].Value = party.RegionName;
            profile["GeneralInfo.postal_code"].Value = party.ZipPostalCode;
            profile["GeneralInfo.country_code"].Value = party.CountryCode;
            profile["GeneralInfo.country_name"].Value = party.Country;
            profile["GeneralInfo.tel_number"].Value = party.PhoneNumber;
            profile["GeneralInfo.region_code"].Value = party.State;

            this.TranslateCommerceCustomerPartyCustomProperties(party, profile);
        }

        /// <summary>
        /// Translates the commerce customer party custom properties.
        /// </summary>
        /// <param name="party">The party.</param>
        /// <param name="profile">The profile.</param>
        protected virtual void TranslateCommerceCustomerPartyCustomProperties(RefSFModels.CommerceParty party, CommerceServer.Core.Runtime.Profiles.Profile profile)
        {
        }

        /// <summary>
        /// Translates the custom party.
        /// </summary>
        /// <param name="party">The party.</param>
        /// <param name="profile">The profile.</param>
        private void TranslateCustomParty(RefSFModels.CommerceParty party, CommerceServer.Core.Runtime.Profiles.Profile profile)
        {
        }
    }
}