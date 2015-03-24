//-----------------------------------------------------------------------
// <copyright file="ProductViewModel.cs" company="Sitecore Corporation">
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Links;
    using Sitecore.Mvc;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Extensions;

    /// <summary>
    /// A view model for a Product.
    /// </summary>
    public class ProductViewModel : RenderingModel
    {
        private readonly Item _item;
        private List<MediaItem> _images;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel" /> class
        /// </summary>
        public ProductViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductViewModel" /> class
        /// </summary>
        /// <param name="item">The item to initialize the class with</param>
        public ProductViewModel(Item item)
        {
            _item = item;
        }

        /// <summary>
        /// Gets the item for the current model
        /// </summary>
        public override Item Item
        {
            get
            {
                return _item ?? base.Item;
            }
        }
        
        /// <summary>
        /// Gets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId
        {
            get
            {
                return Item.Name;
            }
        }

        /// <summary>
        /// Gets or sets the Product Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the description as a html string
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
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName 
        {
            get
            {
                var displayName = Item[FieldIDs.DisplayName.ToString()];
                return string.IsNullOrEmpty(displayName) ? string.Empty : displayName;
            }
        }

        /// <summary>
        /// Gets or sets the Product ListPrice.
        /// </summary>
        public decimal? ListPrice { get; set; }

        /// <summary>
        /// Gets the list price with currency.
        /// </summary>
        /// <value>
        /// The list price with currency.
        /// </value>
        public string ListPriceWithCurrency 
        {
            get
            {
                return this.ListPrice.HasValue ? this.ListPrice.ToCurrency() : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the customer average rating.
        /// </summary>        
        public decimal CustomerAverageRating { get; set; }      

        /// <summary>
        /// Gets the computed Star image to use given the rating of the product.  
        /// Based on naming convention (stars_sm_#) where # is the whole number of the rating (0-5)
        /// </summary>
        public string RatingStarImage
        {
            get
            {
                string starsImage = "stars_sm_0";
                decimal rating = CustomerAverageRating;
                if (rating > 0 && rating < 1)
                {
                    starsImage = "stars_sm_1";
                }
                else if (rating > 1 && rating < 2)
                {
                    starsImage = "stars_sm_1";
                }
                else if (rating > 2 && rating < 3)
                {
                    starsImage = "stars_sm_2";
                }
                else if (rating > 3 && rating < 4)
                {
                    starsImage = "stars_sm_3";
                }
                else if (rating > 4 && rating < 5)
                {
                    starsImage = "stars_sm_4";
                }
                else
                {
                    starsImage = "stars_sm_5";
                }

                return starsImage;
            }
        }

        /// <summary>
        /// Gets or sets the Product AdjustedPrice.
        /// </summary>
        public decimal? AdjustedPrice { get; set; }

        /// <summary>
        /// Gets the adjusted price with currency.
        /// </summary>
        /// <value>
        /// The adjusted price with currency.
        /// </value>
        public string AdjustedPriceWithCurrency
        {
            get
            {
                return this.AdjustedPrice.HasValue ? this.AdjustedPrice.ToCurrency() : string.Empty;
            }
        }

        /// <summary>
        /// Gets the percentage savings for the product.
        /// </summary>
        public decimal SavingsPercentage
        {
            get
            {
                if (!this.ListPrice.HasValue || !this.AdjustedPrice.HasValue || this.ListPrice.Value <= this.AdjustedPrice.Value)
                {
                    return 0;
                }

                var percentage = decimal.Floor(100 * (this.ListPrice.Value - this.AdjustedPrice.Value) / this.ListPrice.Value);
                var roundedPercentage = percentage + (5 - (percentage % 5));
                return roundedPercentage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is on sale.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is on sale; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnSale
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Item["OnSale"]))
                {
                    return Item["OnSale"] == "1";
                }

                return false;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is product.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is product; otherwise, <c>false</c>.
        /// </value>
        public bool IsProduct
        {
            get
            {
                var val = Item["IsProduct"];

                return (!string.IsNullOrEmpty(val) && val != "0");
            }
        }

        /// <summary>
        /// Gets the Catalog name.
        /// </summary>
        public string CatalogName
        {
            get
            {
                return Item["CatalogName"];
            }
        }

        /// <summary>
        /// Gets or sets the default Quantity to be used when creating a new LineItem.
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Gets or sets the gift card amount.
        /// </summary> 
        [Required]
        [Display(Name = "Gift Card Amount")]
        public decimal? GiftCardAmount { get; set; }

        /// <summary>
        /// Gets or sets the gift card amount options.
        /// </summary>       
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public List<KeyValuePair<string, decimal?>> GiftCardAmountOptions { get; set; }        

        /// <summary>
        /// Gets or sets the stock status.
        /// </summary>
        public StockStatus StockStatus { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the stock status.
        /// </summary>
        /// <value>
        /// The name of the stock status.
        /// </value>
        public string StockStatusName { get; set; }

        /// <summary>
        /// Gets or sets the stock count.
        /// </summary>
        public double StockCount { get; set; }

        /// <summary>
        /// Gets or sets the stock availability date.
        /// </summary>
        /// <value>
        /// The stock availability date.
        /// </value>
        public string StockAvailabilityDate { get; set; }

        /// <summary>
        /// Gets or sets the variants for the current product
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is the desired behavior")]
        public List<VariantViewModel> Variants
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the available colors of all the variants
        /// </summary>
        /// <value>
        /// The color of the variant product.
        /// </value>
        public List<string> VariantProductColor
        {
            get
            {
                return Variants.GroupBy(v => v.ProductColor).Select(grp => grp.First().ProductColor).ToList();
            }
        }

        /// <summary>
        /// Gets the available sizes of all the variants
        /// </summary>
        /// <value>
        /// The variant sizes.
        /// </value>
        public List<string> VariantSizes
        {
            get
            {
                var groups = Variants.GroupBy(v => v.Size);
                var sizes = groups.Select(grp => grp.First().Size).ToList();
                return sizes;
            }
        }

        /// <summary>
        /// Gets the product list display texts item
        /// </summary>
        /// <value>
        /// The product list texts.
        /// </value>
        public Item ProductListTexts
        {
            get
            {
                var home = Context.Database.GetItem(Context.Site.RootPath + Context.Site.StartItem);
                var textsItemPath = home["Product List Texts"];
                if (string.IsNullOrEmpty(textsItemPath))
                {
                    return null;
                }

                return Context.Database.GetItem(textsItemPath);
            }
        }

        /// <summary>
        /// Gets the add to cart link display text for the product summery
        /// </summary>
        /// <value>
        /// The add to cart link text.
        /// </value>
        public string AddToCartLinkText
        {
            get
            {
                var productListTexts = ProductListTexts;
                if (productListTexts != null)
                {
                    return productListTexts["Add To Cart Link Text"];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the product details link display text for the product summery
        /// </summary>
        /// <value>
        /// The product details link text.
        /// </value>
        public string ProductDetailsLinkText
        {
            get
            {
                var productListTexts = ProductListTexts;
                if (productListTexts != null)
                {
                    return productListTexts["Product Page Link Text"];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the product URL.
        /// </summary>
        /// <value>The product URL.</value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ProductUrl
        {
            get
            {
                return this.ProductId.Equals(StorefrontManager.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase)
             ? StorefrontManager.StorefrontUri("/buygiftcard")
             : LinkManager.GetDynamicUrl(Item);
            }
        }

        /// <summary>
        /// Initializes the model with the specified data
        /// </summary>
        /// <param name="rendering">The rendering associated to the current render</param>
        /// <param name="variants">The variants associated to the current product</param>
        public void Initialize(Rendering rendering, List<VariantViewModel> variants)
        {
            base.Initialize(rendering);
            Variants = variants;
        }

        /// <summary>
        /// General purpose field renderer
        /// </summary>
        /// <param name="fieldName">the field to render</param>
        /// <returns>The HtmlString based on Sitecore field rendering.</returns>
        public HtmlString RenderField(string fieldName)
        {
            return PageContext.Current.HtmlHelper.Sitecore().Field(fieldName, Item);
        }

        /// <summary>
        /// Get a proper product link
        /// </summary>
        /// <returns>The pruduct url.</returns>
        public string GetLink()
        {
            return this.ProductId.Equals(StorefrontManager.CurrentStorefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase)
             ? StorefrontManager.StorefrontUri("/buygiftcard")
             : LinkManager.GetDynamicUrl(Item);
        }
    }
}
