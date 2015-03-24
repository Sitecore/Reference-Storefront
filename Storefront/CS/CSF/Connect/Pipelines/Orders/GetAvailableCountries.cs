//-----------------------------------------------------------------------
// <copyright file="GetAvailableCountries.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline responsible for returning the available countries.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Orders
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetAvailableCountries class.
    /// </summary>
    public class GetAvailableCountries : CommerceOrderPipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is GetAvailableCountriesRequest, "args.Request", "args.Request is GetAvailableCountriesRequest");
            Assert.ArgumentCondition(args.Result is GetAvailableCountriesResult, "args.Result", "args.Result is GetAvailableCountriesResult");

            var request = (GetAvailableCountriesRequest)args.Request;
            var result = (GetAvailableCountriesResult)args.Result;

            var countryDictionary = new Dictionary<string, string>();

            foreach (Item country in this.GetCountryAndRegionItem().GetChildren())
            {
                countryDictionary.Add(country[CommerceServerStorefrontConstants.KnownFieldNames.CountryCode], country[CommerceServerStorefrontConstants.KnownFieldNames.CountryName]);
            }

            result.AvailableCountries = countryDictionary;
        }

        /// <summary>
        /// Gets the country and region item.
        /// </summary>
        /// <returns>The country and region item from Sitecore.</returns>
        protected virtual Item GetCountryAndRegionItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Countries and Geographical Regions");
        }
    }
}