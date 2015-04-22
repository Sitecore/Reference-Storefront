//-----------------------------------------------------------------------
// <copyright file="CartBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CartBaseJsonResult class.</summary>
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
    using System.Linq;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Reference.Storefront.Extensions;

    /// <summary>
    /// Emits the Json result of a Cart request.
    /// </summary>
    public class CartBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartBaseJsonResult"/> class.
        /// </summary>
        public CartBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public CartBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the request is in preview mode.
        /// </summary>
        public bool IsPreview { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior.")]
        public List<CartLineBaseJsonResult> Lines { get; set; }

        /// <summary>
        /// Gets or sets the list of cart adjustments.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior.")]
        public List<CartAdjustmentBaseJsonResult> Adjustments { get; set; }

        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>
        /// The sub total.
        /// </value>
        public string Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        /// <value>
        /// The tax total.
        /// </value>
        public string TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>
        /// The total amount.
        /// </value>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount.
        /// </summary>
        /// <value>
        /// The discount.
        /// </value>
        public string Discount { get; set; }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        public string ShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets the promo codes.
        /// </summary>
        /// <value>
        /// The promo codes.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> PromoCodes { get; set; }

        /// <summary>
        /// Initializes this object based on the data contained in the provided cart.
        /// </summary>
        /// <param name="cart">The cart used to initialize this object.</param>
        public virtual void Initialize(Cart cart)
        {
            this.Lines = new List<CartLineBaseJsonResult>();
            this.Adjustments = new List<CartAdjustmentBaseJsonResult>();
            this.PromoCodes = new List<string>();

            if (cart == null)
            {
                return;
            }

            foreach (var line in (cart.Lines ?? Enumerable.Empty<CartLine>()))
            {
                var cartLine = CommerceTypeLoader.CreateInstance<CartLineBaseJsonResult>(line);
                this.Lines.Add(cartLine);
            }

            foreach (var adjustment in (cart.Adjustments ?? Enumerable.Empty<CartAdjustment>()))
            {
                this.Adjustments.Add(new CartAdjustmentBaseJsonResult(adjustment));
            }

            var commerceTotal = (CommerceTotal)cart.Total;
            this.Subtotal = commerceTotal.Subtotal.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.TaxTotal = cart.Total.TaxTotal.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.Total = cart.Total.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.TotalAmount = cart.Total.Amount;
            this.Discount = commerceTotal.OrderLevelDiscountAmount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.ShippingTotal = commerceTotal.ShippingTotal.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
        }
    }
}