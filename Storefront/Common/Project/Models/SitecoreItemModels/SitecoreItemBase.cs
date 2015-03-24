//-----------------------------------------------------------------------
// <copyright file="SitecoreItemBase.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SitecoreItemBase class.</summary>
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
    using System.Web;
    using System.Linq;
    using Sitecore.Data.Items;

    /// <summary>
    /// Base class for Sitecore Item wrappers
    /// </summary>
    public class SitecoreItemBase
    {
        /// <summary>
        /// The _item
        /// </summary>
        private Item _item;

        /// <summary>
        /// Gets or sets the inner item.
        /// </summary>
        /// <value>
        /// The inner item.
        /// </value>
        public Item InnerItem 
        { 
            get 
            {
                return this._item; 
            } 

            set
            {
                this._item = value;
            }
        }

        /// <summary>
        /// Gets the Id for this Item
        /// </summary>
        public string Id
        {
            get { return this._item.ID.ToShortID().ToString(); }
        }

        /// <summary>
        /// Gets the field with default.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The field value or the defaultValue if the item is null.</returns>
        public string GetFieldWithDefault(string fieldName, string defaultValue)
        {
            return this._item == null ? defaultValue : this._item[fieldName];
        }
    }
}