//---------------------------------------------------------------------
// <copyright file="ProductItemResolver.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The product item resolver</summary>
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
    using System.Web;
    using System.Web.Routing;
    using Sitecore.Data;
    using Sitecore.Pipelines;
    using Sitecore.Web;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Caching;
    using Sitecore.Reference.Storefront.Managers;
    
    /// <summary>
    /// Class representing a ProductItemResolver
    /// </summary>
    public class ProductItemResolver
    {
        /// <summary>
        /// Constant string representing the product's route name
        /// </summary>
        public const string ShopProductRouteName = "shop-product";

        /// <summary>
        /// Constant string representing the product and catalog route name
        /// </summary>
        public const string ShopProductWithCatalogRouteName = "shop-product-catalog";

        /// <summary>
        /// Constant string representing the category's route name
        /// </summary>
        public const string ShopCategoryRouteName = "shop-category";

        /// <summary>
        /// Constant string representing the category and catalog route name
        /// </summary>
        public const string ShopCategoryWithCatalogRouteName = "shop-category-catalog";

        /// <summary>
        /// Constant string representing the product's route name
        /// </summary>
        public const string ProductRouteName = "product";

        /// <summary>
        /// Constant string representing a product and catalog route name
        /// </summary>
        public const string ProductWithCatalogRouteName = "product-catalog";

        /// <summary>
        /// Constant string representing the category's route name
        /// </summary>
        public const string CategoryRouteName = "category";

        /// <summary>
        /// Constant string representing a category and cataog route name
        /// </summary>
        public const string CategoryWithCatalogRouteName = "category-catalog";

        /// <summary>
        /// The id field used in a route
        /// </summary>
        public const string IdField = "id";

        /// <summary>
        /// The itemType field used in a route
        /// </summary>
        public const string ItemTypeField = "itemType";

        /// <summary>
        /// The catalog field used in a route
        /// </summary>
        public const string CatalogField = "catalog";

        /// <summary>
        /// The product item type assigned to a route
        /// </summary>
        public const string ProductItemType = "product";

        /// <summary>
        /// The category item type assigned to a route
        /// </summary>
        public const string CategoryItemType = "category";

        /// <summary>
        /// The product url route base
        /// </summary>
        public const string ProductUrlRoute = "product";

        /// <summary>
        /// The category url route base
        /// </summary>
        public const string CategoryUrlRoute = "category";

        /// <summary>
        /// The shop url route base
        /// </summary>
        public const string ShopUrlRoute = "shop";

        /// <summary>
        /// The langing url route base
        /// </summary>
        public const string LandingUrlRoute = "landing";

        /// <summary>
        /// The BuyGiftCard url route
        /// </summary>
        public const string BuyGiftCardUrlRoute = "buygiftcard";

        /// <summary>
        /// Checks to see if there is a catalog item that maps to the current id
        /// </summary>
        /// <param name="itemId">The ID of the catalog item.</param>
        /// <param name="catalogName">The name of the catalog that contains the catalog item.</param>
        /// <param name="isProduct">Specifies whether the item is a product.</param>
        /// <returns>An item if found, otherwise null</returns>
        public static Sitecore.Data.Items.Item ResolveCatalogItem(string itemId, string catalogName, bool isProduct)
        {
            Sitecore.Data.Items.Item foundItem = null;

            // If we make it here, the right route was used, but might have an empty value
            if (!string.IsNullOrEmpty(itemId))
            {
                var cachekey = "FriendlyUrl-" + itemId + "-" + catalogName;
                ICacheProvider cacheProvider = CommerceTypeLoader.GetCacheProvider(CommerceConstants.KnownCacheNames.FriendlyUrlsCache);
                var id = cacheProvider.GetData<ID>(CommerceConstants.KnownCachePrefixes.Sitecore, CommerceConstants.KnownCacheNames.FriendlyUrlsCache, cachekey);

                if (ID.IsNullOrEmpty(id) || id == ID.Undefined)
                {
                    if (isProduct)
                    {
                        foundItem = SearchNavigation.GetProduct(itemId, catalogName);
                    }
                    else
                    {
                        foundItem = SearchNavigation.GetCategory(itemId, catalogName);
                    }

                    if (foundItem != null)
                    {
                        cacheProvider.AddData<ID>(CommerceConstants.KnownCachePrefixes.Sitecore, CommerceConstants.KnownCacheNames.FriendlyUrlsCache, cachekey, foundItem.ID);
                    }
                    else
                    {
                        cacheProvider.AddData<ID>(CommerceConstants.KnownCachePrefixes.Sitecore, CommerceConstants.KnownCacheNames.FriendlyUrlsCache, cachekey, ID.Undefined);
                    }
                }
                else if (id != ID.Undefined && id != ID.Null)
                {
                    foundItem = Context.Database.GetItem(id);
                }
            }

            return foundItem;
        }

        /// <summary>
        /// This method returns the Route data value
        /// </summary>
        /// <param name="routeData">The route</param>
        /// <returns>The value if found, empty string if route is found but not value, null if route invalid</returns>
        public virtual CatalogRouteData GetRouteDataValue(RouteData routeData)
        {
            var data = new CatalogRouteData();

            if (routeData.Values.ContainsKey(ItemTypeField))
            {
                data.IsProduct = routeData.Values[ItemTypeField].ToString() == ProductItemType ? true : false;
            }
            else
            {
                return null;
            }

            if (routeData.Values.ContainsKey(IdField))
            {
                data.Id = CatalogUrlManager.ExtractItemId(routeData.Values[IdField].ToString());
            }
            else
            {
                return null;
            }

            if (routeData.Values.ContainsKey(CatalogField))
            {
                data.Catalog = routeData.Values[CatalogField].ToString();
            }

            if (string.IsNullOrEmpty(data.Catalog))
            {
                var defaultCatalog = StorefrontManager.CurrentStorefront.DefaultCatalog;

                if (defaultCatalog != null)
                {
                    data.Catalog = defaultCatalog.Name;
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the product id from the incomming request
        /// </summary>
        /// <returns>The product id if found</returns>
        public virtual CatalogRouteData GetCatalogItemFromIncomingRequest()
        {
            var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(siteContext.CurrentContext));

            if (routeData != null)
            {
                var data = this.GetRouteDataValue(routeData);

                return data;
            }

            return null;
        }

        /// <summary>
        /// Runs the processor.
        /// </summary>
        /// <param name="args">The args.</param>
        public virtual void Process(PipelineArgs args)
        {
            if (Context.Item != null)
            {
                return;
            }

            var routeData = this.GetCatalogItemFromIncomingRequest();

            if (routeData != null)
            {
                var siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>();

                Context.Item = ResolveCatalogItem(routeData.Id, routeData.Catalog, routeData.IsProduct);
                siteContext.CurrentCatalogItem = Context.Item;

                if (Context.Item == null)
                {
                    WebUtil.Redirect("~/");
                }
            }
        }
    }
}