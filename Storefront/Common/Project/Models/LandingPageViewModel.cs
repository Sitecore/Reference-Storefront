//-----------------------------------------------------------------------
// <copyright file="LandingPageViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LandingPageViewModel class.</summary>
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
    using System.Globalization;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using Sitecore.Mvc;
    using Sitecore.Mvc.Common;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    
    /// <summary>
    /// Used to represent a category
    /// </summary>
    public class LandingPageViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        private readonly Item _item;
        private List<MediaItem> _images;

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingPageViewModel" /> class.
        /// </summary>
        public LandingPageViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LandingPageViewModel"/> class.
        /// </summary>
        /// <param name="item">The item to initialize the class with</param>
        public LandingPageViewModel(Item item)
        {
            _item = item;
            ChildProducts = new List<Item>();
            ChildCategories = new List<Item>();
        }

        /// <summary>
        /// Gets the item for the current model
        /// </summary>
        public override Item Item
        {
            get
            {
                if (_item == null)
                {
                    return base.Item;
                }

                return _item;
            }
        }

        /// <summary>
        /// Gets the Product DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Item != null && Item["Heading"] != null)
                {
                    return Item["Heading"];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get
            {
                return Item["Description"];
            }

            set
            {
                Item["Description"] = value;
            }
        }

        /// <summary>
        /// Gets the description as an html string
        /// </summary>
        public HtmlString DescriptionRender
        {
            get
            {
                return PageContext.Current.HtmlHelper.Sitecore().Field("Description", Item);
            }
        }

        /// <summary>
        /// Gets the associated product ids.
        /// </summary>
        /// <value>
        /// The associated product ids.
        /// </value>
        public string AssociatedProductIds
        {
            get
            {
                return Item["AssociatedProducts"];
            }
        }

        /// <summary>
        /// Gets the description as a html string
        /// </summary>
        public List<MediaItem> HeroImages
        {
            get
            {
                if (_images != null)
                {
                    return _images;
                }

                _images = new List<MediaItem>();

                MultilistField field = Item.Fields["HeroImages"];

                if (field != null)
                {
                    foreach (var id in field.TargetIDs)
                    {
                        MediaItem mediaItem = Item.Database.GetItem(id);
                        _images.Add(mediaItem);
                    }
                }

                return _images;
            }
        }

        /// <summary>
        /// Gets or sets the list of facets
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public IEnumerable<CommerceQueryFacet> ChildProductFacets
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the list of sortable fields
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public IEnumerable<CommerceQuerySort> SortFields
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the list of child products
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public List<Item> ChildProducts
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the list of child categories
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public List<Item> ChildCategories
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the pagination details for this category
        /// </summary>
        public PaginationModel Pagination { get; set; }

        /// <summary>
        /// Gets the context for the current view
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        protected ViewContext CurrentViewContext
        {
            get
            {
                return ContextService.Get().GetCurrentOrDefault<ViewContext>();
            }
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        /// <param name="rendering">The rendering</param>
        /// <param name="products">The list of child products</param>
        /// <param name="childCategories">The list of child categories</param>
        /// <param name="sortFields">The fields to allow sorting on</param>
        /// <param name="searchOptions">Any search options used to find products in this category</param>
        public void Initialize(Rendering rendering, SearchResults products, CategorySearchResults childCategories, IEnumerable<CommerceQuerySort> sortFields, CommerceSearchOptions searchOptions)
        {
            base.Initialize(rendering);

            ChildProducts = products == null ? new List<Item>() : products.SearchResultItems;
        }
    }
}