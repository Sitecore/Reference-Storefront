//-----------------------------------------------------------------------
// <copyright file="SiteContext.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SiteContext class.</summary>
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

namespace Sitecore.Reference.Storefront
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using System;
    using System.Collections;
    using System.Web;

    /// <summary>
    /// Provides contextual information about the current site instance
    /// </summary>
    public class SiteContext : ISiteContext
    {
        private const string CurrentCatalogItemKey = "_CurrentCatallogItem";
        private const string IsCategoryKey = "_IsCategory";
        private const string IsProductKey = "_IsProduct";

        /// <summary>
        /// Gets the current HTTP context.
        /// </summary>
        public virtual HttpContext CurrentContext
        {
            get { return HttpContext.Current; }
        }

        /// <summary>
        /// Gets the current HTTP context items collection.
        /// </summary>
        public virtual IDictionary Items
        {
            get { return HttpContext.Current.Items; }
        }

        /// <summary>
        /// Gets or sets the current catalog item.
        /// </summary>
        /// <value>
        /// The current catalog item.
        /// </value>
        public virtual Item CurrentCatalogItem 
        { 
            get
            {
                return this.Items[CurrentCatalogItemKey] as Item;
            }

            set
            {
                Item item = value as Item;

                this.Items[CurrentCatalogItemKey] = item;
                if (item != null)
                {
                    Template t = TemplateManager.GetTemplate(item.TemplateID, Sitecore.Context.Database);

                    this.Items[IsCategoryKey] = t.DescendsFrom(CommerceConstants.KnownTemplateIds.CommerceCategoryTemplate);
                    this.Items[IsProductKey] = t.DescendsFrom(CommerceConstants.KnownTemplateIds.CommerceProductTemplate);
                }
                else
                {
                    this.Items[IsCategoryKey] = false;
                    this.Items[IsProductKey] = false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the instance of the CurrentCatalogItem is a category.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is category; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsCategory
        {
            get
            {
                return (this.Items[IsCategoryKey] != null) ? (bool)this.Items[IsCategoryKey] : false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the instance of the CurrentCatalogItem is product.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is product; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsProduct
        {
            get
            {
                return (this.Items[IsProductKey] != null) ? (bool)this.Items[IsProductKey] : false;
            }
        }
    }
}