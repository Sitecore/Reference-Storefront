//-----------------------------------------------------------------------
// <copyright file="Category.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the Category class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.SitecoreItemModels
{
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using System.Collections.Generic;
    using Sitecore.Data.Items;

    /// <summary>
    /// Category class
    /// </summary>
    public class Category : SitecoreItemBase
    {
        private int _itemsPerPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Category(Item item)
        {
            this.InnerItem = item;
        }

        /// <summary>
        /// Gets the Name of the Item
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return this.InnerItem.Name; }
        }

        /// <summary>
        /// Gets the DisplayName of the Item
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName 
        { 
            get 
            { 
                return this.InnerItem.DisplayName;  
            } 
        }

        /// <summary>
        /// Gets or sets the Required Facets
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<CommerceQueryFacet> RequiredFacets { get; set; }

        /// <summary>
        /// Gets or sets the Sort Fields
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<CommerceQuerySort> SortFields { get; set; }

        /// <summary>
        /// Gets or sets the Items per page
        /// </summary>
        public int ItemsPerPage
        {
            get
            {
                return (_itemsPerPage == 0) ? StorefrontConstants.Settings.DefaultItemsPerPage : _itemsPerPage;
            }

            set 
            { 
                _itemsPerPage = value; 
            }
        }

        /// <summary>
        /// Label for the Category field
        /// </summary>
        /// <returns>The name title.</returns>
        public string NameTitle()
        {
            return this.InnerItem["Name Title"];
        }

        /// <summary>
        /// The Title of the Category Page
        /// </summary>
        /// <returns>The title.</returns>
        public string Title()
        {
            return this.InnerItem[StorefrontConstants.ItemFields.Title];
        }
    }
}