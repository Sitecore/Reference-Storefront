//-----------------------------------------------------------------------
// <copyright file="NavigationViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the NavigationViewModel class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.RenderingModels
{
    using Sitecore.Data.Items;
    using Sitecore.Mvc.Presentation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Used to represent the navigation
    /// </summary>
    public class NavigationViewModel : Sitecore.Mvc.Presentation.RenderingModel
    {
        /// <summary>
        /// Gets the list of child categories
        /// </summary>
        public List<Item> ChildCategories
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title of the store
        /// </summary>
        public string StoreTitle
        {
            get
            {
                var home = Context.Database.GetItem(Context.Site.RootPath + Context.Site.StartItem);
                return home[StorefrontConstants.ItemFields.Title];
            }
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        /// <param name="rendering">The rendering</param>
        /// <param name="childCategories">The list of child categories</param>
        public void Initialize(Rendering rendering, CategorySearchResults childCategories)
        {
            base.Initialize(rendering);

            if (childCategories != null)
            {
                ChildCategories = childCategories.CategoryItems;
            }
        }
    }
}