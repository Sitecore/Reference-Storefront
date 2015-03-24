//-----------------------------------------------------------------------
// <copyright file="GetPaymentMethods.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Pipeline responsible for returning the payment methods.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Payments
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the GetPaymentMethods class.
    /// </summary>
    public class GetPaymentMethods : Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines.GetPaymentMethods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPaymentMethods"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetPaymentMethods([NotNull] IEntityFactory entityFactory) : base(entityFactory)
        {
        }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is CommerceGetPaymentMethodsRequest, "args.Request", "args.Request is CommerceGetPaymentMethodsRequest");
            Assert.ArgumentCondition(args.Result is GetPaymentMethodsResult, "args.Result", "args.Result is GetPaymentMethodsResult");

            var request = (CommerceGetPaymentMethodsRequest)args.Request;
            var result = (GetPaymentMethodsResult)args.Result;

            if (request.PaymentOption.PaymentOptionType == null)
            {
                base.Process(args);
                return;
            }

            Item paymentOptionsItem = this.GetPaymentOptionsItem();

            string query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", paymentOptionsItem.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.PaymentOptionValue, request.PaymentOption.PaymentOptionType.Value);
            Item foundOption = paymentOptionsItem.Database.SelectSingleItem(query);
            if (foundOption != null)
            {
                string paymentMethodsIds = foundOption[CommerceServerStorefrontConstants.KnownFieldNames.CommerceServerPaymentMethods];
                if (!string.IsNullOrWhiteSpace(paymentMethodsIds))
                {
                    base.Process(args);
                    if (result.Success)
                    {
                        List<PaymentMethod> currentList = new List<PaymentMethod>(result.PaymentMethods);
                        List<PaymentMethod> returnList = new List<PaymentMethod>();

                        string[] ids = paymentMethodsIds.Split('|');
                        foreach (string id in ids)
                        {
                            string trimmedId = id.Trim();

                            var found2 = currentList.Find(o => o.ExternalId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase));
                            PaymentMethod found = currentList.Find(o => o.ExternalId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase));
                            if (found != null)
                            {
                                returnList.Add(found);
                            }
                        }

                        result.PaymentMethods = new System.Collections.ObjectModel.ReadOnlyCollection<PaymentMethod>(returnList);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the payment option item.
        /// </summary>
        /// <returns>Get the Payment Options item from Sitecore.</returns>
        protected virtual Item GetPaymentOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Payment Options");
        }

        /// <summary>
        /// Gets the name of the shop.
        /// </summary>
        /// <returns>Get the shop name.</returns>
        protected virtual string GetShopName()
        {
            // TODO: Shopname missing from the request.  Maybe OBEC should define it.
            return ((CommerceServerStorefront)StorefrontManager.CurrentStorefront).ShopName;
        }
    }
}