//-----------------------------------------------------------------------
// <copyright file="CategoryCookieHelper.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Helper class to handle the last visited category cookie.</summary>
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

namespace Sitecore.Reference.Storefront.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines the CategoryCookieHelper class.
    /// </summary>
    public static class CategoryCookieHelper
    {
        private const string CookieName = "_lastVisitedCategory";
        private const string VisitorIdKey = "VisitorId";
        private const string CategoryIdKey = "CategoryId";

        /// <summary>
        /// Gets the last visited category.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The last visited category id or empty sting.</returns>
        public static string GetLastVisitedCategory(string customerId)
        {
            var categoryCookie = HttpContext.Current.Request.Cookies[CookieName];

            return categoryCookie != null && categoryCookie[VisitorIdKey] != null && categoryCookie[VisitorIdKey].Equals(customerId, StringComparison.OrdinalIgnoreCase) ? categoryCookie[CategoryIdKey] : string.Empty;
        }

        /// <summary>
        /// Sets the last visited category.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        public static void SetLastVisitedCategory(string customerId, string categoryId)
        {
            // The cookie does not defined an expiry date therefore the browser will not persist it.
            var categoryCookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            categoryCookie.Values[VisitorIdKey] = customerId;
            categoryCookie.Values[CategoryIdKey] = categoryId;
            HttpContext.Current.Response.Cookies.Add(categoryCookie);
        }
    }
}
