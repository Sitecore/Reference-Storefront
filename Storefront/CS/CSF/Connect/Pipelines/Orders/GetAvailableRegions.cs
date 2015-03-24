//-----------------------------------------------------------------------
// <copyright file="GetAvailableRegions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline responsible for returning the available regions of a given country.</summary>
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
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetAvailableRegions class.
    /// </summary>
    public class GetAvailableRegions : CommerceOrderPipelineProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is GetAvailableRegionsRequest, "args.Request", "args.Request is GetAvailableRegionsRequest");
            Assert.ArgumentCondition(args.Result is GetAvailableRegionsResult, "args.Result", "args.Result is GetAvailableRegionsResult");

            var request = (GetAvailableRegionsRequest)args.Request;
            var result = (GetAvailableRegionsResult)args.Result;

            var regions = new Dictionary<string, string>();

            //// The country guid is passed into the coutry code.
            //// Item countryItem = Sitecore.Context.Database.GetItem(Sitecore.Data.ID.Parse(request.CountryCode));

            Item countryAndRegionItem = this.GetCountryAndRegionItem();
            string countryAndRegionPath = countryAndRegionItem.Paths.FullPath;
            string query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", countryAndRegionPath, CommerceServerStorefrontConstants.KnownFieldNames.CountryCode, request.CountryCode);
            Item foundCountry = countryAndRegionItem.Database.SelectSingleItem(query);
            if (foundCountry != null)
            {
                foreach (Item region in foundCountry.GetChildren())
                {
                    regions.Add(region.ID.ToGuid().ToString(), region[CommerceServerStorefrontConstants.KnownFieldNames.RegionName]);
                }
            }

            result.AvailableRegions = regions;
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