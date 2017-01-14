//-----------------------------------------------------------------------
// <copyright file="GetShippingMethods.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline responsible for returning the shipping methods.</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
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
    using Data;
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

            Item shippingOptionTypesFolder = this.GetShippingOptionsTypeFolder();

            string query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", shippingOptionTypesFolder.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.TypeId, request.ShippingOption.ShippingOptionType.Value);
            Item foundOptionType = shippingOptionTypesFolder.Database.SelectSingleItem(query);
            if (foundOptionType != null)
            {
                Item shippingOptionsItem = this.GetShippingOptionsItem();

                query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", shippingOptionsItem.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.FulfillmentOptionType, foundOptionType.ID);
                Item fulfillmentOptionItem = shippingOptionsItem.Database.SelectSingleItem(query);
                if (fulfillmentOptionItem != null)
                {
                    // Has methods?
                    if (fulfillmentOptionItem.HasChildren)
                    {
                        List<ShippingMethod> returnList = new List<ShippingMethod>();

                        foreach (Item fulfillmentMethodItem in fulfillmentOptionItem.GetChildren())
                        {
                            // Do we have a Commerce Server Method?
                            if (fulfillmentMethodItem.HasChildren)
                            {
                                Item csMethod = fulfillmentMethodItem.GetChildren()[0];
                                string csMethodId = csMethod[StorefrontConstants.KnownFieldNames.MethodId];
                                Assert.IsNotNullOrEmpty(csMethodId, string.Format(CultureInfo.InvariantCulture, "The CS Method of the {0} Fulfillment Method is empty.", fulfillmentMethodItem.Name));

                                ShippingMethod shippingMethod = this.EntityFactory.Create<ShippingMethod>("ShippingMethod");

                                this.TranslateShippingMethod(fulfillmentOptionItem, fulfillmentMethodItem, csMethodId, shippingMethod);

                                returnList.Add(shippingMethod);
                            }
                        }

                        result.ShippingMethods = new System.Collections.ObjectModel.ReadOnlyCollection<ShippingMethod>(returnList);

                        // We need to do this type casting for now until the base OBEC classes support the additional properties.  Setting the shipping
                        // methods calls this pipeline processor for validation purposes (call is made by CS integration) and we must allow to be called using
                        // the original CS integration classes.
                        if (request is Services.Orders.GetShippingMethodsRequest && result is GetShippingMethodsResult)
                        {
                            var obecRequest = (Services.Orders.GetShippingMethodsRequest)request;
                            var obecResult = (GetShippingMethodsResult)result;

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

                                obecResult.ShippingMethodsPerItem = new System.Collections.ObjectModel.ReadOnlyCollection<ShippingMethodPerItem>(shippingMethodPerItemList);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Translates the shipping method from an item to a ShippingMethod instance.
        /// </summary>
        /// <param name="shippingOptionItem">The shipping option item.</param>
        /// <param name="shippingMethodItem">The shipping method item.</param>
        /// <param name="csMethodId">The cs method identifier.</param>
        /// <param name="shippingMethod">The shipping method.</param>
        protected virtual void TranslateShippingMethod(Item shippingOptionItem, Item shippingMethodItem, string csMethodId, ShippingMethod shippingMethod)
        {
            Guid shippingMethodGuid = new Guid(csMethodId);

            shippingMethod.ExternalId = shippingMethodGuid.ToString("B");
            shippingMethod.Name = shippingMethodItem.Name;
            shippingMethod.Description = shippingMethod.Name;
            shippingMethod.ShippingOptionId = shippingOptionItem.ID.ToString();
        }

        /// <summary>
        /// Gets the shipping option item.
        /// </summary>
        /// <returns>The Shipping Options item from Sitecore.</returns>
        protected virtual Item GetShippingOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Commerce Control Panel/Shared Settings/Fulfillment Options");
        }

        /// <summary>
        /// Gets the shipping options type folder item.
        /// </summary>
        /// <returns>The shipping options type folder item.</returns>
        protected virtual Item GetShippingOptionsTypeFolder()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Commerce Control Panel/Shared Settings/Fulfillment Option Types");
        }
    }
}