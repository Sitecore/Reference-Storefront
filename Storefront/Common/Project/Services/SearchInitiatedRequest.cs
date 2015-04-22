//---------------------------------------------------------------------
// <copyright file="SearchInitiatedRequest.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Request used to track storefront search requests.</summary>
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
    using Sitecore.Diagnostics;

    /// <summary>
    /// Defines the SearchInitiatedRequest class.
    /// </summary>
    public class SearchInitiatedRequest : CatalogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchInitiatedRequest"/> class.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="numberOfHits">The number of hits.</param>
        public SearchInitiatedRequest([NotNull] string shopName, [NotNull] string searchTerm, int numberOfHits)
        {
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");
            Assert.ArgumentNotNullOrEmpty(searchTerm, "searchTerm");

            this.ShopName = shopName;
            this.SearchTerm = searchTerm;
            this.NumberOfHits = numberOfHits;
        }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>
        /// The name of the shop.
        /// </value>
        public string ShopName { get; set; }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        /// <value>
        /// The search term.
        /// </value>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the number of hits.
        /// </summary>
        /// <value>
        /// The number of hits.
        /// </value>
        public int NumberOfHits { get; set; }
    }
}
