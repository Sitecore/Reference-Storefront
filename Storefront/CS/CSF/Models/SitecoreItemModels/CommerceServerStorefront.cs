//-----------------------------------------------------------------------
// <copyright file="CommerceServerStorefront.cs" company="Sitecore Corporation">
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

namespace Sitecore.Reference.Storefront.Models.SitecoreItemModels
{
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The CommerceServerStorefront class.
    /// </summary>
    public class CommerceServerStorefront : CommerceStorefront
    {
        private Item _countryAndRegionItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceServerStorefront"/> class.
        /// </summary>
        public CommerceServerStorefront()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceServerStorefront"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CommerceServerStorefront(Item item)
            : base(item)
        {
        }

        /// <summary>
        /// Gets the country and region item.
        /// </summary>
        /// <value>
        /// The country and region item.
        /// </value>
        public Item CountryAndRegionItem
        {
            get
            {
                if (this._countryAndRegionItem == null)
                {
                    this._countryAndRegionItem = this.HomeItem.Database.GetItem(this.HomeItem[CommerceServerStorefrontConstants.KnownFieldNames.CountryLocationPath]);
                }

                return this._countryAndRegionItem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the storefront supports wishlists.
        /// </summary>
        /// <value>
        /// <c>true</c> if the storefront supports wishlists; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsWishLists
        {
            get
            {
                return MainUtil.GetBool(this.HomeItem[CommerceServerStorefrontConstants.KnownFieldNames.SupportsWishLists], false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether storefront supports loyalty programs.
        /// </summary>
        /// <value>
        /// <c>true</c> if the storefront supports loyalty programs; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsLoyaltyPrograms
        {
            get
            {
                return MainUtil.GetBool(this.HomeItem[CommerceServerStorefrontConstants.KnownFieldNames.SupportsLoyaltyProgram], false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the storefront supports gift card payment.
        /// </summary>
        /// <value>
        /// <c>true</c> if the storefront supports gift card payment; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsGiftCardPayment
        {
            get
            {
                return MainUtil.GetBool(this.HomeItem[CommerceServerStorefrontConstants.KnownFieldNames.SupportsGirstCardPayment], false);
            }
        }
    }
}