//---------------------------------------------------------------------
// <copyright file="PriceSearchResultItem.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Price specific search result.</summary>
//---------------------------------------------------------------------
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
    /// Defines the PriceSearchResultItem class.
    /// </summary>
    public class PriceSearchResultItem : CommerceProductSearchResultItem
    {
        /// <summary>
        /// Gets or sets the base price.
        /// </summary>
        /// <value>
        /// The base price.
        /// </value>
        [IndexField("baseprice")]
        public double BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the adjusted price.
        /// </summary>
        /// <value>
        /// The adjusted price.
        /// </value>
        [IndexField("adjustedprice")]
        public double AdjustedPrice { get; set; }

        /// <summary>
        /// Gets or sets the variant information.
        /// </summary>
        /// <value>
        /// The variant information.
        /// </value>
        [IndexField("variantinfo")]
        public string VariantInfo { get; set; }

        /// <summary>
        /// Gets or sets the other fields.
        /// </summary>
        /// <value>
        /// The other fields.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public System.Collections.Generic.Dictionary<string, object> OtherFields { get; set; }
    }
}
