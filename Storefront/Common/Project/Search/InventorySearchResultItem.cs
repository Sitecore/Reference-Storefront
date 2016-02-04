//-----------------------------------------------------------------------
// <copyright file="InventorySearchResultItem.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
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

namespace Sitecore.Reference.Storefront.Search
{
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.ContentSearch;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the InventorySearchResultItem class.
    /// </summary>
    public class InventorySearchResultItem : CommerceProductSearchResultItem
    {
        /// <summary>
        /// Gets or sets the out of stock locations.
        /// </summary>
        /// <value>
        /// The out of stock locations.
        /// </value>
        [IndexField("outofstocklocations")]
        public string OutOfStockLocations { get; set; }

        /// <summary>
        /// Gets or sets the orderable locations.
        /// </summary>
        /// <value>
        /// The orderable locations.
        /// </value>
        [IndexField("orderablelocations")]
        public string OrderableLocations { get; set; }

        /// <summary>
        /// Gets or sets the pre orderable.
        /// </summary>
        /// <value>
        /// The pre orderable.
        /// </value>
        [IndexField("preorderable")]
        public string PreOrderable { get; set; }

        /// <summary>
        /// Gets or sets the in stock locations.
        /// </summary>
        /// <value>
        /// The in stock locations.
        /// </value>
        [IndexField("instocklocations")]
        public string InStockLocations { get; set; }
    }
}
