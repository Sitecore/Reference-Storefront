//-----------------------------------------------------------------------
// <copyright file="ExtendedCommercePrice.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Extends the CS Connect CommercePrice.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Models
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the ExtendedCommercePrice class.
    /// </summary>
    public class ExtendedCommercePrice : CommercePrice
    {
        /// <summary>
        /// Gets or sets the lowest priced variant.
        /// </summary>
        /// <value>
        /// The lowest priced variant.
        /// </value>
        public decimal? LowestPricedVariant { get; set; }

        /// <summary>
        /// Gets or sets the lowest priced variant list price.
        /// </summary>
        /// <value>
        /// The lowest priced variant list price.
        /// </value>
        public decimal? LowestPricedVariantListPrice { get; set; }

        /// <summary>
        /// Gets or sets the highest priced variant.
        /// </summary>
        /// <value>
        /// The highest priced variant.
        /// </value>
        public decimal? HighestPricedVariant { get; set; }
    }
}
