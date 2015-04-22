//-----------------------------------------------------------------------
// <copyright file="StockInfoBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the StockInfoBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Extensions;

    /// <summary>
    /// The Json result of a request to retrieve product stock information.
    /// </summary>
    public class StockInfoBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockInfoBaseJsonResult"/> class.
        /// </summary>
        public StockInfoBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockInfoBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public StockInfoBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

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
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the availability data.
        /// </summary>
        /// <value>
        /// The availability data.
        /// </value>
        public string AvailabilityDate { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public double Count { get; set; }

        /// <summary>
        /// Initializes the specified stock infos.
        /// </summary>
        /// <param name="stockInfo">The stock information.</param>
        public virtual void Initialize(StockInformation stockInfo)
        {
            Assert.ArgumentNotNull(stockInfo, "stockInfo");

            if (stockInfo == null || stockInfo.Status == null)
            {
                return;
            }

            this.ProductId = stockInfo.Product.ProductId;
            this.VariantId = string.IsNullOrEmpty(((CommerceInventoryProduct)stockInfo.Product).VariantId) ? string.Empty : ((CommerceInventoryProduct)stockInfo.Product).VariantId;
            this.Status = StorefrontManager.GetProductStockStatusName(stockInfo.Status);
            this.Count = stockInfo.Count < 0 ? 0 : stockInfo.Count;
            if (stockInfo.AvailabilityDate != null & stockInfo.AvailabilityDate.HasValue)
            {
                this.AvailabilityDate = stockInfo.AvailabilityDate.Value.ToDisplayedDate();
            }
        }
    }
}