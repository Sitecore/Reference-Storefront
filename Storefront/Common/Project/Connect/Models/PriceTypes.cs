//-----------------------------------------------------------------------
// <copyright file="PriceTypes.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the PriceTypes constants.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the PriceTypes class.
    /// </summary>
    public static class PriceTypes
    {
        /// <summary>
        /// The list PriceType.
        /// </summary>
        public const string List = "List";

        /// <summary>
        /// The adjusted PriceType.
        /// </summary>
        public const string Adjusted = "Adjusted";

        /// <summary>
        /// The lowest priced variant PriceType.
        /// </summary>
        public const string LowestPricedVariant = "LowestPricedVariant";

        /// <summary>
        /// The lowest priced variant list price PriceType.
        /// </summary>
        public const string LowestPricedVariantListPrice = "LowestPricedVariantListPrice";

        /// <summary>
        /// The highest priced variant PriceType.
        /// </summary>
        public const string HighestPricedVariant = "HighestPricedVariant";
    }
}