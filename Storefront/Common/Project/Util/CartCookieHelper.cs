//-----------------------------------------------------------------------
// <copyright file="CartCookieHelper.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the cart cookie helper class.</summary>
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
        /// The anonymous cart cookie name
        /// </summary>
        public const string AnonymousCartCookieName = "asc";

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

            return cartCookie != null && cartCookie.Values[VisitorIdKey] == customerId;
        }

        /// <summary>
        /// Creates the cart cookie for the customer
        /// </summary>
        /// <param name="customerId">the customer id</param>
        public static void CreateCartCookieForCustomer(string customerId)
        {
            var cartCookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            cartCookie.Values[VisitorIdKey] = customerId;            
            cartCookie.HttpOnly = true;            
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
            cartCookie.Values[VisitorIdKey] = null;
            cartCookie.Value = null;
            HttpContext.Current.Response.SetCookie(cartCookie);

            return true;
        }

        /// <summary>
        /// Gets the anonymous cart identifier from the session cookie.
        /// </summary>
        /// <returns>Anonymous cart Id</returns>
        public static string GetAnonymousCartIdFromCookie()
        {
            var cartCookie = HttpContext.Current.Request.Cookies[AnonymousCartCookieName];
           
            if (cartCookie == null || string.IsNullOrEmpty(cartCookie.Value))
            {
                var cartId = Guid.NewGuid().ToString();
                cartCookie = new HttpCookie(AnonymousCartCookieName, cartId);
                cartCookie.HttpOnly = true;                
                HttpContext.Current.Response.SetCookie(cartCookie);
                return cartId;
            }

            return cartCookie.Value;
        }
    }
}
