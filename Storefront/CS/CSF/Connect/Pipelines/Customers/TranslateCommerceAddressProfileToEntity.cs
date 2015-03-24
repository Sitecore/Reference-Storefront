//-----------------------------------------------------------------------
// <copyright file="TranslateCommerceAddressProfileToEntity.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline processor used to translate a Commerce Server address to a Party.</summary>
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
    using CommerceServer.Core.Runtime.Profiles;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Defines the TranslateCommerceAddressProfileToEntity class.
    /// </summary>
    public class TranslateCommerceAddressProfileToEntity : CommerceTranslateProcessor
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
            Assert.ArgumentCondition(args.Request is TranslateCommerceAddressProfileToEntityRequest, "args.Request ", "args.Request is TranslateCommerceProfileToEntityRequest");

            TranslateCommerceAddressProfileToEntityRequest request = (TranslateCommerceAddressProfileToEntityRequest)args.Request;
            Assert.ArgumentNotNull(request.SourceProfile, "request.SourceProfile");
            Assert.ArgumentNotNull(request.DestinationParty, "request.DestinationParty");

            if (request.DestinationParty is RefSFModels.CommerceParty)
            {
                this.TranslateToCommerceParty(request.SourceProfile, request.DestinationParty as RefSFModels.CommerceParty);
            }
            else
            {
                this.TranslateToCustomParty(request.SourceProfile, request.DestinationParty);
            }
        }

        /// <summary>
        /// Translates to commerce party.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="party">The party.</param>
        protected virtual void TranslateToCommerceParty(CommerceServer.Core.Runtime.Profiles.Profile profile, RefSFModels.CommerceParty party)
        {
            party.ExternalId = this.Get<string>(profile, "GeneralInfo.address_id");
            party.FirstName = this.Get<string>(profile, "GeneralInfo.first_name");
            party.LastName = this.Get<string>(profile, "GeneralInfo.last_name");
            party.Name = this.Get<string>(profile, "GeneralInfo.address_name");
            party.Address1 = this.Get<string>(profile, "GeneralInfo.address_line1");
            party.Address2 = this.Get<string>(profile, "GeneralInfo.address_line2");
            party.City = this.Get<string>(profile, "GeneralInfo.city");
            party.RegionCode = this.Get<string>(profile, "GeneralInfo.region_code");
            party.RegionName = this.Get<string>(profile, "GeneralInfo.region_name");
            party.ZipPostalCode = this.Get<string>(profile, "GeneralInfo.postal_code");
            party.CountryCode = this.Get<string>(profile, "GeneralInfo.country_code");
            party.Country = this.Get<string>(profile, "GeneralInfo.country_name");
            party.PhoneNumber = this.Get<string>(profile, "GeneralInfo.tel_number");
            party.State = this.Get<string>(profile, "GeneralInfo.region_code");

            this.TranslateToCommercePartyCustomProperties(profile, party);
        }

        /// <summary>
        /// Translates to commerce party custom properties.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="party">The party.</param>
        protected virtual void TranslateToCommercePartyCustomProperties(Profile profile, RefSFModels.CommerceParty party)
        {
        }

        /// <summary>
        /// Translates to custom party.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="party">The party.</param>
        protected virtual void TranslateToCustomParty(CommerceServer.Core.Runtime.Profiles.Profile profile, Party party)
        {
        }

        /// <summary>
        /// Gets the specified profile.
        /// </summary>
        /// <typeparam name="T">Type of the property in the Commerce Server profile object.</typeparam>
        /// <param name="profile">The profile.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The Commerce Server profile property value.
        /// </returns>
        protected T Get<T>(Profile profile, string propertyName)
        {
            if (profile[propertyName].Value == System.DBNull.Value)
            {
                return default(T);
            }

            return (T)profile[propertyName].Value;
        }
    }
}