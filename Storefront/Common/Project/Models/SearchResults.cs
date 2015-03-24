//-----------------------------------------------------------------------
// <copyright file="SearchResults.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
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

namespace Sitecore.Reference.Storefront.Models
{
    using Sitecore.Data.Items;
    using System.Collections.Generic;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using System.Linq;
    
    /// <summary>
    /// Used to represent a search result items
    /// </summary>
    public class SearchResults
    {
        private List<Item> _searchResultItems;
        private IEnumerable<CommerceQueryFacet> _facets;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults" /> class
        /// </summary>
        public SearchResults()
            : this(null, 0, 0, 0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults" /> class
        /// </summary>
        /// <param name="searchResultItems">The search result items.</param>
        /// <param name="totalItemCount">The total number of search result items.</param>
        /// <param name="totalPageCount">The total number of pages.</param>
        /// <param name="currentPageNumber">The current page number.</param>
        /// <param name="facets">The facets for the collection of search result items.</param>
        public SearchResults(List<Item> searchResultItems, int totalItemCount, int totalPageCount, int currentPageNumber, IEnumerable<CommerceQueryFacet> facets)
            : base()
        {
            this.SearchResultItems = searchResultItems ?? new List<Item>();
            this.TotalPageCount = totalPageCount;
            this.TotalItemCount = totalItemCount;
            this.Facets = facets ?? Enumerable.Empty<CommerceQueryFacet>();
            this.CurrentPageNumber = currentPageNumber;
        }

        /// <summary>
        /// Gets or sets the display name to show
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the items for the results
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Property setting is desired here.")]
        public List<Item> SearchResultItems
        {
            get
            {
                return this._searchResultItems;
            }

            set
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNull(value, "value");
                this._searchResultItems = value;
            }
        }

        /// <summary>
        /// Gets or sets the total item count
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Gets or sets the total page count
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// Gets or sets the collection of facets for the collection of search results
        /// </summary>
        public IEnumerable<CommerceQueryFacet> Facets
        {
            get
            {
                return this._facets;
            }

            set
            {
                Sitecore.Diagnostics.Assert.ArgumentNotNull(value, "value");
                this._facets = value;
            }
        }

        /// <summary>
        /// Gets or sets the current page number
        /// </summary>
        public int CurrentPageNumber { get; set; }
    }
}