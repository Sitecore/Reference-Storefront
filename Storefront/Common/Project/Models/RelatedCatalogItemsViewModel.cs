//-----------------------------------------------------------------------
// <copyright file="RelatedCatalogItemsViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the RelatedCatalogItemsViewModel class.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The model to display related catalog items.
    /// </summary>
    public class RelatedCatalogItemsViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedCatalogItemsViewModel" /> class.
        /// </summary>
        public RelatedCatalogItemsViewModel()
        {
            this.RelatedProducts = new List<CategoryViewModel>();
            this.RelatedCategories = new List<CategoryViewModel>();
        }

        /// <summary>
        /// Gets the list of related products.
        /// </summary>
        public List<CategoryViewModel> RelatedProducts { get; private set; }

        /// <summary>
        /// Gets the list of related categories.
        /// </summary>
        public List<CategoryViewModel> RelatedCategories { get; private set; }
    }
}