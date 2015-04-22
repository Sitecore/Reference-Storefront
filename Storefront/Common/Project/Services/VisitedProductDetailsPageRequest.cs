//---------------------------------------------------------------------
// <copyright file="VisitedProductDetailsPageRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the VisitedProductDetailsPageRequest class.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.Services
{
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The request parameters required to trigger the page event to track visits to the product details page.
    /// </summary>
    public class VisitedProductDetailsPageRequest : CatalogRequest
    {
        private string _productId;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitedProductDetailsPageRequest" /> class
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="productId">The product ID.</param>
        public VisitedProductDetailsPageRequest([NotNull] string shopName, [NotNull] string productId)
        {
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");
            Assert.ArgumentNotNullOrEmpty(productId, "productId");

            this.ShopName = shopName;
            this.ProductId = productId;
        }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>
        /// The name of the shop.
        /// </value>
        public string ShopName { get; set; }

        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        [NotNull]
        public string ProductId
        {
            get
            {
                return this._productId;
            }

            set
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(value, "value");
                this._productId = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent category name.
        /// </summary>
        public string ParentCategoryName { get; set; }

        /// <summary>
        /// Gets or sets the catalog name.
        /// </summary>
        public string CatalogName { get; set; }
    }
}