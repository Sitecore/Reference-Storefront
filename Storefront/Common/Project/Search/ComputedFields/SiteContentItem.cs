// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteContentItem.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>
//   Contains the SiteContentItem computed field class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.Search.ComputedFields
{
    using Sitecore.Commerce.Search.ComputedFields;
    using Sitecore.ContentSearch;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Diagnostics;
    using System.Linq;

    /// <summary>
    /// SiteContentItem computed field
    /// </summary>
    public class SiteContentItem : BaseComputedField
    {
        /// <summary>
        /// Computes the value of an indexable item to build the desired value
        /// </summary>
        /// <param name="itemToIndex">The indexable item</param>
        /// <returns>The computed value</returns>        
        public override object ComputeValue(IIndexable itemToIndex)
        {
            Assert.ArgumentNotNull(itemToIndex, "itemToIndex");

            var item = (Item)(itemToIndex as SitecoreIndexableItem);
            if (item != null)
            {
                var template = TemplateManager.GetTemplate(item);
                if (template != null && template.DescendsFromOrEquals(StorefrontConstants.KnownTemplateItemIds.StandardPage))
                {
                    return (item[StorefrontConstants.ItemFields.DisplayInSearchResults] == "1");
                }
            }

            return false;
        }
    }
}
