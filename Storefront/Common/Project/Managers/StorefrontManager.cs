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
    using Sitecore.Diagnostics;

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
        /// <param name="insertBracketsWhenNotFound">if set to <c>true</c> and the itemName is not found, the itemName is returned with surrounding brackets.</param>
        /// <returns>
        /// A system message based on the key
        /// </returns>
        public static string GetSystemMessage(string messageKey, bool insertBracketsWhenNotFound = true)
        {
            Item lookupItem = null;

            return Lookup(StorefrontConstants.KnowItemNames.SystemMessages, messageKey, out lookupItem, insertBracketsWhenNotFound);
        }

        /// <summary>
        /// Gets the name of the product stock status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>A stock status localizable name from the site content</returns>
        public static string GetProductStockStatusName(StockStatus status)
        {
            if (status == null)
            {
                return string.Empty;
            }

            Item lookupItem = null;

            return Lookup(StorefrontConstants.KnowItemNames.InventoryStatuses, status.Name, out lookupItem, true);
        }

        /// <summary>
        /// Gets the name of the relationship.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lookupItem">The lookup item.</param>
        /// <returns>
        /// A relationship name localizable from the site content.
        /// </returns>
        public static string GetRelationshipName(string name, out Item lookupItem)
        {
            return Lookup(StorefrontConstants.KnowItemNames.Relationships, name, out lookupItem, true);
        }

        /// <summary>
        /// Lookups a specific node in the "Lookups" global area based on the given table and item name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="lookupItem">The lookup item.</param>
        /// <param name="insertBracketsWhenNotFound">if set to <c>true</c> and the itemName is not found, the itemName is returned with surrounding brackets.</param>
        /// <returns>
        /// The located item value if the item was found;  Otherwise string.Empty if the itemName is empty or [itemName] if no item was defined.
        /// </returns>
        public static string Lookup(string tableName, string itemName, out Item lookupItem, bool insertBracketsWhenNotFound)
        {
            Assert.ArgumentNotNullOrEmpty(tableName, "tableName");

            lookupItem = null;

            if (string.IsNullOrWhiteSpace(itemName))
            {
                return string.Empty;
            }

            string contentStartPath = CurrentStorefront.GlobalItem.Axes.GetItem(string.Concat(StorefrontConstants.KnowItemNames.Lookups, "/", tableName)).Paths.Path;
            string statusPath = contentStartPath + "/" + itemName;

            Item item = Sitecore.Context.Database.GetItem(statusPath);
            if (item == null)
            {
                if (insertBracketsWhenNotFound)
                {
                    return string.Format(CultureInfo.InvariantCulture, "[{0}]", itemName);
                }
                else
                {
                    return itemName;
                }
            }

            lookupItem = item;
            return item[StorefrontConstants.KnownFieldNames.Value];
        }
    }
}