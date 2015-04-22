//-----------------------------------------------------------------------
// <copyright file="WishListItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the WishListItemBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities.WishLists;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Reference.Storefront.Extensions;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Links;

    /// <summary>
    /// Json result for wish list header operations.
    /// </summary>
    public class WishListItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WishListItemBaseJsonResult" /> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        public WishListItemBaseJsonResult(WishListLine line, string wishListId)
        {
            Assert.ArgumentNotNull(line, "line");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var product = (CommerceCartProduct)line.Product;
            var productItem = Sitecore.Reference.Storefront.SitecorePipelines.ProductItemResolver.ResolveCatalogItem(product.ProductId, product.ProductCatalog, true);

            this.DisplayName = product.DisplayName;
            this.Color = product.Properties["Color"] as string;
            this.LineDiscount = ((CommerceTotal)line.Total).LineItemDiscountAmount.ToString(Sitecore.Context.Language.CultureInfo);
            this.Quantity = line.Quantity.ToString(Sitecore.Context.Language.CultureInfo);
            this.LineTotal = line.Total.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            this.ExternalLineId = line.ExternalId;
            this.ProductId = product.ProductId;
            this.VariantId = product.ProductVariantId;
            this.ProductCatalog = product.ProductCatalog;
            this.WishListId = wishListId;
            this.ProductUrl = product.ProductId.Equals(StorefrontManager.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase)
              ? StorefrontManager.StorefrontUri("/buygiftcard")
              : LinkManager.GetDynamicUrl(productItem);

            if (product.Price.Amount != 0M)
            {
                this.LinePrice = product.Price.Amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            }
            
            var imageInfo = product.Properties["_product_Images"] as string;
            if (imageInfo != null)
            {
                var imageId = imageInfo.Split('|')[0];
                MediaItem mediaItem = Sitecore.Context.Database.GetItem(imageId);
                this.Image = mediaItem.GetImageUrl(100, 100);
            }

            var giftCardAmount = line.GetPropertyValue("GiftCardAmount");
            if (giftCardAmount != null)
            {
                decimal amount = System.Convert.ToDecimal(giftCardAmount, Sitecore.Context.Language.CultureInfo);
                this.GiftCardAmount = amount.ToCurrency(StorefrontConstants.Settings.DefaultCurrencyCode);
            }
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
        public string ExternalLineId { get; set; }

        /// <summary>
        /// Gets or sets the product URL.
        /// </summary>
        /// <value>
        /// The product URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ProductUrl { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the variant identifier.
        /// </summary>
        /// <value>
        /// The variant identifier.
        /// </value>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the product catalog.
        /// </summary>
        /// <value>
        /// The product catalog.
        /// </value>
        public string ProductCatalog { get; set; }

        /// <summary>
        /// Gets or sets the gift card amount.
        /// </summary>       
        public string GiftCardAmount { get; set; }

        /// <summary>
        /// Gets or sets the wish list identifier.
        /// </summary>
        /// <value>The wish list identifier.</value>
        public string WishListId { get; set; }
    }
}