//-----------------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the Sitecore Item extension methods.</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the ItemExtensions class.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Items the type.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item type of Unknown if the item is null or unknown.></returns>
        public static StorefrontConstants.ItemTypes ItemType(this Item item)
        {
            if (item != null)
            {
                var templateList = TemplateManager.GetTemplate(item).GetBaseTemplates();

                if (templateList.FirstOrDefault(t => t.ID == CommerceConstants.KnownTemplateIds.CommerceProductTemplate) != null)
                {
                    return StorefrontConstants.ItemTypes.Product;
                }

                if (templateList.FirstOrDefault(t => t.ID == CommerceConstants.KnownTemplateIds.CommerceCategoryTemplate) != null)
                {
                    return StorefrontConstants.ItemTypes.Category;
                }

                if (templateList.FirstOrDefault(t => t.ID == CommerceConstants.KnownTemplateIds.CommerceProductVariantTemplate) != null)
                {
                    return StorefrontConstants.ItemTypes.Variant;
                }

                if (templateList.FirstOrDefault(t => t.ID == StorefrontConstants.KnownTemplateItemIds.SecuredPage) != null)
                {
                    return StorefrontConstants.ItemTypes.SecuredPage;
                }

                if (templateList.FirstOrDefault(t => t.ID == StorefrontConstants.KnownTemplateItemIds.StandardPage) != null)
                {
                    return StorefrontConstants.ItemTypes.StandardPage;
                }

                if (item.TemplateID == StorefrontConstants.KnownTemplateItemIds.NamedSearch)
                {
                    return StorefrontConstants.ItemTypes.NamedSearch;
                }

                if (item.TemplateID == StorefrontConstants.KnownTemplateItemIds.SelectedProducts)
                {
                    return StorefrontConstants.ItemTypes.SelectedProducts;
                }
            }

            return StorefrontConstants.ItemTypes.Unknown;
        }
    }
}
