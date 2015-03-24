//-----------------------------------------------------------------------
// <copyright file="WishListsBaseJsonResult.cs" company="Sitecore Corporation">
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
    /// Json result for wish lists operations.
    /// </summary>
    public class WishListsBaseJsonResult : BaseJsonResult
    {
        private List<WishListHeaderItemBaseJsonResult> _wishLists = new List<WishListHeaderItemBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListsBaseJsonResult"/> class.
        /// </summary>
        public WishListsBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListsBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public WishListsBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }    

        /// <summary>
        /// Gets the wish lists.
        /// </summary>
        public List<WishListHeaderItemBaseJsonResult> WishLists 
        { 
            get 
            { 
                return this._wishLists; 
            } 
        }

        /// <summary>
        /// Initializes the specified wish lists.
        /// </summary>
        /// <param name="wishLists">The wish lists.</param>
        public virtual void Initialize(IEnumerable<WishListHeader> wishLists)
        {
            if (wishLists == null)
            {
                return;
            }

            foreach (var wishList in wishLists)
            {
                this._wishLists.Add(new WishListHeaderItemBaseJsonResult(wishList));
            }
        }
    }
}