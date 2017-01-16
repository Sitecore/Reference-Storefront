//-----------------------------------------------------------------------
// <copyright file="GetPaymentMethods.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline responsible for returning the payment methods.</summary>
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

            Item paymentOptionTypesFolder = this.GetPaymentOptionsTypesFolder();

            string query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", paymentOptionTypesFolder.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.TypeId, request.PaymentOption.PaymentOptionType.Value);
            Item foundOptionType = paymentOptionTypesFolder.Database.SelectSingleItem(query);
            if (foundOptionType != null)
            {
                Item paymentOptionsItem = this.GetPaymentOptionsItem();

                query = string.Format(CultureInfo.InvariantCulture, "fast:{0}//*[@{1} = '{2}']", paymentOptionsItem.Paths.FullPath, CommerceServerStorefrontConstants.KnownFieldNames.PaymentOptionType, foundOptionType.ID);
                Item paymentOptionItem = paymentOptionsItem.Database.SelectSingleItem(query);
                if (paymentOptionItem != null)
                {
                    // Has methods?
                    if (paymentOptionItem.HasChildren)
                    {
                        List<PaymentMethod> returnList = new List<PaymentMethod>();

                        foreach (Item paymentMethodItem in paymentOptionItem.GetChildren())
                        {
                            // Do we have a Commerce Server Method?
                            if (paymentMethodItem.HasChildren)
                            {
                                Item csMethod = paymentMethodItem.GetChildren()[0];
                                string csMethodId = csMethod[StorefrontConstants.KnownFieldNames.MethodId];
                                Assert.IsNotNullOrEmpty(csMethodId, string.Format(CultureInfo.InvariantCulture, "The CS Method of the {0} Fulfillment Method is empty.", paymentMethodItem.Name));

                                PaymentMethod shippingMethod = this.EntityFactory.Create<PaymentMethod>("PaymentMethod");

                                this.TranslatePaymentMethod(paymentOptionItem, paymentMethodItem, csMethodId, shippingMethod);

                                returnList.Add(shippingMethod);
                            }
                        }

                        result.PaymentMethods = new System.Collections.ObjectModel.ReadOnlyCollection<PaymentMethod>(returnList);
                    }
                }
            }
        }

        /// <summary>
        /// Translates the payment method from an item to a PaymentMethod clas..
        /// </summary>
        /// <param name="paymentOptionItem">The payment option item.</param>
        /// <param name="paymentMethodItem">The payment method item.</param>
        /// <param name="csMethodId">The cs method identifier.</param>
        /// <param name="paymentMethod">The payment method.</param>
        protected virtual void TranslatePaymentMethod(Item paymentOptionItem, Item paymentMethodItem, string csMethodId, PaymentMethod paymentMethod)
        {
            Guid shippingMethodGuid = new Guid(csMethodId);

            paymentMethod.ExternalId = shippingMethodGuid.ToString("B");
            paymentMethod.Name = paymentMethodItem.Name;
            paymentMethod.Description = paymentMethodItem.Name;
            paymentMethod.PaymentOptionId = paymentOptionItem.ID.ToString();
        }

        /// <summary>
        /// Gets the payment option item.
        /// </summary>
        /// <returns>Get the Payment Options item from Sitecore.</returns>
        protected virtual Item GetPaymentOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Commerce Control Panel/Shared Settings/Payment Options");
        }

        /// <summary>
        /// Gets the payment options types folder item.
        /// </summary>
        /// <returns>The payment option types folder item.</returns>
        protected virtual Item GetPaymentOptionsTypesFolder()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Commerce Control Panel/Shared Settings/Payment Option Types");
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