//-----------------------------------------------------------------------
// <copyright file="CheckoutDataBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CheckoutDataBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Connect.CommerceServer;
    using System.Linq;
    using System;

    /// <summary>
    /// The Json result of a request to retrieve the available states..
    /// </summary>
    public class CheckoutDataBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutDataBaseJsonResult"/> class.
        /// </summary>
        public CheckoutDataBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutDataBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public CheckoutDataBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the order shipping options.
        /// </summary>
        public IEnumerable<ShippingOptionBaseJsonResult> OrderShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets the line item shipping options.
        /// </summary>
        public IEnumerable<LineShippingOptionBaseJsonResult> LineShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets the ID of the 'email' delivery method.
        /// </summary>
        /// <value>
        /// The email delivery method.
        /// </value>
        public ShippingMethodBaseJsonResult EmailDeliveryMethod { get; set; }

        /// <summary>
        /// Gets or sets the ID of the 'ship to store' delivery method.
        /// </summary>
        /// <value>
        /// The ship to store delivery method.
        /// </value>
        public ShippingMethodBaseJsonResult ShipToStoreDeliveryMethod { get; set; }
        
        /// <summary>
        /// Gets or sets the countries that items can be shipped to.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public IDictionary<string, string> Countries { get; set; }

        /// <summary>
        /// Gets or sets the available payment options.
        /// </summary>
        public IEnumerable<PaymentOption> PaymentOptions { get; set; }

        /// <summary>
        /// Gets or sets the available payment methods.
        /// </summary>
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets the payment client token.
        /// </summary>       
        public string PaymentClientToken { get; set; }

        /// <summary>
        /// Gets or sets the available shipping methods.
        /// </summary>
        public IEnumerable<ShippingMethod> ShippingMethods { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is authenticated.
        /// </summary>
        public bool IsUserAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the addresses saved in the user's profile.
        /// </summary>
        public AddressListItemBaseJsonResult UserAddresses { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the ID of the loyalty card associated with the cart.
        /// </summary>
        public string CartLoyaltyCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>The currency code.</value>
        public string CurrencyCode { get; set; }
        
        /// <summary>
        /// Gets or sets the cart.
        /// </summary>
        /// <value>The cart.</value>
        public CartBaseJsonResult Cart { get; set; }

        /// <summary>
        /// Initializes the shipping options.
        /// </summary>
        /// <param name="shippingOptions">The shipping options.</param>
        public virtual void InitializeShippingOptions(IEnumerable<ShippingOption> shippingOptions)
        {
            if (shippingOptions == null)
            {
                return;
            }

            var shippingOptionList = new List<ShippingOptionBaseJsonResult>();

            foreach (var shippingOption in shippingOptions)
            {
                var jsonResult = CommerceTypeLoader.CreateInstance<ShippingOptionBaseJsonResult>();

                jsonResult.Initialize(shippingOption);
                shippingOptionList.Add(jsonResult);
            }

            this.OrderShippingOptions = shippingOptionList;
        }

        /// <summary>
        /// Initializes the line item shipping options.
        /// </summary>
        /// <param name="lineItemShippingOptionList">The line item shipping option list.</param>
        public virtual void InitializeLineItemShippingOptions(IEnumerable<LineShippingOption> lineItemShippingOptionList)
        {
            if (lineItemShippingOptionList != null && lineItemShippingOptionList.Any())
            {
                var lineShippingOptionList = new List<LineShippingOptionBaseJsonResult>();

                foreach (var lineShippingOption in lineItemShippingOptionList)
                {
                    var jsonResult = CommerceTypeLoader.CreateInstance<LineShippingOptionBaseJsonResult>();

                    jsonResult.Initialize(lineShippingOption);
                    lineShippingOptionList.Add(jsonResult);
                }

                this.LineShippingOptions = lineShippingOptionList;

                foreach (var line in this.Cart.Lines)
                {
                    var lineShippingOption = lineItemShippingOptionList.FirstOrDefault(l => l.LineId.Equals(line.ExternalCartLineId, StringComparison.OrdinalIgnoreCase));
                    if (lineShippingOption != null)
                    {
                        line.SetShippingOptions(lineShippingOption.ShippingOptions);
                    }
                }
            }
        }
    }
}