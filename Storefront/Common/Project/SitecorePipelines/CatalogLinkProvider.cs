//---------------------------------------------------------------------
// <copyright file="CatalogLinkProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The catelog link provider</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Links;

    /// <summary>
    /// Class to help build dynamic urls
    /// </summary>
    public class CatalogLinkProvider : LinkProvider
    {
        /// <summary>
        /// The attribute name of the includeCatalogs setting.
        /// </summary>
        public const string IncludeCatalogsAttribute = "includeCatalog";

        /// <summary>
        /// The attribute name of the useShopLinks setting.
        /// </summary>
        public const string UseShopLinksAttribute = "useShopLinks";

        /// <summary>
        /// The attribute name of the includeFriendlyName setting.
        /// </summary>
        public const string IncludeFriendlyNameAttribute = "includeFriendlyName";

        /// <summary>
        /// The default value for the includeCatalogs setting.
        /// </summary>
        public const bool IncludeCatalogsDefault = false;

        /// <summary>
        /// The default value for the useShopLinks setting.
        /// </summary>
        public const bool UseShopLinksDefault = true;

        /// <summary>
        /// The default value for the includeFriendlyName setting.
        /// </summary>
        public const bool IncludeFriendlyNameDefault = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not to include catalog names in item urls.
        /// </summary>
        public bool IncludeCatalog { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not shop links should be generated for item urls.
        /// </summary>
        public bool UseShopLinks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether friendly names should be included in item urls.
        /// </summary>
        public bool IncludeFriendlyName { get; set; }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            this.IncludeCatalog = MainUtil.GetBool(config[IncludeCatalogsAttribute], IncludeCatalogsDefault);
            this.UseShopLinks = MainUtil.GetBool(config[UseShopLinksAttribute], UseShopLinksDefault);
            this.IncludeFriendlyName = MainUtil.GetBool(config[IncludeFriendlyNameAttribute], IncludeFriendlyNameDefault);
        }

        /// <summary>
        /// This method returns the dynamicly generated URL based on the item type.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="options">The options</param>
        /// <returns>The dynamically built URL</returns>
        public override string GetDynamicUrl(Item item, LinkUrlOptions options)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(options, "options");

            var url = string.Empty;
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            if (this.UseShopLinks)
            {
                if (searchManager.IsItemProduct(item))
                {
                    url = CatalogUrlManager.BuildProductShopLink(item, this.IncludeCatalog, this.IncludeFriendlyName, true);
                }
                else if (searchManager.IsItemCategory(item))
                {
                    url = CatalogUrlManager.BuildCategoryShopLink(item, this.IncludeCatalog, this.IncludeFriendlyName);
                }
                else if (this.UseShopLinks && searchManager.IsItemVariant(item))
                {
                    url = CatalogUrlManager.BuildVariantShopLink(item, this.IncludeCatalog, this.IncludeFriendlyName, true);
                }
            }
            else
            {
                if (searchManager.IsItemProduct(item))
                {
                    url = CatalogUrlManager.BuildProductLink(item, this.IncludeCatalog, this.IncludeFriendlyName);
                }
                else if (searchManager.IsItemCategory(item))
                {
                    url = CatalogUrlManager.BuildCategoryLink(item, this.IncludeCatalog, this.IncludeFriendlyName);
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                url = base.GetDynamicUrl(item, options);
            }

            return url;
        }
    }
}