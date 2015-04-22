//-----------------------------------------------------------------------
// <copyright file="CartLineBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CartLineBaseJsonResult class.</summary>
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
    using System;
    using Sitecore.Reference.Storefront.Managers;
    using System.Diagnostics.CodeAnalysis;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Reference.Storefront.Extensions;
    using Sitecore.Reference.Storefront.SitecorePipelines;
    using Sitecore.Links;
    using System.Collections.Generic;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Reference.Storefront.Util;

    /// <summary>
    /// Emits the Json result of a cart line request.
    /// </summary>
    public class CartLineBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartLineBaseJsonResult"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        public CartLineBaseJsonResult(CustomCommerceCartLine line)
        {
            this.DiscountOfferNames = new List<string>();

            var product = (CommerceCartProduct)line.Product;
            var productItem = ProductItemResolver.ResolveCatalogItem(product.ProductId, product.ProductCatalog, true);

            if (line.Images.Count > 0)
            {
                this.Image = line.Images[0].GetImageUrl(100, 100);
            }

            this.DisplayName = product.DisplayName;
            this.Color = product.Properties["Color"] as string;
            this.LineDiscount = ((CommerceTotal)line.Total).LineItemDiscountAmount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.Quantity = line.Quantity.ToString(Context.Language.CultureInfo);
            this.LinePrice = product.Price.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.LineTotal = line.Total.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.ExternalCartLineId = StringUtility.RemoveCurlyBrackets(line.ExternalCartLineId);
            this.ProductUrl = product.ProductId.Equals(StorefrontManager.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase)
                ? StorefrontManager.StorefrontUri("/buygiftcard")
                : LinkManager.GetDynamicUrl(productItem);
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the line discount.
        /// </summary>
        /// <value>
        /// The line discount.
        /// </value>
        public string LineDiscount { get; set; }

        /// <summary>
        /// Gets or sets the discount offer names.
        /// </summary>
        /// <value>
        /// The discount offer names.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<string> DiscountOfferNames { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public string Quantity { get; set; }

        /// <summary>
        /// Gets or sets the line price.
        /// </summary>
        /// <value>
        /// The line price.
        /// </value>
        public string LinePrice { get; set; }

        /// <summary>
        /// Gets or sets the line total.
        /// </summary>
        /// <value>
        /// The line total.
        /// </value>
        public string LineTotal { get; set; }

        /// <summary>
        /// Gets or sets the external cart line identifier.
        /// </summary>
        /// <value>
        /// The external cart line identifier.
        /// </value>
        public string ExternalCartLineId { get; set; }

        /// <summary>
        /// Gets or sets the product URL.
        /// </summary>
        /// <value>
        /// The product URL.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ProductUrl { get; set; }

        /// <summary>
        /// Gets or sets the line shipping options.
        /// </summary>
        /// <value>The line shipping options.</value>
        public IEnumerable<ShippingOption> ShippingOptions { get; set; }
    }
}