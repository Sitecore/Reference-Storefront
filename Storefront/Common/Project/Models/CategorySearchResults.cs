//-----------------------------------------------------------------------
// <copyright file="CategorySearchResults.cs" company="Sitecore Corporation">
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
    using System.Collections.Generic;

    using Sitecore.ContentSearch.Linq;
    using Sitecore.Data.Items;

    /// <summary>
    /// Used to represent a category search result item
    /// </summary>
    public class CategorySearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySearchResults" /> class
        /// </summary>
        /// <param name="categoryItems">The products to init the item with</param>
        /// <param name="totalCategoryCount">The total number of categories</param>
        /// <param name="totalPageCount">The total number of pages</param>
        /// <param name="currentPageNumber">The current page number</param>
        /// <param name="facets">The facets for the collection of categories</param>
        public CategorySearchResults(List<Item> categoryItems, int totalCategoryCount, int totalPageCount, int currentPageNumber, List<FacetCategory> facets)
        {
            this.CategoryItems = categoryItems;
            this.TotalPageCount = totalPageCount;
            this.TotalCategoryCount = totalCategoryCount;
            this.Facets = facets;
            this.CurrentPageNumber = currentPageNumber;
        }

        /// <summary>
        /// Gets the list of items the results are based on
        /// </summary>
        public List<Item> CategoryItems { get; private set; }

        /// <summary>
        /// Gets the total number of categories
        /// </summary>
        public int TotalCategoryCount { get; private set; }

        /// <summary>
        /// Gets the total page count
        /// </summary>
        public int TotalPageCount { get; private set; }

        /// <summary>
        /// Gets the collection of facets for the collection of categories
        /// </summary>        
        public List<FacetCategory> Facets { get; private set; }

        /// <summary>
        /// Gets the current page number
        /// </summary>
        public int CurrentPageNumber { get; private set; }
    }
}