//-----------------------------------------------------------------------
// <copyright file="ProductFacetsViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the ProductFacetsViewModel class.</summary>
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Used to represent a product facet list
    /// </summary>
    public class ProductFacetsViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFacetsViewModel"/> class.
        /// </summary>
        public ProductFacetsViewModel()
        {
            this.ChildProductFacets = new List<CommerceQueryFacet>();
            this.ActiveFacets = new List<CommerceQueryFacet>();
        }

        /// <summary>
        /// Gets or sets the list of product facets
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public IEnumerable<CommerceQueryFacet> ChildProductFacets
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the list of active facets
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public IEnumerable<CommerceQueryFacet> ActiveFacets
        {
            get;
            protected set;
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        /// <param name="rendering">The rendering</param>
        /// <param name="products">The list of child products</param>
        /// <param name="searchOptions">Any search options used to find products in this category</param>
        public void Initialize(Rendering rendering, SearchResults products, CommerceSearchOptions searchOptions)
        {
            base.Initialize(rendering);

            if (products != null)
            {
                this.ChildProductFacets = products.Facets;
            }

            this.ActiveFacets = searchOptions.FacetFields;
        }
    }
}