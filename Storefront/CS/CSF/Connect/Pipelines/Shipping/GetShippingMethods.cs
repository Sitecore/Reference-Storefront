//-----------------------------------------------------------------------
// <copyright file="GetShippingMethods.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline responsible for returning the shipping methods.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Shipping
{
    using Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services.Shipping;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Defines the GetShippingMethods class.
    /// </summary>
    public class GetShippingMethods : Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines.GetShippingMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetShippingMethods"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetShippingMethods([NotNull] IEntityFactory entityFactory) : base(entityFactory)
        {
        }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is CommerceGetShippingMethodsRequest, "args.Request", "args.Request is CommerceGetShippingMethodsRequest");
            Assert.ArgumentCondition(args.Result is GetShippingMethodsResult, "args.Result", "args.Result is GetShippingMethodsResult");

            var request = (CommerceGetShippingMethodsRequest)args.Request;
            var result = (GetShippingMethodsResult)args.Result;

            Assert.ArgumentNotNullOrEmpty(request.Language, "request.Language");

            if (request.ShippingOption.ShippingOptionType == null)
            {
                base.Process(args);
                return;
            }

            Item shippingOptionsItem = this.GetShippingOptionsItem();

            string query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", shippingOptionsItem.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.ShippingOptionValue, request.ShippingOption.ShippingOptionType.Value);
            Item foundOption = shippingOptionsItem.Database.SelectSingleItem(query);
            if (foundOption != null)
            {
                string shippingMethodsIds = foundOption[CommerceServerStorefrontConstants.KnownFieldNames.CommerceServerShippingMethods];
                if (!string.IsNullOrWhiteSpace(shippingMethodsIds))
                {
                    base.Process(args);
                    if (result.Success)
                    {
                        List<ShippingMethod> currentList = new List<ShippingMethod>(result.ShippingMethods);
                        List<ShippingMethod> returnList = new List<ShippingMethod>();

                        string[] ids = shippingMethodsIds.Split('|');
                        foreach (string id in ids)
                        {
                            string trimmedId = id.Trim();

                            var found2 = currentList.Find(o => o.ExternalId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase));
                            ShippingMethod found = currentList.Find(o => o.ExternalId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase)) as ShippingMethod;
                            if (found != null)
                            {
                                returnList.Add(found);
                            }
                        }

                        result.ShippingMethods = new System.Collections.ObjectModel.ReadOnlyCollection<ShippingMethod>(returnList);
                    }
                }

                // We need to do this type casting for now until the base OBEC classes support the additional properties.  Setting the shipping
                // methods calls this pipeline processor for validation purposes (call is made by CS integration) and we must allow to be called using
                // the original CS integration classes.
                if (request is Services.Orders.GetShippingMethodsRequest && result is Services.Orders.GetShippingMethodsResult)
                {
                    var obecRequest = (Services.Orders.GetShippingMethodsRequest)request;
                    var obecResult = (Services.Orders.GetShippingMethodsResult)result;

                    if (obecRequest.Lines != null && obecRequest.Lines.Any())
                    {
                        var shippingMethodPerItemList = new List<ShippingMethodPerItem>();

                        foreach (var line in obecRequest.Lines)
                        {
                            var shippingMethodPerItem = new ShippingMethodPerItem();

                            shippingMethodPerItem.LineId = line.ExternalCartLineId;
                            shippingMethodPerItem.ShippingMethods = result.ShippingMethods;

                            shippingMethodPerItemList.Add(shippingMethodPerItem);
                        }

                        obecResult.ShippingMethodsPerItems = new System.Collections.ObjectModel.ReadOnlyCollection<ShippingMethodPerItem>(shippingMethodPerItemList);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the shipping option item.
        /// </summary>
        /// <returns>The Shipping Options item from Sitecore.</returns>
        protected virtual Item GetShippingOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Shipping Options");
        }
    }
}