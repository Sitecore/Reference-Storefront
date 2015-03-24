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

    /// <summary>
    /// The manager for storefronts
    /// </summary>
    public static class StorefrontManager
    {
        private const string IndexNameFormat = "sitecore_{0}_index";
        private static Dictionary<string, string> _statuses; 
        
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
                searchResults = searchResults.Where(item => item[StorefrontConstants.KnownFieldNames.Key] == messageKey);

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
                return value == null ? string.Empty : value.ToString();
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

            var statusName = status.Name;
            if (_statuses != null && _statuses.ContainsKey(statusName))
            {
                return _statuses[statusName];
            }

            _statuses = new Dictionary<string, string>();
            var lookups = CurrentStorefront.GlobalItem.Children[StorefrontConstants.KnowItemNames.Lookups];
            if (lookups == null)
            {
                return statusName;
            }

            var inventoryStatuses = lookups.Children[StorefrontConstants.KnowItemNames.InventoryStatuses];
            if (inventoryStatuses == null)
            {
                return statusName;
            }

            if (!inventoryStatuses.Children.Any())
            {
                return statusName;
            }

            foreach (Item statusItem in inventoryStatuses.Children)
            {
                var value = statusItem.Fields[StorefrontConstants.KnownFieldNames.Value];
                var key = statusItem.Name;
                if (value == null || _statuses.ContainsKey(key))
                {
                    continue;
                }

                _statuses.Add(key, value.ToString());
            }

            return _statuses.ContainsKey(statusName) ? _statuses[statusName] : statusName;
        }
    }
}