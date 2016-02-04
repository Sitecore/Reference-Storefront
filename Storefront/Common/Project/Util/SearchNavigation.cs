//-----------------------------------------------------------------------
// <copyright file="SearchNavigation.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the SearchNavigation class.</summary>
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

namespace Sitecore.Reference.Storefront
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.ContentSearch.Linq.Utilities;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.ContentSearch.Security;
    using Sitecore.ContentSearch.Utilities;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Sitecore.Commerce.Entities.Inventory;
    using System.Collections.Generic;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Search;

    /// <summary>
    /// Static helper class to aid with search for navigation
    /// </summary>
    public static class SearchNavigation
    {
        /// <summary>
        /// Gets the current language based off of the sitecore context
        /// </summary>
        public static string CurrentLanguageName
        {
            get
            {
                return Sitecore.Context.Language.Name;
            }
        }

        /// <summary>
        /// Returns the navigation categories based on a root navigation ID identified by a Data Source string.
        /// </summary>
        /// <param name="navigationDataSource">A Sitecore Item ID or query that identifies the root navigation ID.</param>
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>A list of category items</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "All classes are required.")]
        public static SearchResponse GetNavigationCategories(string navigationDataSource, CommerceSearchOptions searchOptions)
        {
            ID navigationId;
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            if (navigationDataSource.IsGuid())
            {
                navigationId = ID.Parse(navigationDataSource);
            }
            else
            {
                using (var context = searchIndex.CreateSearchContext())
                {
                    var query = LinqHelper.CreateQuery<Sitecore.ContentSearch.SearchTypes.SitecoreUISearchResultItem>(context, SearchStringModel.ParseDatasourceString(navigationDataSource))
                        .Select(result => result.GetItem().ID);
                    if (query != null & query.Any())
                    {
                        navigationId = query.First();
                    }
                    else
                    {
                        return new SearchResponse();
                    }
                }
            }

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceBaseCatalogSearchResultItem>()
                   .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Category)
                   .Where(item => item.Language == CurrentLanguageName)
                   .Where(item => item.CommerceAncestorIds.Contains(navigationId))
                    .Select(p => new CommerceBaseCatalogSearchResultItem()
                    {
                        ItemId = p.ItemId,
                        Uri = p.Uri
                    });

                searchResults = searchManager.AddSearchOptionsToQuery<CommerceBaseCatalogSearchResultItem>(searchResults, searchOptions);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Gets a category based on its name
        /// </summary>
        /// <param name="categoryName">The name of the category</param>
        /// <param name="catalogName">The name of the catalog containing the category</param>        
        /// <returns>The found category item, or null if not found</returns>
        public static Item GetCategory(string categoryName, string catalogName)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(catalogName, "catalogName");

            Item result = null;
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex(catalogName);

            using (var context = searchIndex.CreateSearchContext())
            {
                var categoryQuery = context.GetQueryable<CommerceBaseCatalogSearchResultItem>()
                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Category)
                    .Where(item => item.Language == CurrentLanguageName)
                    .Where(item => (item.Name == categoryName && item.CatalogName == catalogName) || (item.Name == categoryName))
                    .Select(p => new CommerceBaseCatalogSearchResultItem()
                    {
                        ItemId = p.ItemId,
                        Uri = p.Uri
                    })
                    .Take(1);

                var foundSearchItem = categoryQuery.FirstOrDefault();
                if (foundSearchItem != null)
                {
                    result = foundSearchItem.GetItem();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a product based on its product id
        /// </summary>
        /// <param name="productId">The product's id</param> 
        /// <param name="catalogName">The name of the catalog containing the product</param>		       
        /// <returns>The found product item, or null if not found</returns>
        public static Item GetProduct(string productId, string catalogName)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(catalogName, "catalogName");

            Item result = null;
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex(catalogName);

            using (var context = searchIndex.CreateSearchContext())
            {
                var productQuery = context.GetQueryable<CommerceProductSearchResultItem>()
                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                    .Where(item => item.CatalogName == catalogName)
                    .Where(item => item.Language == CurrentLanguageName)
                    .Where(item => item.CatalogItemId == productId.ToLowerInvariant())
                    .Select(p => new CommerceProductSearchResultItem()
                    {
                        ItemId = p.ItemId,
                        Uri = p.Uri
                    })
                    .Take(1);

                var foundSearchItem = productQuery.FirstOrDefault();
                if (foundSearchItem != null)
                {
                    result = foundSearchItem.GetItem();
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a search to retrieve catalog items.
        /// </summary>
        /// <param name="defaultBucketQuery">The search default bucket query value.</param>
        /// <param name="persistentBucketFilter">The search persistent bucket filter value.</param>
        /// <param name="searchOptions">The search options.</param>
        /// <returns>A list of catalog items.</returns>
        public static SearchResponse SearchCatalogItems(string defaultBucketQuery, string persistentBucketFilter, CommerceSearchOptions searchOptions)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            var defaultQuery = defaultBucketQuery.Replace("&", ";");
            var persistentQuery = persistentBucketFilter.Replace("&", ";");
            var combinedQuery = CombineQueries(persistentQuery, defaultQuery);
            var searchStringModel = SearchStringModel.ParseDatasourceString(combinedQuery);

            using (var context = searchIndex.CreateSearchContext(SearchSecurityOptions.EnableSecurityCheck))
            {
                var query = LinqHelper.CreateQuery<Sitecore.ContentSearch.SearchTypes.SitecoreUISearchResultItem>(context, searchStringModel)
                    .Where(item => item.Language == SearchNavigation.CurrentLanguageName);

                query = searchManager.AddSearchOptionsToQuery(query, searchOptions);

                var results = query.GetResults();
                var response = SearchResponse.CreateFromUISearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Executes a search in a bucket to retrieve catalog items.
        /// </summary>
        /// <param name="defaultBucketQuery">The search default bucket query value.</param>
        /// <param name="persistentBucketFilter">The search persistent bucket filter value.</param>
        /// <returns>A list of catalog items.</returns>
        public static SearchResponse SearchBucketForCatalogItems(string defaultBucketQuery, string persistentBucketFilter)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            var defaultQuery = defaultBucketQuery.Replace("&", ";");
            var persistentQuery = persistentBucketFilter.Replace("&", ";");
            var combinedQuery = CombineQueries(persistentQuery, defaultQuery);
            var searchStringModel = SearchStringModel.ParseDatasourceString(combinedQuery);

            using (var context = searchIndex.CreateSearchContext(SearchSecurityOptions.EnableSecurityCheck))
            {
                var query = LinqHelper.CreateQuery<Sitecore.ContentSearch.SearchTypes.SitecoreUISearchResultItem>(context, searchStringModel)
                    .Where(item => item.Language == SearchNavigation.CurrentLanguageName);

                var results = query.GetResults();
                var response = SearchResponse.CreateFromUISearchResultsItems(new CommerceSearchOptions(), results);

                return response;
            }
        }

        /// <summary>
        /// Gets the products with the passed in productid
        /// </summary>
        /// <param name="productId">The category name</param>
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>A list of child products</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByProduct")]
        public static SearchResponse GetProductByProductId(string productId, CommerceSearchOptions searchOptions)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceProductSearchResultItem>()
                                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                                    .Where(item => item.Language == CurrentLanguageName)
                                    .Where(item => item.Name == productId)
                                    .Select(p => new CommerceProductSearchResultItem()
                                    {
                                        ItemId = p.ItemId,
                                        Uri = p.Uri
                                    });

                searchResults = searchManager.AddSearchOptionsToQuery<CommerceProductSearchResultItem>(searchResults, searchOptions);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Gets all the products under a specific category
        /// </summary>
        /// <param name="categoryId">The category name</param>
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>A list of child products</returns>
        public static SearchResponse GetCategoryProducts(ID categoryId, CommerceSearchOptions searchOptions)
        {
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceProductSearchResultItem>()
                                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                                    .Where(item => item.Language == CurrentLanguageName)
                                    .Where(item => item.CommerceAncestorIds.Contains(categoryId))
                                    .Select(p => new CommerceProductSearchResultItem()
                                    {
                                        ItemId = p.ItemId,
                                        Uri = p.Uri
                                    });

                searchResults = searchManager.AddSearchOptionsToQuery<CommerceProductSearchResultItem>(searchResults, searchOptions);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Searches for catalog items based on keyword
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <param name="catalogName">The name of the catalog containing the keyword</param>		
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>A list of child products</returns>
        public static SearchResponse SearchCatalogItemsByKeyword(string keyword, string catalogName, CommerceSearchOptions searchOptions)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(catalogName, "catalogName");
            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex(catalogName);

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceProductSearchResultItem>()
                    .Where(item => item.Name.Equals(keyword) || item["_displayname"].Equals(keyword) || item.Content.Contains(keyword))
                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product || item.CommerceSearchItemType == CommerceSearchResultItemType.Category)
                    .Where(item => item.CatalogName == catalogName)
                    .Where(item => item.Language == CurrentLanguageName)
                    .Select(p => new CommerceProductSearchResultItem()
                    {
                        ItemId = p.ItemId,
                        Uri = p.Uri
                    });

                searchResults = searchManager.AddSearchOptionsToQuery<CommerceProductSearchResultItem>(searchResults, searchOptions);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Searches for site content items based on keyword
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>
        /// A list of child products
        /// </returns>
        public static SearchResponse SearchSiteByKeyword(string keyword, CommerceSearchOptions searchOptions)
        {
            const string IndexNameFormat = "sitecore_{0}_index";
            string indexName = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                IndexNameFormat,
                Sitecore.Context.Database.Name);

            var searchIndex = ContentSearchManager.GetIndex(indexName);
            using (var context = searchIndex.CreateSearchContext())
            {
                //var rootSearchPath = Sitecore.IO.FileUtil.MakePath(Sitecore.Context.Site.ContentStartPath, "Home", '/');
                var searchResults = context.GetQueryable<SearchResultItem>();
                searchResults = searchResults.Where(item => item.Path.StartsWith(Sitecore.Context.Site.ContentStartPath));
                searchResults = searchResults.Where(item => item[StorefrontConstants.KnownIndexFields.SiteContentItem] == "1");
                searchResults = searchResults.Where(item => item.Language == CurrentLanguageName);
                searchResults = searchResults.Where(GetContentExpression(keyword));
                searchResults = searchResults.Page(searchOptions.StartPageIndex, searchOptions.NumberOfItemsToReturn);

                var results = searchResults.GetResults();
                var response = SearchResponse.CreateFromSearchResultsItems(searchOptions, results);

                return response;
            }
        }

        /// <summary>
        /// Gets all the products under a specific category
        /// </summary>
        /// <param name="categoryId">The parent category id</param>
        /// <param name="searchOptions">The paging options for this query</param>
        /// <returns>A list of child products</returns>
        public static CategorySearchResults GetCategoryChildCategories(ID categoryId, CommerceSearchOptions searchOptions)
        {
            List<Item> childCategoryList = new List<Item>();

            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            using (var context = searchIndex.CreateSearchContext())
            {
                var searchResults = context.GetQueryable<CommerceBaseCatalogSearchResultItem>()
                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Category)
                    .Where(item => item.Language == CurrentLanguageName)
                    .Where(item => item.ItemId == categoryId)
                    .Select(p => p);

                var list = searchResults.ToList();
                if (list.Count > 0)
                {
                    if (list[0].Fields.ContainsKey(StorefrontConstants.KnownIndexFields.ChildCategoriesSequence))
                    {
                        var childCategoryDelimitedString = list[0][StorefrontConstants.KnownIndexFields.ChildCategoriesSequence];

                        string[] categoryIdArray = childCategoryDelimitedString.Split('|');

                        foreach (var childCategoryId in categoryIdArray)
                        {
                            var childCategoryItem = Sitecore.Context.Database.GetItem(ID.Parse(childCategoryId));
                            if (childCategoryItem != null)
                            {
                                childCategoryList.Add(childCategoryItem);
                            }
                        }
                    }
                }
            }

            return new CategorySearchResults(childCategoryList, childCategoryList.Count, 1, 1, new List<FacetCategory>());
        }

        /// <summary>
        /// Gets the index of the product stock status from.
        /// </summary>
        /// <param name="viewModelList">The view model list.</param>
        public static void GetProductStockStatusFromIndex(List<ProductViewModel> viewModelList)
        {
            if (viewModelList == null || viewModelList.Count == 0)
            {
                return;
            }

            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchIndex = searchManager.GetIndex();

            using (var context = searchIndex.CreateSearchContext())
            {
                var predicate = PredicateBuilder.Create<InventorySearchResultItem>(item => item[StorefrontConstants.KnownIndexFields.InStockLocations].Contains("Default"));
                predicate = predicate.Or(item => item[StorefrontConstants.KnownIndexFields.OutOfStockLocations].Contains("Default"));
                predicate = predicate.Or(item => item[StorefrontConstants.KnownIndexFields.OrderableLocations].Contains("Default"));
                predicate = predicate.Or(item => item[StorefrontConstants.KnownIndexFields.PreOrderable].Contains("0"));

                var searchResults = context.GetQueryable<InventorySearchResultItem>()
                    .Where(item => item.CommerceSearchItemType == CommerceSearchResultItemType.Product)
                    .Where(item => item.Language == SearchNavigation.CurrentLanguageName)
                    .Where(BuildProductIdListPredicate(viewModelList))
                    .Where(predicate)
                    .Select(x => new { x.OutOfStockLocations, x.OrderableLocations, x.PreOrderable, x.InStockLocations, x.Fields, x.Name });

                var results = searchResults.GetResults();
                if (results.TotalSearchResults == 0)
                {
                    return;
                }

                foreach (var result in results)
                {
                    var resultDocument = result.Document;
                    if (resultDocument == null)
                    {
                        continue;
                    }

                    StockStatus status = null;

                    var isInStock = resultDocument.Fields.ContainsKey(StorefrontConstants.KnownIndexFields.InStockLocations)
                                    && resultDocument.Fields[StorefrontConstants.KnownIndexFields.InStockLocations] != null;
                    if (isInStock)
                    {
                        status = StockStatus.InStock;
                    }
                    else
                    {
                        var isPreOrderable = resultDocument.Fields.ContainsKey(StorefrontConstants.KnownIndexFields.PreOrderable)
                                             && result.Document.PreOrderable != null
                                             && (result.Document.PreOrderable.Equals("1", StringComparison.OrdinalIgnoreCase)
                                                || result.Document.PreOrderable.Equals("true", StringComparison.OrdinalIgnoreCase));
                        if (isPreOrderable)
                        {
                            status = StockStatus.PreOrderable;
                        }
                        else
                        {
                            var isOutOfStock = resultDocument.Fields.ContainsKey(StorefrontConstants.KnownIndexFields.OutOfStockLocations)
                                               && result.Document.OutOfStockLocations != null;
                            var isBackOrderable = resultDocument.Fields.ContainsKey(StorefrontConstants.KnownIndexFields.OrderableLocations)
                                                  && result.Document.OrderableLocations != null;
                            if (isOutOfStock && isBackOrderable)
                            {
                                status = StockStatus.BackOrderable;
                            }
                            else
                            {
                                status = isOutOfStock ? StockStatus.OutOfStock : null;
                            }
                        }
                    }

                    var foundModel = viewModelList.Find(x => x.ProductId == result.Document.Name);
                    if (foundModel != null)
                    {
                        foundModel.StockStatus = status;
                        foundModel.StockStatusName = StorefrontManager.GetProductStockStatusName(foundModel.StockStatus);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the product identifier list predicate.
        /// </summary>
        /// <param name="viewModelList">The view model list.</param>
        /// <returns>The search predicate ORing the product ids.</returns>
        private static Expression<Func<InventorySearchResultItem, bool>> BuildProductIdListPredicate(List<ProductViewModel> viewModelList)
        {
            Expression<Func<InventorySearchResultItem, bool>> predicate = null;

            bool isFirst = true;
            foreach (var viewModel in viewModelList)
            {
                if (isFirst)
                {
                    predicate = PredicateBuilder.Create<InventorySearchResultItem>(p => p.CatalogItemId == viewModel.ProductId.ToLowerInvariant());
                }
                else
                {
                    predicate = predicate.Or(p => p.CatalogItemId == viewModel.ProductId.ToLowerInvariant());
                }

                isFirst = false;
            }

            return predicate;
        }

        /// <summary>
        /// Combines multiple string queries
        /// </summary>
        /// <param name="query1">The first query</param>
        /// <param name="query2">The second query</param>
        /// <returns>Both queries combined</returns>
        private static string CombineQueries(string query1, string query2)
        {
            if (!string.IsNullOrWhiteSpace(query1) && !string.IsNullOrWhiteSpace(query2))
            {
                return string.Concat(query1, ";", query2);
            }
            else if (!string.IsNullOrWhiteSpace(query1))
            {
                return query1;
            }
            else
            {
                return query2;
            }
        }

        /// <summary>
        /// Gets the site content search expression that will be used to select results based on a user input search phrase.
        /// </summary>
        /// <param name="searchPhrase">The search phrase entered by the user.</param>
        /// <returns>An expression that represents the search phrase entered by the user.</returns>
        private static Expression<Func<SearchResultItem, bool>> GetContentExpression(string searchPhrase)
        {
            if (!string.IsNullOrWhiteSpace(searchPhrase))
            {
                Expression<Func<SearchResultItem, bool>> predicate = null;
                var termList = searchPhrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var term in termList)
                {
                    if (predicate == null)
                    {
                        predicate = PredicateBuilder.Create<SearchResultItem>(item => item.Content.Contains(term));
                    }
                    else
                    {
                        predicate = predicate.And(item => item.Content.Contains(term));
                    }
                }

                return predicate;
            }

            return PredicateBuilder.False<SearchResultItem>();
        }
    }
}
