//-----------------------------------------------------------------------
// <copyright file="SitecoreItemManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SitecoreItemManager class.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using System;
    using Sitecore.Data;
    using Sitecore.Data.Items;

    /// <summary>
    /// SitecoreItemManager class
    /// </summary>
    public class SitecoreItemManager
    {
        private static SitecoreItemManager _sitecoreItemManager;

        /// <summary>
        /// Instances this instance.
        /// </summary>
        /// <returns>Sitecore item manager</returns>
        public static SitecoreItemManager Instance()
        {
            return _sitecoreItemManager ?? (_sitecoreItemManager = new SitecoreItemManager());
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemIdOrPath">The item identifier or path.</param>
        /// <returns>The item</returns>
        public Item GetItem(string itemIdOrPath)
        {
            Guid itemGuid;
            return Guid.TryParse(itemIdOrPath, out itemGuid) ? Context.Database.GetItem(ID.Parse(itemGuid)) : Context.Database.GetItem(itemIdOrPath);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <typeparam name="T">Thje container type of the item.</typeparam>
        /// <param name="itemIdOrPath">The item identifier or path.</param>
        /// <returns>
        /// The item
        /// </returns>
        public T GetItem<T>(string itemIdOrPath)
        {
            Item item = this.GetItem(itemIdOrPath);
            var sitecoreItem = (T)Activator.CreateInstance(typeof(T), item);
            return sitecoreItem;
        }
    }
}