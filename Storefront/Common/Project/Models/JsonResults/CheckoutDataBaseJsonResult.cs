//-----------------------------------------------------------------------
// <copyright file="CheckoutDataBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CheckoutDataBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services;

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
        public IEnumerable<ShippingOption> OrderShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets the line item shipping options.
        /// </summary>
        public IEnumerable<LineShippingOption> LineShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets the ID of the 'email' delivery method.
        /// </summary>
        /// <value>
        /// The email delivery method.
        /// </value>
        public ShippingMethod EmailDeliveryMethod { get; set; }

        /// <summary>
        /// Gets or sets the ID of the 'ship to store' delivery method.
        /// </summary>
        /// <value>
        /// The ship to store delivery method.
        /// </value>
        public ShippingMethod ShipToStoreDeliveryMethod { get; set; }
        
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
    }
}