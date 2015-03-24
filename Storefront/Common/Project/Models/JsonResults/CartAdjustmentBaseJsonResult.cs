//-----------------------------------------------------------------------
// <copyright file="CartAdjustmentBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CartAdjustmentBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Services.Carts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sitecore.Reference.Storefront.Extensions;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Reference.Storefront.Managers;
    using System.Globalization;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Diagnostics;

    /// <summary>
    /// Emits the Json result of a cart adjustment request.
    /// </summary>
    public class CartAdjustmentBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartAdjustmentBaseJsonResult"/> class.
        /// </summary>
        /// <param name="adjustment">The cart adjustment.</param>
        public CartAdjustmentBaseJsonResult(CartAdjustment adjustment)
        {
            Assert.ArgumentNotNull(adjustment, "adjustment");
            this.Amount = adjustment.Amount.ToString("C", Sitecore.Context.Language.CultureInfo);
            this.Description = adjustment.Description;
            this.IsCharge = adjustment.IsCharge;
            this.LineNumber = adjustment.LineNumber;
            this.Percentage = adjustment.Percentage;
        }

        /// <summary>
        /// Gets or sets the adjustment amount.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the adjustment desccription.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the adjustment is a charge.
        /// </summary>
        public bool IsCharge { get; set; }

        /// <summary>
        /// Gets or sets the adjustment line number.
        /// </summary>
        public uint LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the adjustment percentage.
        /// </summary>
        public float Percentage { get; set; }
    }
}