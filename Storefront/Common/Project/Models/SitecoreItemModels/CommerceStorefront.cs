//-----------------------------------------------------------------------
// <copyright file="CommerceStorefront.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CommerceStorefront class.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Data.Items;
    using Sitecore.Mvc.Presentation;

    /// <summary>
    /// Defines the CommerceStorefront class
    /// </summary>
    public class CommerceStorefront : SitecoreItemBase
    {
        private string _shopName = "website";
        private string _defaultCartName = CommerceConstants.CartSettings.DefaultCartName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceStorefront"/> class.
        /// </summary>
        public CommerceStorefront()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceStorefront"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CommerceStorefront(Item item)
        {
            this.InnerItem = item;

            if (!string.IsNullOrWhiteSpace(Context.Site.Name))
            {
                this._shopName = Context.Site.Name;
            }
        }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        /// <value>
        /// The root item.
        /// </value>
        public virtual Item RootItem
        {
            get
            {
                return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath);
            }
        }

        /// <summary>
        /// Gets the home item.
        /// </summary>
        /// <value>
        /// The home item.
        /// </value>
        public virtual Item HomeItem
        {
            get
            {
                return this.InnerItem;
            }
        }

        /// <summary>
        /// Gets the global item.
        /// </summary>
        /// <value>
        /// The global item.
        /// </value>
        public virtual Item GlobalItem
        {
            get
            {
                return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath + "/Global");
            }
        }

        /// <summary>
        /// Gets the sender email address.
        /// </summary>
        /// <value>
        /// The sender email address.
        /// </value>
        public virtual string SenderEmailAddress
        {
            get
            {
                var email = this.HomeItem.Fields[StorefrontConstants.KnownFieldNames.SenderEmailAddress];
                return email != null ? email.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>The name of the shop.</value>
        public virtual string ShopName
        {
            get { return _shopName; }
            set { _shopName = value; }
        }

        /// <summary>
        /// Gets or sets the default cart name
        /// </summary>
        /// <value>The default name of the cart.</value>
        public virtual string DefaultCartName
        {
            get { return _defaultCartName; }
            set { _defaultCartName = value; }
        }

        /// <summary>
        /// Gets the Default Catalog for this storefront
        /// </summary>
        public virtual Catalog DefaultCatalog
        {
            get
            {
                //If this is not in a storefront then return the default catalog
                if (this.InnerItem == null)
                {
                    var defaultCatalogContainer =
                        Context.Database.GetItem(CommerceConstants.KnownItemIds.DefaultCatalog);
                    var catalogPath = defaultCatalogContainer.Fields["Catalog"].Source + "/" + defaultCatalogContainer.Fields["Catalog"].Value;
                    var defaultCatalog = Context.Database.GetItem(catalogPath);
                    return new Catalog(defaultCatalog);
                }

                //Get list of all related catalogs
                var catalogs = this.InnerItem.Fields["Catalogs"].Value;
                var catalogArray = catalogs.Split("|".ToCharArray());
                if (catalogArray.Length == 0)
                {
                    return null;
                }

                //Return first catalog
                var catalogItem = Context.Database.GetItem(catalogArray[0]);
                return new Catalog(catalogItem);
            }
        }

        /// <summary>
        /// Gets the ProductId established for the Gift Card
        /// </summary>
        public virtual string GiftCardProductId
        {
            get
            {
                return this.InnerItem == null ? "22565422120" : this.InnerItem["GiftCardProductId"];
            }
        }

        /// <summary>
        /// Gets the gift card amount options.
        /// </summary>        
        public virtual string GiftCardAmountOptions
        {
            get
            {
                if (this.InnerItem == null)
                {
                    return "10|20|25|50|100";
                }

                return this.InnerItem["GiftCardAmountOptions"];
            }
        }

        /// <summary>
        /// Gets the default ProductId to use if no ProductId is presented
        /// </summary>
        public virtual string DefaultProductId
        {
            get
            {
                return this.InnerItem == null ? "22565422120" : this.InnerItem["DefaultProductId"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the storefront supports wishlists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the storefront supports wishlists; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsWishLists
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether storefront supports loyalty programs.
        /// </summary>
        /// <value>
        /// <c>true</c> if the storefront supports loyalty programs; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsLoyaltyPrograms
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the storefront supports gift card payment.
        /// </summary>
        /// <value>
        /// <c>true</c> if the storefront supports gift card payment; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsGiftCardPayment
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the maximum number of addresses.
        /// </summary>
        /// <value>
        /// The maximum number of addresses.
        /// </value>
        public virtual int MaxNumberOfAddresses
        {
            get
            {
                return MainUtil.GetInt(this.HomeItem[StorefrontConstants.KnownFieldNames.MaxNumberOfAddresses], 10);
            }
        }

        /// <summary>
        /// Gets the maximum number of wish lists.
        /// </summary>
        /// <value>
        /// The maximum number of wish lists.
        /// </value>
        public virtual int MaxNumberOfWishLists
        {
            get
            {
                return MainUtil.GetInt(this.HomeItem[StorefrontConstants.KnownFieldNames.MaxNumberOfWishLists], 10);
            }
        }

        /// <summary>
        /// Gets the maximum number of wish list items.
        /// </summary>
        /// <value>
        /// The maximum number of wish list items.
        /// </value>
        public virtual int MaxNumberOfWishListItems
        {
            get
            {
                return MainUtil.GetInt(this.HomeItem[StorefrontConstants.KnownFieldNames.MaxNumberOfWishListItems], 10);
            }
        }

        /// <summary>
        /// The Title of the Page
        /// </summary>
        /// <returns>The title.</returns>
        public virtual string Title()
        {
            return this.InnerItem == null ? "default" : this.InnerItem[StorefrontConstants.ItemFields.Title];
        }

        /// <summary>
        /// Returns the "Name Title" item property.
        /// </summary>
        /// <returns>The name title item property value.</returns>
        public virtual string NameTitle()
        {
            return this.InnerItem == null ? "default" : this.InnerItem["Name Title"];
        }
    }
}