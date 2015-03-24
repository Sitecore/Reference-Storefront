//-----------------------------------------------------------------------
// <copyright file="CategoryViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CategoryViewModel class.</summary>
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
    using System.Web;
    using System.Web.Mvc;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Mvc;
    using Sitecore.Mvc.Common;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Reference.Storefront.Managers;

    /// <summary>
    /// Used to represent a category
    /// </summary>
    public class CategoryViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        private readonly Item _item;
        private List<MediaItem> _images;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryViewModel" /> class
        /// </summary>
        public CategoryViewModel()
        {
            ChildProducts = new List<ProductViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryViewModel" /> class
        /// </summary>
        /// <param name="item">The item to initialize the class with</param>
        public CategoryViewModel(Item item)
        {
            ChildProducts = new List<ProductViewModel>();
            _item = item;
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
        /// Gets or sets the relationship name, if this view represents items in a catalog relationship.
        /// </summary>
        public string RelationshipName { get; set; }

        /// <summary>
        /// Gets or sets the relationship description, if this view represents items in a catalog relationship.
        /// </summary>
        public string RelationshipDescription { get; set; }

        /// <summary>
        /// Gets the Product DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Item.DisplayName;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get
            {
                return Item["Description"];
            }
        }

        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId
        {
            get
            {
                return Item["ExternalId"];
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return Item.Name;
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
        /// Gets the description as a html string
        /// </summary>
        public List<MediaItem> Images
        {
            get
            {
                if (_images != null)
                {
                    return _images;
                }

                _images = new List<MediaItem>();

                MultilistField field = Item.Fields["Images"];

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
        public List<ProductViewModel> ChildProducts { get; set; }

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
        /// <param name="sortFields">The fields to allow sorting on</param>
        /// <param name="searchOptions">Any search options used to find products in this category</param>
        public void Initialize(Rendering rendering, SearchResults products, IEnumerable<CommerceQuerySort> sortFields, CommerceSearchOptions searchOptions)
        {
            base.Initialize(rendering);

            int itemsPerPage = (searchOptions != null) ? searchOptions.NumberOfItemsToReturn : 0;

            if (products != null)
            {
                ChildProducts = new List<ProductViewModel>();
                foreach (var child in products.SearchResultItems)
                {
                    var productModel = new ProductViewModel(child);
                    productModel.Initialize(this.Rendering);
                    this.ChildProducts.Add(productModel);
                }

                ChildProductFacets = products.Facets;
                if (itemsPerPage > products.SearchResultItems.Count)
                {
                    itemsPerPage = products.SearchResultItems.Count;
                }

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

            SortFields = sortFields;
        }

        /// <summary>
        /// Gets a property category link
        /// </summary>
        /// <returns>A Category link</returns>
        public string GetLink()
        {
            return Item.Name.Equals(StorefrontManager.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase)
              ? StorefrontManager.StorefrontUri("/buygiftcard")
              : Links.LinkManager.GetDynamicUrl(Item).TrimEnd('/');
        }
    }
}