//-----------------------------------------------------------------------
// <copyright file="StorefrontManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the StorefrontManager class.</summary>
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.Data.Items;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Commerce.Entities.Inventory;
    using System.Web;
    using System;

    /// <summary>
    /// The manager for storefronts
    /// </summary>
    public static class StorefrontManager
    {
        private const string IndexNameFormat = "sitecore_{0}_index";
        
        /// <summary>
        /// Gets the current sitecontext
        /// </summary>
        public static ISiteContext CurrentSiteContext
        {
            get
            {
                return CommerceTypeLoader.CreateInstance<ISiteContext>();
            }
        }

        /// <summary>
        /// Gets the Current Storefront being accessed
        /// </summary>
        public static CommerceStorefront CurrentStorefront
        {
            get
            {
                string path = Context.Site.RootPath + Context.Site.StartItem;

                var storefront = CommerceTypeLoader.CreateInstance<CommerceStorefront>(Context.Database.GetItem(path));
                return storefront;
            }
        }

        /// <summary>
        /// Gets the URL for the current storefronts home page
        /// </summary>
        public static string StorefrontHome
        {
            get
            {
                return "/";
            }
        }

        /// <summary>
        /// Returns a proper local URI for a route
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>The store front url.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string StorefrontUri(string route)
        {
            return route;
        }

        /// <summary>
        /// Used to return an external link in HTTP or HTTPS depending on the current state of the request.
        /// </summary>
        /// <param name="externalLink">The base URL string.</param>
        /// <returns>Returns the external link.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string ExternalUri(string externalLink)
        {
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                return "https://" + externalLink;
            }
            else
            {
                return "http://" + externalLink;
            }
        }

        /// <summary>
        /// Selects the external URI based on the security of the current request connection.
        /// </summary>
        /// <param name="unsecuredConnection">The unsecured connection.</param>
        /// <param name="securedConnection">The secured connection.</param>
        /// <returns>The proper url to use.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string SelectExternalUri(string unsecuredConnection, string securedConnection)
        {
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                return securedConnection;
            }
            else
            {
                return unsecuredConnection;
            }
        }

        /// <summary>
        /// Returns a secure HTTPS link.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>The HTTPS link.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string SecureStorefrontUri(string route)
        {
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                return route;
            }
            else
            {
                UrlBuilder builder = new UrlBuilder(HttpContext.Current.Request.Url);

                if (!route.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    route = "/" + route;
                }

                return string.Format(CultureInfo.InvariantCulture, "https://{0}{1}", builder.Host, route);
            }
        }

        /// <summary>
        /// Gets the HTML system message.
        /// </summary>
        /// <param name="messageKey">The message key.</param>
        /// <returns>The system message as an HtmlString/</returns>
        public static HtmlString GetHtmlSystemMessage(string messageKey)
        {
            return new HtmlString(GetSystemMessage(messageKey));
        }

            /// <summary>
        /// Gets the system message.
        /// </summary>
        /// <param name="messageKey">The message key.</param>
        /// <returns>A system message based on the key</returns>
        public static string GetSystemMessage(string messageKey)
        {
            string indexName = string.Format(CultureInfo.InvariantCulture, IndexNameFormat, Context.Database.Name);
            string contentStartPath = CurrentStorefront.GlobalItem.Axes.GetItem(string.Concat(StorefrontConstants.KnowItemNames.Lookups, "/", StorefrontConstants.KnowItemNames.SystemMessages)).Paths.Path;
            var searchIndex = ContentSearchManager.GetIndex(indexName);

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<SearchResultItem>();
                searchResults = searchResults.Where(item => item.Path.StartsWith(contentStartPath));
                searchResults = searchResults.Where(item => item.Language == SearchNavigation.CurrentLanguageName);
                searchResults = searchResults.Where(item => item.Name == messageKey);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(new CommerceSearchOptions(), results);

                if (response.ResponseItems == null || response.TotalItemCount == 0)
                {
                    return string.Empty;
                }

                var resultItem = response.ResponseItems.FirstOrDefault();
                if (resultItem == null)
                {
                    return string.Empty;
                }

                var value = resultItem.Fields[StorefrontConstants.KnownFieldNames.Value];
                return value == null ? string.Empty : value.Value;
            }
        }

        /// <summary>
        /// Gets the name of the product stock status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>Aa stock status localizable name from the site content</returns>
        public static string GetProductStockStatusName(StockStatus status)
        {
            if (status == null)
            {
                return string.Empty;
            }

            string contentStartPath = CurrentStorefront.GlobalItem.Axes.GetItem(string.Concat(StorefrontConstants.KnowItemNames.Lookups, "/", StorefrontConstants.KnowItemNames.InventoryStatuses)).Paths.Path;
            string statusPath = contentStartPath + "/" + status.Name;

            Item inventoryItem = Sitecore.Context.Database.GetItem(statusPath);
            if (inventoryItem == null)
            {
                return status.Name;
            }

            return inventoryItem[StorefrontConstants.KnownFieldNames.Value];
        }
    }
}