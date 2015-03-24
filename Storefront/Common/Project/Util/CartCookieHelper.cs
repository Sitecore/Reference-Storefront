//---------------------------------------------------------------------
// <copyright file="CartCookieHelper.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The CartCookieHelper class</summary>
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
    using System.Web;
    using System;

    /// <summary>
    /// Cart Cookie Helper
    /// </summary>
    /// <remarks>
    /// As constantly getting a basket from Commerce server can be expensive, this cache can be used as a way to mitigate getting a cart.
    /// When a cart for a customer is created, it will be placed into the cache.  When operations are performed on the cart, the cahce is invalidated for that
    /// customer cart, if it exists in the cahce.
    /// </remarks>
    public static class CartCookieHelper
    {
        /// <summary>
        /// mini cart cookie name
        /// </summary>
        public const string CookieName = "_minicart";

        /// <summary>
        /// visitorId for the cart cookie
        /// </summary>
        public const string VisitorIdKey = "VisitorId";

        /// <summary>
        /// cookie expiration time in days
        /// </summary>
        private const int CookieExpirationInDays = 365;

        /// <summary>
        /// Does the cart cookie exist for the given customer
        /// </summary>
        /// <param name="customerId">the customer id</param>
        /// <returns>true if the cookie exists</returns>
        public static bool DoesCookieExistForCustomer(string customerId)
        {
            var cartCookie = HttpContext.Current.Request.Cookies[CookieName];

            return cartCookie != null && cartCookie.Values["VisitorId"] == customerId;
        }

        /// <summary>
        /// Creates the cart cookie for the customer
        /// </summary>
        /// <param name="customerId">the customer id</param>
        public static void CreateCartCookieForCustomer(string customerId)
        {
            var cartCookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            cartCookie.Values["VisitorId"] = customerId;
            cartCookie.Expires = DateTime.Now.AddDays(CookieExpirationInDays);
            HttpContext.Current.Response.Cookies.Add(cartCookie);
        }

        /// <summary>
        /// Deletes the cart cookie for the customer
        /// </summary>
        /// <param name="customerId">the customer id</param>
        /// <returns>true if the cookie was deleted</returns>
        public static bool DeleteCartCookieForCustomer(string customerId)
        {
            var cartCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cartCookie == null)
            {
                return false;
            }

            // invalidate the cookie
            HttpContext.Current.Response.Cookies.Remove(CookieName);
            cartCookie.Expires = DateTime.Now.AddDays(-10);
            cartCookie.Values["VisitorId"] = null;
            cartCookie.Value = null;
            HttpContext.Current.Response.SetCookie(cartCookie);

            return true;
        }
    }
}