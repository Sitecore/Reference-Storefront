//-----------------------------------------------------------------------
// <copyright file="PaginationViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the PaginationViewModel class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.RenderingModels
{
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Mvc.Presentation;
    using System;

    /// <summary>
    /// Used to represent a pagination
    /// </summary>
    public class PaginationViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        /// <summary>
        /// Gets or sets the pagination details for this category
        /// </summary>
        public PaginationModel Pagination { get; set; }

        /// <summary>
        /// Gets or sets the paging querystring token.
        /// </summary>
        public string QueryStringToken { get; set; }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        /// <param name="rendering">The rendering</param>
        /// <param name="products">The list of child products</param>
        /// <param name="searchOptions">Any search options used to find products in this category</param>
        public void Initialize(Rendering rendering, SearchResults products, CommerceSearchOptions searchOptions)
        {
            base.Initialize(rendering);
            this.QueryStringToken = StorefrontConstants.QueryStrings.Paging;

            int itemsPerPage = (searchOptions != null) ? searchOptions.NumberOfItemsToReturn : 20;

            if (products != null)
            {
                var alreadyShown = products.CurrentPageNumber * itemsPerPage;
                Pagination = new PaginationModel
                {
                    PageNumber = products.CurrentPageNumber,
                    TotalResultCount = products.TotalItemCount,
                    NumberOfPages = products.TotalPageCount,
                    PageResultCount = itemsPerPage,
                    StartResultIndex = alreadyShown + 1,
                    EndResultIndex = System.Math.Min(products.TotalItemCount, alreadyShown + itemsPerPage)
                };
            }
        }
    }
}