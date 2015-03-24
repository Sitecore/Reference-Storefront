//---------------------------------------------------------------------
// <copyright file="MetadataManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Metadata manager class used to generate the metadata tags.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Reference.Storefront.SitecorePipelines;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// The MetadataManager class definition.
    /// </summary>
    public static class MetadataManager
    {
        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <returns>The Html metadata tags if necessary.</returns>
        public static HtmlString GetTags()
        {
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();

            if (siteContext.IsCategory)
            {
                return new HtmlString(GetCategoryTags(siteContext.CurrentCatalogItem));
            }
            else if (siteContext.IsProduct)
            {
                return new HtmlString(GetProductTags(siteContext.CurrentCatalogItem));
            }

            return new HtmlString(string.Empty);
        }

        /// <summary>
        /// Gets the category tags.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the metadata tags for the given category.</returns>
        private static string GetCategoryTags(Item item)
        {
            return String.Format(CultureInfo.InvariantCulture, "<link rel='canonical' href='{0}'/>", GetServerUrl() + "/category/" + item.Name);
        }

        /// <summary>
        /// Gets the product tags.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The metadata tags for the given product item.</returns>
        private static string GetProductTags(Item item)
        {
            return String.Format(CultureInfo.InvariantCulture, "<link rel='canonical' href='{0}'/>", GetServerUrl() + "/product/" + item.Name);
        }

        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <returns>Returns the url as an Http link even if we are browsing as Https.</returns>
        private static string GetServerUrl()
        {
            string serverUrl = Web.WebUtil.GetServerUrl();
            if (serverUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                serverUrl = serverUrl.Replace(serverUrl.Substring(0, 8), "http://");
            }

            return serverUrl;
        }
    }
}
