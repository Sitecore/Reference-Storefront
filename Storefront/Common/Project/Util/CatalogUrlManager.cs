//---------------------------------------------------------------------
// <copyright file="CatalogUrlManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The CatalogUrlManager class</summary>
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

namespace Sitecore.Reference.Storefront
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.SitecorePipelines;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System;
    using System.Text;

    /// <summary>
    /// Helper class used to build product and category links
    /// </summary>
    public static class CatalogUrlManager
    {
        private const string UrlTokenDelimiter = "_";
        private const string EncodedDelimiter = "[[_]]";

        private static string[] invalidPathCharacters = new string[] { "<", ">", "*", "%", "&", ":", "\\", "?", ".", "\"", " " };
        private static Lazy<ICommerceSearchManager> searchManagerLoader = new Lazy<ICommerceSearchManager>(() => CommerceTypeLoader.CreateInstance<ICommerceSearchManager>());

        private static bool IncludeLanguage
        {
            get
            {
                if (Sitecore.Context.Language != null)
                {
                    if (Sitecore.Context.Site == null || !string.Equals(Sitecore.Context.Language.Name, Sitecore.Context.Site.Language, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Builds a product link
        /// </summary>
        /// <param name="item">The product item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <returns>The built link</returns>
        public static string BuildProductLink(Item item, bool includeCatalog, bool includeFriendlyName)
        {
            var url = BuildUrl(item, includeCatalog, includeFriendlyName, ProductItemResolver.ProductUrlRoute);
            return url;
        }

        /// <summary>
        /// Builds a category link
        /// </summary>
        /// <param name="item">The category item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <returns>The built link</returns>
        public static string BuildCategoryLink(Item item, bool includeCatalog, bool includeFriendlyName)
        {
            var url = BuildUrl(item, includeCatalog, includeFriendlyName, ProductItemResolver.CategoryUrlRoute);
            return url;
        }

        /// <summary>
        /// Builds a catalog item link
        /// </summary>
        /// <param name="item">The category item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <param name="root">The root part of the url e.g. product or category</param>
        /// <returns>The built link</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string BuildUrl(Item item, bool includeCatalog, bool includeFriendlyName, string root)
        {
            Assert.ArgumentNotNull(item, "item");

            var itemFriendlyName = string.Empty;
            var itemName = item.Name.ToLowerInvariant();
            var catalogName = ExtractCatalogName(item, includeCatalog);

            ExtractCatalogItemInfo(item, includeFriendlyName, out itemName, out itemFriendlyName);

            var url = BuildUrl(itemName, itemFriendlyName, catalogName, root);
            return url;
        }

        /// <summary>
        /// Builds a catalog item link.
        /// </summary>
        /// <param name="itemName">The catalog item name.</param>
        /// <param name="itemFriendlyName">The catalog item friendy / display name.</param>
        /// <param name="catalogName">The name of the catalog that includes the catalog item.</param>
        /// <param name="root">The root catalog item path.</param>
        /// <returns>The catalog item link.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string BuildUrl(string itemName, string itemFriendlyName, string catalogName, string root)
        {
            Assert.ArgumentNotNullOrEmpty(itemName, "itemName");

            var route = new StringBuilder("/");

            if (IncludeLanguage)
            {
                route.Append(Sitecore.Context.Language.Name);
                route.Append("/");
            }

            var isGiftCard = (itemName == StorefrontManager.CurrentStorefront.GiftCardProductId);
            if (isGiftCard)
            {
                route.Append(ProductItemResolver.LandingUrlRoute);
                route.Append("/");
                route.Append(ProductItemResolver.BuyGiftCardUrlRoute);
            }
            else
            {
                if (!string.IsNullOrEmpty(catalogName))
                {
                    route.Append(EncodeUrlToken(catalogName, true));
                    route.Append("/");
                }

                route.Append(root);
                route.Append("/");

                if (!string.IsNullOrEmpty(itemFriendlyName))
                {
                    route.Append(EncodeUrlToken(itemFriendlyName, true));
                    route.Append(UrlTokenDelimiter);
                }

                route.Append(EncodeUrlToken(itemName, false));
            }

            var url = StorefrontManager.StorefrontUri(route.ToString());
            return url;
        }

        /// <summary>
        /// Builds a product variant shop link
        /// </summary>
        /// <param name="item">The product variant item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <param name="includeCurrentCategory">Specifies whether or not to include the current category information in the URL, or to instead use the primary parent category.</param>
        /// <returns>The built link</returns>
        public static string BuildVariantShopLink(Item item, bool includeCatalog, bool includeFriendlyName, bool includeCurrentCategory)
        {
            Assert.ArgumentNotNull(item, "item");

            var variantName = string.Empty;
            var variantId = string.Empty;
            var productName = string.Empty;
            var productId = string.Empty;
            var categoryName = string.Empty;
            var categoryId = string.Empty;
            var catalogName = ExtractCatalogName(item, includeCatalog);

            ExtractCatalogItemInfo(item, includeFriendlyName, out variantId, out variantName);

            var parentItem = item.Parent;
            ExtractCatalogItemInfo(parentItem, includeFriendlyName, out productId, out productName);

            if (includeCurrentCategory)
            {
                ExtractCategoryInfoFromCurrentShopUrl(out categoryId, out categoryName);
            }

            if (string.IsNullOrEmpty(categoryId))
            {
                var grandParentItem = parentItem.Parent;
                ExtractCatalogItemInfo(grandParentItem, includeFriendlyName, out categoryId, out categoryName);
            }

            var url = BuildShopUrl(categoryId, categoryName, productId, productName, variantId, variantName, catalogName);
            return url;
        }

        /// <summary>
        /// Builds a product shop link
        /// </summary>
        /// <param name="item">The product item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <param name="includeCurrentCategory">Specifies whether or not to include the current category information in the URL, or to instead use the primary parent category.</param>
        /// <returns>The built link</returns>
        public static string BuildProductShopLink(Item item, bool includeCatalog, bool includeFriendlyName, bool includeCurrentCategory)
        {
            Assert.ArgumentNotNull(item, "item");

            var productName = string.Empty;
            var productId = string.Empty;
            var categoryName = string.Empty;
            var categoryId = string.Empty;
            var catalogName = ExtractCatalogName(item, includeCatalog);

            ExtractCatalogItemInfo(item, includeFriendlyName, out productId, out productName);

            if (includeCurrentCategory)
            {
                ExtractCategoryInfoFromCurrentShopUrl(out categoryId, out categoryName);
            }

            if (string.IsNullOrEmpty(categoryId))
            {
                var parentItem = item.Parent;
                ExtractCatalogItemInfo(parentItem, includeFriendlyName, out categoryId, out categoryName);
            }

            var url = BuildShopUrl(categoryId, categoryName, productId, productName, string.Empty, string.Empty, catalogName);
            return url;
        }

        /// <summary>
        /// Builds a category shop link
        /// </summary>
        /// <param name="item">The category item to build a link for</param>
        /// <param name="includeCatalog">Whether or not to include catalog id in the url</param>
        /// <param name="includeFriendlyName">Specifies whether or not to include the friendly / display name in the url.</param>
        /// <returns>The built link</returns>
        public static string BuildCategoryShopLink(Item item, bool includeCatalog, bool includeFriendlyName)
        {
            Assert.ArgumentNotNull(item, "item");

            var categoryName = string.Empty;
            var categoryId = string.Empty;
            var catalogName = ExtractCatalogName(item, includeCatalog);

            ExtractCatalogItemInfo(item, includeFriendlyName, out categoryId, out categoryName);

            var url = BuildShopUrl(categoryId, categoryName, string.Empty, string.Empty, string.Empty, string.Empty, catalogName);
            return url;
        }

        /// <summary>
        /// Gets the canonical shop URI for a catalog item.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <param name="categoryName">The category friendly name / display name.</param>
        /// <param name="productId">The product ID.</param>
        /// <param name="productName">The product friendly name / display name.</param>
        /// <param name="variantId">The product variant ID.</param>
        /// <param name="variantName">The product variant fiendly name / display name.</param>
        /// <param name="catalogName">The name of the catalog that includes the catalog item.</param>
        /// <returns>The canonical shop URI for the catalog item.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string BuildShopUrl(string categoryId, string categoryName, string productId, string productName, string variantId, string variantName, string catalogName)
        {
            Assert.ArgumentNotNullOrEmpty(categoryId, "categoryId");

            var route = new StringBuilder("/");

            if (IncludeLanguage)
            {
                route.Append(Sitecore.Context.Language.Name);
                route.Append("/");
            }

            var isGiftCard = (productId == StorefrontManager.CurrentStorefront.GiftCardProductId);
            if (isGiftCard)
            {
                route.Append(ProductItemResolver.LandingUrlRoute);
                route.Append("/");
                route.Append(ProductItemResolver.BuyGiftCardUrlRoute);
            }
            else
            {
                if (!string.IsNullOrEmpty(catalogName))
                {
                    route.Append(EncodeUrlToken(catalogName, true));
                    route.Append("/");
                }

                route.Append(ProductItemResolver.ShopUrlRoute);
                route.Append("/");

                if (!string.IsNullOrEmpty(categoryName))
                {
                    route.Append(EncodeUrlToken(categoryName, true));
                    route.Append(UrlTokenDelimiter);
                }

                route.Append(EncodeUrlToken(categoryId, false));

                if (!string.IsNullOrEmpty(productId))
                {
                    route.Append("/");

                    if (!string.IsNullOrEmpty(productName))
                    {
                        route.Append(EncodeUrlToken(productName, true));
                        route.Append(UrlTokenDelimiter);
                    }

                    route.Append(EncodeUrlToken(productId, false));

                    if (!string.IsNullOrEmpty(variantId))
                    {
                        route.Append("/");

                        if (!string.IsNullOrEmpty(variantName))
                        {
                            route.Append(EncodeUrlToken(variantName, true));
                            route.Append(UrlTokenDelimiter);
                        }

                        route.Append(EncodeUrlToken(variantId, false));
                    }
                }
            }

            var url = StorefrontManager.StorefrontUri(route.ToString());
            return url;
        }

        /// <summary>
        /// Extracts the target catalog item ID from the current request URL.
        /// </summary>
        /// <returns>The target catalog item ID.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string ExtractItemIdFromCurrentUrl()
        {
            return ExtractItemId(Sitecore.Web.WebUtil.GetUrlName(0));
        }

        /// <summary>
        /// Extracts the target category name from the current request URL.
        /// </summary>
        /// <returns>The target category name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string ExtractCategoryNameFromCurrentUrl()
        {
            var categoryFolder = Sitecore.Web.WebUtil.GetUrlName(1);
            if (string.IsNullOrEmpty(categoryFolder))
            {
                return ExtractItemIdFromCurrentUrl();
            }

            return ExtractItemId(categoryFolder);
        }

        /// <summary>
        /// Extracts the target catalog name from the current request URL.
        /// </summary>
        /// <returns>The target catalog name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "By design, we ned a string to be returned.")]
        public static string ExtractCatalogNameFromCurrentUrl()
        {
            var linkProvider = Sitecore.Links.LinkManager.Provider as CatalogLinkProvider;
            if (linkProvider != null && linkProvider.IncludeCatalog)
            {
                var catalogName = Sitecore.Web.WebUtil.GetUrlName(2);
                if (!string.IsNullOrEmpty(catalogName))
                {
                    return catalogName;
                }

                catalogName = Sitecore.Web.WebUtil.GetUrlName(1);
                if (!string.IsNullOrEmpty(catalogName))
                {
                    return catalogName;
                }
            }

            return StorefrontManager.CurrentStorefront.DefaultCatalog.Name;
        }

        /// <summary>
        /// Extracts a catalog item ID from a URL folder name.
        /// </summary>
        /// <param name="folder">The URL folder name.</param>
        /// <returns>The catalog item ID.</returns>
        public static string ExtractItemId(string folder)
        {
            var itemName = folder;
            if (folder != null && folder.Contains(UrlTokenDelimiter))
            {
                var tokens = folder.Split(new[] { UrlTokenDelimiter }, StringSplitOptions.None);
                itemName = tokens[tokens.Length - 1];
            }

            return DecodeUrlToken(itemName);
        }

        /// <summary>
        /// Extracts a catalog item friendly name from a URL folder name.
        /// </summary>
        /// <param name="folder">The URL folder name.</param>
        /// <returns>The catalog item friendly name.</returns>
        public static string ExtractItemName(string folder)
        {
            var itemName = string.Empty;
            if (folder != null && folder.Contains(UrlTokenDelimiter))
            {
                var tokens = folder.Split(new[] { UrlTokenDelimiter }, StringSplitOptions.None);
                itemName = tokens[tokens.Length - 2];
            }

            return DecodeUrlToken(itemName);
        }

        private static void ExtractCatalogItemInfo(string folder, out string itemId, out string itemName)
        {
            itemId = ExtractItemId(folder);
            itemName = ExtractItemName(folder);
        }

        private static void ExtractCategoryInfoFromCurrentShopUrl(out string categoryId, out string categoryName)
        {
            categoryId = string.Empty;
            categoryName = string.Empty;

            if (Sitecore.Web.WebUtil.GetUrlName(1).ToLowerInvariant() == ProductItemResolver.ShopUrlRoute)
            {
                // store/<category>
                ExtractCatalogItemInfo(Sitecore.Web.WebUtil.GetUrlName(0), out categoryId, out categoryName);
            }

            if (Sitecore.Web.WebUtil.GetUrlName(2).ToLowerInvariant() == ProductItemResolver.ShopUrlRoute)
            {
                // store/<category>/<product>
                ExtractCatalogItemInfo(Sitecore.Web.WebUtil.GetUrlName(1), out categoryId, out categoryName);
            }

            if (Sitecore.Web.WebUtil.GetUrlName(3).ToLowerInvariant() == ProductItemResolver.ShopUrlRoute)
            {
                // store/<category>/<product>/<variant>
                ExtractCatalogItemInfo(Sitecore.Web.WebUtil.GetUrlName(2), out categoryId, out categoryName);
            }
        }

        private static string ExtractCatalogName(Item item, bool includeCatalog)
        {
            Assert.ArgumentNotNull(item, "item");

            if (includeCatalog)
            {
                return item[CommerceConstants.KnownFieldIds.CatalogName].ToLowerInvariant();
            }

            return string.Empty;
        }

        private static void ExtractCatalogItemInfo(Item item, bool includeFriendlyName, out string itemName, out string itemFriendlyName)
        {
            Assert.ArgumentNotNull(item, "item");

            if (searchManagerLoader.Value.IsItemCatalog(item) || searchManagerLoader.Value.IsItemVirtualCatalog(item))
            {
                itemName = ProductItemResolver.ProductUrlRoute;
                itemFriendlyName = string.Empty;
            }
            else
            {
                itemName = item.Name.ToLowerInvariant();
                itemFriendlyName = string.Empty;
                if (includeFriendlyName)
                {
                    itemFriendlyName = item.DisplayName;
                }
            }
        }

        private static string EncodeUrlToken(string urlToken, bool removeInvalidPathCharacters)
        {
            if (!string.IsNullOrEmpty(urlToken))
            {
                if (removeInvalidPathCharacters)
                {
                    foreach (var character in invalidPathCharacters)
                    {
                        urlToken = urlToken.Replace(character, string.Empty);
                    }
                }

                urlToken = Uri.EscapeDataString(urlToken).Replace(UrlTokenDelimiter, EncodedDelimiter);
            }

            return urlToken;
        }

        private static string DecodeUrlToken(string urlToken)
        {
            if (!string.IsNullOrEmpty(urlToken))
            {
                urlToken = Uri.UnescapeDataString(urlToken).Replace(EncodedDelimiter, UrlTokenDelimiter);
            }

            return urlToken;
        }
    }
}