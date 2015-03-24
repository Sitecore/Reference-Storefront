//-----------------------------------------------------------------------
// <copyright file="Catalog.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the Catalog class.</summary>
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

    /// <summary>
    /// CommercePromotion class
    /// </summary>
    public class Catalog : SitecoreItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Catalog"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Catalog(Item item)
        {
            this.InnerItem = item;
        }

        /// <summary>
        /// Gets the Name of the item
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return this.InnerItem.Name; }
        }

        /// <summary>
        /// The Title of the Create Wish List Page
        /// </summary>
        /// <returns>The title.</returns>
        public string Title()
        {
            return this.InnerItem[StorefrontConstants.ItemFields.Title];
        }

        /// <summary>
        /// Label for the Wish List Name field
        /// </summary>
        /// <returns>The name title.</returns>
        public string NameTitle()
        {
            return this.InnerItem["Name Title"];
        }
    }
}