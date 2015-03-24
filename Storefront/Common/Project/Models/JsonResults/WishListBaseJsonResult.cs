//-----------------------------------------------------------------------
// <copyright file="WishListBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the WishListsBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Services;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for wish list operations.
    /// </summary>
    public class WishListBaseJsonResult : BaseJsonResult
    {
        private readonly List<WishListItemBaseJsonResult> _lines = new List<WishListItemBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListBaseJsonResult"/> class.
        /// </summary>
        public WishListBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public WishListBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }        

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is favorite.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is favorite; otherwise, <c>false</c>.
        /// </value>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets the wish list lines.
        /// </summary>
        /// <value>
        /// The lines.
        /// </value>
        public List<WishListItemBaseJsonResult> Lines
        {
            get { return this._lines; }
        }

        /// <summary>
        /// Initializes the specified wish list.
        /// </summary>
        /// <param name="wishList">The wish list.</param>
        public virtual void Initialize(WishList wishList)
        {
            if (wishList == null)
            {
                return;
            }

            this.Name = wishList.Name;
            this.IsFavorite = wishList.IsFavorite;
            this.ExternalId = wishList.ExternalId;

            foreach (var line in wishList.Lines)
            {
                this._lines.Add(new WishListItemBaseJsonResult(line, wishList.ExternalId));
            }
        }
    }
}