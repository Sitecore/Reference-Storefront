//-----------------------------------------------------------------------
// <copyright file="PromoList.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the PromoList class.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Data.Items;

    /// <summary>
    /// PromoList class
    /// </summary>
    public class PromoList : SitecoreItemBase
    {
        /// <summary>
        /// The promotions
        /// </summary>
        private List<CommercePromotion> _promotions = new List<CommercePromotion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoList"/> class.
        /// </summary>
        public PromoList()
        {
            //New instance.  _item will be null for non persisted instance
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromoList"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public PromoList(Item item)
        {
            this.InnerItem = item;
        }

        /// <summary>
        /// Gets the promotions.
        /// </summary>
        /// <value>
        /// The promotions.
        /// </value>
        public List<CommercePromotion> Promotions 
        { 
            get { return _promotions; } 
        }
    }
}