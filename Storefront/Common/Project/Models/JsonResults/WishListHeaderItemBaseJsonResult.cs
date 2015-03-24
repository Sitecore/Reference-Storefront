//-----------------------------------------------------------------------
// <copyright file="WishListHeaderItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the WishListHeaderItemBaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Entities.WishLists;
    using Sitecore.Reference.Storefront.Managers;

    /// <summary>
    /// Json result for whish list header operations.
    /// </summary>
    public class WishListHeaderItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WishListHeaderItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="header">The wish list header.</param>
        public WishListHeaderItemBaseJsonResult(WishListHeader header)
        {
            this.ExternalId = header.ExternalId;
            this.Name = header.Name;
            this.IsFavorite = header.IsFavorite;
            this.DetailsUrl = string.Concat(StorefrontManager.StorefrontUri("/accountmanagement/mywishlist"), "?id=", header.ExternalId);
        }

        /// <summary>
        /// Gets or sets the external ID of the wish list header.
        /// </summary>
        public string ExternalId { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the wish list header.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the favorite wish list.
        /// </summary>
        public bool IsFavorite { get; protected set; }

        /// <summary>
        /// Gets or sets the details url of the wish list header.
        /// </summary>
        public string DetailsUrl { get; protected set; }
    }
}