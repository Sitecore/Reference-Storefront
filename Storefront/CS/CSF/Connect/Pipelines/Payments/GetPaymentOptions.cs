//-----------------------------------------------------------------------
// <copyright file="GetPaymentOptions.cs" company="Sitecore Corporation">
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

namespace Sitecore.Reference.Storefront.Connect.Pipelines.Payments
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Services;
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
    /// Defines the GetPaymentOptions class.
    /// </summary>
    public class GetPaymentOptions : CommercePipelineProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPaymentOptions"/> class.
        /// </summary>
        /// <param name="entityFactory">The entity factory.</param>
        public GetPaymentOptions([NotNull] IEntityFactory entityFactory)
        {
            Assert.IsNotNull(entityFactory, "entityFactory");

            this.EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets or sets the entity factory.
        /// </summary>
        /// <value>
        /// The entity factory.
        /// </value>
        public IEntityFactory EntityFactory { get; set; }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentCondition(args.Request is GetPaymentOptionsRequest, "args.Request", "args.Request is GetPaymentOptionsRequest");
            Assert.ArgumentCondition(args.Result is GetPaymentOptionsResult, "args.Result", "args.Result is GetPaymentOptionsResult");

            var request = (GetPaymentOptionsRequest)args.Request;
            var result = (GetPaymentOptionsResult)args.Result;

            List<PaymentOption> paymentOptionList = new List<PaymentOption>();

            foreach (Item paymentOptionItem in this.GetPaymentOptionsItem().GetChildren())
            {
                PaymentOption option = this.EntityFactory.Create<PaymentOption>("PaymentOption");

                this.TranslateToPaymentOption(paymentOptionItem, option, result);

                paymentOptionList.Add(option);
            }

            result.PaymentOptions = new System.Collections.ObjectModel.ReadOnlyCollection<PaymentOption>(paymentOptionList);
        }

        /// <summary>
        /// Translates to payment option.
        /// </summary>
        /// <param name="paymentOptionItem">The payment option item.</param>
        /// <param name="paymentOption">The payment option.</param>
        /// <param name="result">The result.</param>
        protected virtual void TranslateToPaymentOption(Item paymentOptionItem, PaymentOption paymentOption, GetPaymentOptionsResult result)
        {
            paymentOption.ExternalId = paymentOptionItem.ID.Guid.ToString();
            paymentOption.Name = paymentOptionItem.DisplayName;
            paymentOption.ShopName = this.GetShopName();

            int enumValue = MainUtil.GetInt(paymentOptionItem[CommerceServerStorefrontConstants.KnownFieldNames.PaymentOptionValue], 0);
            paymentOption.PaymentOptionType = this.TranslatePaymentOptionType(enumValue, result);
        }

        /// <summary>
        /// Gets the payment option item.
        /// </summary>
        /// <returns>The Payment Options node from Sitecore.</returns>
        protected virtual Item GetPaymentOptionsItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/Commerce/Payment Options");
        }

        /// <summary>
        /// Gets the name of the shop.
        /// </summary>
        /// <returns>The shop name.</returns>
        protected virtual string GetShopName()
        {
            // TODO: Shopname missing from the request.  Maybe OBEC should define it.
            return ((CommerceServerStorefront)StorefrontManager.CurrentStorefront).ShopName;
        }

        /// <summary>
        /// Translateoes the type of the payment option.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns>The full PaymentOptionType.</returns>
        protected virtual PaymentOptionType TranslatePaymentOptionType(int value, GetPaymentOptionsResult result)
        {
            PaymentOptionType returnValue;

            switch (value)
            {
                case 0:
                    returnValue = PaymentOptionType.None;
                    break;

                case 1:
                    returnValue = PaymentOptionType.PayCard;
                    break;

                case 2:
                    returnValue = PaymentOptionType.PayLoyaltyCard;
                    break;

                case 3:
                    returnValue = PaymentOptionType.PayGiftCard;
                    break;

                default:
                    returnValue = PaymentOptionType.None;
                    string errorMessage = String.Format(CultureInfo.InvariantCulture, "The given PaymentOptionType value is unsupported: {0}", value);
                    result.SystemMessages.Add(new SystemMessage() { Message = errorMessage });
                    result.Success = false;
                    CommerceLog.Current.Error(errorMessage, this);
                    break;
            }

            return returnValue;
        }
    }
}