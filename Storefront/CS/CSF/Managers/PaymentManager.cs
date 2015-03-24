//-----------------------------------------------------------------------
// <copyright file="PaymentManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the payment business logic for the site.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;

    /// <summary>
    /// Defines the PaymentManager class.
    /// </summary>
    public class PaymentManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentManager" /> class.
        /// </summary>
        /// <param name="paymentServiceProvider">The payment service provider.</param>
        /// <param name="cartManager">The cart manager.</param>
        public PaymentManager([NotNull] PaymentServiceProvider paymentServiceProvider, [NotNull] CartManager cartManager)
        {
            Assert.ArgumentNotNull(paymentServiceProvider, "paymentServiceProvider");

            this.PaymentServiceProvider = paymentServiceProvider;
            this.CartManager = cartManager;
        }

        /// <summary>
        /// Gets or sets the payment service provider.
        /// </summary>
        /// <value>
        /// The payment service provider.
        /// </value>
        public PaymentServiceProvider PaymentServiceProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the cart manager.
        /// </summary>
        /// <value>
        /// The cart manager.
        /// </value>
        public CartManager CartManager { get; protected set; }

        /// <summary>
        /// Gets the payment options.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <returns>
        /// The manager response where the payment option list is returned in the Result.
        /// </returns>
        public ManagerResponse<GetPaymentOptionsResult, IEnumerable<PaymentOption>> GetPaymentOptions([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");

            var result = new GetPaymentOptionsResult { Success = false };
            var cartResult = this.CartManager.GetCurrentCart(storefront, visitorContext);
            if (!cartResult.ServiceProviderResult.Success || cartResult.Result == null)
            {
                result.SystemMessages.ToList().AddRange(cartResult.ServiceProviderResult.SystemMessages);
                return new ManagerResponse<GetPaymentOptionsResult, IEnumerable<PaymentOption>>(result, null);
            }

            // TODO: CS Connect does not sport passing in a Cart.
            // var request = new GetPaymentOptionsRequest(storefront.ShopName, cart);
            var request = new GetPaymentOptionsRequest(storefront.ShopName);
            result = this.PaymentServiceProvider.GetPaymentOptions(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetPaymentOptionsResult, IEnumerable<PaymentOption>>(result, result.PaymentOptions.ToList());
        }

        /// <summary>
        /// Gets the payment methods.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="paymentOption">The payment option.</param>
        /// <returns>The manager response where the payment method list is returned in the Result.</returns>
        public ManagerResponse<GetPaymentMethodsResult, IEnumerable<PaymentMethod>> GetPaymentMethods([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, PaymentOption paymentOption)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(paymentOption, "paymentOption");

            // TODO: Remove hard coded language - will be fixed in connect.
            var request = new CommerceGetPaymentMethodsRequest("en-us", paymentOption);
            var result = this.PaymentServiceProvider.GetPaymentMethods(request);

            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<GetPaymentMethodsResult, IEnumerable<PaymentMethod>>(result, result.PaymentMethods.ToList());
        }
    }
}