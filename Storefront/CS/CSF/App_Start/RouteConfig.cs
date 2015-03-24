//-----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the RouteConfig class.</summary>
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
    using System.Web.Mvc;
    using System.Web.Routing;
    using Sitecore.Reference.Storefront.SitecorePipelines;

    /// <summary>
    /// Used to register all routes
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        ///  Called to register any routes
        /// </summary>
        /// <param name="routes">The route collection to add to</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: ProductItemResolver.ShopCategoryRouteName,
                url: ProductItemResolver.ShopUrlRoute + "/{id}",
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.CategoryItemType });

            routes.MapRoute(
                name: ProductItemResolver.ShopProductRouteName,
                url: ProductItemResolver.ShopUrlRoute + "/{category}/{id}",
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.ProductItemType });

            routes.MapRoute(
                name: ProductItemResolver.ShopCategoryWithCatalogRouteName,
                url: "{catalog}/" + ProductItemResolver.ShopUrlRoute + "/{id}",
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.CategoryItemType });

            routes.MapRoute(
                name: ProductItemResolver.ShopProductWithCatalogRouteName,
                url: "{catalog}/" + ProductItemResolver.ShopUrlRoute + "/{category}/{id}",
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.ProductItemType });

            routes.MapRoute(
                name: ProductItemResolver.CategoryRouteName, 
                url: ProductItemResolver.CategoryUrlRoute + "/{id}", 
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.CategoryItemType });
            
            routes.MapRoute(
                name: ProductItemResolver.ProductRouteName,
                url: ProductItemResolver.ProductUrlRoute + "/{id}", 
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.ProductItemType });
            
            routes.MapRoute(
                name: "ProductAction",
                url: ProductItemResolver.ProductUrlRoute + "/{action}/{id}", 
                defaults: new { controller = "Catalog", id = UrlParameter.Optional, itemType = ProductItemResolver.ProductItemType });

            routes.MapRoute(
                name: ProductItemResolver.CategoryWithCatalogRouteName,
                url: "{catalog}/" + ProductItemResolver.CategoryUrlRoute + "/{id}", 
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.CategoryItemType });

            routes.MapRoute(
                name: ProductItemResolver.ProductWithCatalogRouteName,
                url: "{catalog}/" + ProductItemResolver.ProductUrlRoute + "/{id}", 
                defaults: new { id = UrlParameter.Optional, itemType = ProductItemResolver.ProductItemType });

            routes.MapRoute(
                name: "DeleteLineItem",
                url: "cart/DeleteLineItem",
                defaults: new { controller = "Cart", action = "DeleteLineItem", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "UpdateLineItem",
                url: "cart/UpdateLineItem",
                defaults: new { controller = "Cart", action = "UpdateLineItem", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "ApplyDiscount",
                url: "cart/ApplyDiscount",
                defaults: new { controller = "Cart", action = "ApplyDiscount", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "RemoveDiscount",
                url: "cart/RemoveDiscount",
                defaults: new { controller = "Cart", action = "RemoveDiscount", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "logoff",
                url: "logoff",
                defaults: new { controller = "Account", action = "LogOff", storefront = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AddItemsToCart",
                url: "cart/additemstocart",
                defaults: new { controller = "Cart", action = "AddItemsToCart", items = UrlParameter.Optional });

            routes.MapRoute(
                name: "GetCheckoutData",
                url: "checkout/GetCheckoutData",
                defaults: new { controller = "Checkout", action = "GetCheckoutData", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "GetShippingMethods",
                url: "checkout/GetShippingMethods/{party}/{shippingPreferenceType}/{lines}",
                defaults: new { controller = "Checkout", action = "GetShippingMethods", id = UrlParameter.Optional, party = UrlParameter.Optional, shippingPreferenceType = UrlParameter.Optional, lines = UrlParameter.Optional });

            routes.MapRoute(
                name: "SetShippingMethods",
                url: "checkout/SetShippingMethods/{orderShippingPreferenceType}/{shippings}/{parties}",
                defaults: new { controller = "Checkout", action = "SetShippingMethods", id = UrlParameter.Optional, orderShippingPreferenceType = UrlParameter.Optional, shippings = UrlParameter.Optional, parties = UrlParameter.Optional });
            
            routes.MapRoute(
                name: "SubmitOrder",
                url: "checkout/SubmitOrder/{userEmail}/{creditCard}/{billingAddress}/{giftCard}/{loyaltyCard}",
                defaults: new
                {
                    controller = "Checkout", 
                    action = "SubmitOrder", 
                    id = UrlParameter.Optional, 
                    userEmail = UrlParameter.Optional, 
                    creditCard = UrlParameter.Optional, 
                    billingAddress = UrlParameter.Optional, 
                    giftCard = UrlParameter.Optional, 
                    loyaltyCard = UrlParameter.Optional
                });

            routes.MapRoute(
                name: "GetGiftCardBalance",
                url: "checkout/GetGiftCardBalance/{giftCardId}",
                defaults: new { controller = "Checkout", action = "GetGiftCardBalance", id = UrlParameter.Optional, giftCardId = UrlParameter.Optional });

            routes.MapRoute(
                name: "GetAvailableStates",
                url: "checkout/GetAvailableStates",
                defaults: new { controller = "Checkout", action = "GetAvailableStates", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "UpdateLoyaltyCard",
                url: "checkout/UpdateLoyaltyCard",
                defaults: new { controller = "Checkout", action = "UpdateLoyaltyCard", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "DeleteWishListLineItem",
                url: "wishlist/DeleteLineItem/{productId}",
                defaults: new { controller = "WishList", action = "DeleteLineItem", id = UrlParameter.Optional, productId = UrlParameter.Optional });

            routes.MapRoute(
                name: "UpdateWishListLineItem",
                url: "wishlist/UpdateLineItem/{productId}/{quantity}",
                defaults: new { controller = "WishList", action = "UpdateLineItem", id = UrlParameter.Optional, productId = UrlParameter.Optional, quantity = UrlParameter.Optional });
        }
    }
}