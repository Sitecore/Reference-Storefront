//-----------------------------------------------------------------------
// <copyright file="CatalogManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CategoryViewModel class.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog.Fields;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Entities.Prices;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Models.RenderingModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Reference.Storefront.Services;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// CatalogManager class
    /// </summary>  
    public class CatalogManager : BaseManager
    {
        private Catalog _currentCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogManager" /> class.
        /// </summary>
        /// <param name="catalogServiceProvider">The catalog service provider.</param>
        /// <param name="pricingManager">The pricing manager.</param>
        /// <param name="inventoryManager">The inventory manager.</param>
        public CatalogManager([NotNull] CatalogServiceProvider catalogServiceProvider, [NotNull] PricingManager pricingManager, [NotNull] InventoryManager inventoryManager)
        {
            Assert.ArgumentNotNull(catalogServiceProvider, "catalogServiceProvider");
            Assert.ArgumentNotNull(pricingManager, "pricingManager");
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");

            this.CatalogServiceProvider = catalogServiceProvider;
            this.PricingManager = pricingManager;
            this.InventoryManager = inventoryManager;
        }

        /// <summary>
        /// Gets or sets the catalog service provider.
        /// </summary>
        public CatalogServiceProvider CatalogServiceProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        /// <summary>
        /// Gets or sets the pricing manager.
        /// </summary>
        /// <value>
        /// The pricing manager.
        /// </value>
        public PricingManager PricingManager { get; protected set; }

        /// <summary>
        /// Gets the current catalog being accessed
        /// </summary>
        public Catalog CurrentCatalog
        {
            get
            {
                if (_currentCatalog == null)
                {
                    _currentCatalog = StorefrontManager.CurrentStorefront.DefaultCatalog;
                    if (_currentCatalog == null)
                    {
                        //There was no catalog associated with the storefront or we are not using multi-storefront
                        //So we use the default catalog
                        _currentCatalog = new Catalog(Context.Database.GetItem(CommerceConstants.KnownItemIds.DefaultCatalog));
                    }
                }

                return _currentCatalog;
            }
        }

        /// <summary>
        /// The <see cref="RelatedCatalogItemsViewModel" /> representing the related catalog items.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="catalogItem">The catalog item.</param>
        /// <param name="rendering">The target renering.</param>
        /// <returns>
        /// The related catalog item view model.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Derived class is prefered.")]
        public RelatedCatalogItemsViewModel GetRelationshipsFromItem([NotNull] CommerceStorefront storefront, Item catalogItem, Rendering rendering)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            if (catalogItem != null &&
                catalogItem.Fields.Contains(CommerceConstants.KnownFieldIds.RelationshipList) &&
                !string.IsNullOrEmpty(catalogItem[CommerceConstants.KnownFieldIds.RelationshipList]))
            {
                var field = new RelationshipField(catalogItem.Fields[CommerceConstants.KnownFieldIds.RelationshipList]);
                if (rendering != null &&
                    !string.IsNullOrWhiteSpace(rendering.RenderingItem.InnerItem["RelationshipsToDisplay"]))
                {
                    var relationshipsToDisplay = rendering.RenderingItem.InnerItem["RelationshipsToDisplay"].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    return this.GetRelationshipsFromField(storefront, field, rendering, relationshipsToDisplay);
                }
                else
                {
                    return this.GetRelationshipsFromField(storefront, field, rendering);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a lists of target items from a relationship field
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="field">the relationship field</param>
        /// <param name="rendering">The target renering.</param>
        /// <returns>
        /// a list of relationship targets or null if no items found
        /// </returns>
        public RelatedCatalogItemsViewModel GetRelationshipsFromField([NotNull] CommerceStorefront storefront, RelationshipField field, Rendering rendering)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            return GetRelationshipsFromField(storefront, field, rendering, null);
        }

        /// <summary>
        /// Gets a lists of target items from a relationship field
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="field">the relationship field</param>
        /// <param name="rendering">The target renering.</param>
        /// <param name="relationshipNames">the names of the relationships, to retrieve (for example upsell).</param>
        /// <returns>
        /// a list of relationship targets or null if no items found
        /// </returns>
        public RelatedCatalogItemsViewModel GetRelationshipsFromField([NotNull] CommerceStorefront storefront, RelationshipField field, Rendering rendering, IEnumerable<string> relationshipNames)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            relationshipNames = relationshipNames ?? Enumerable.Empty<string>();
            relationshipNames = relationshipNames.Select(s => s.Trim());
            var model = new RelatedCatalogItemsViewModel();

            if (field != null)
            {
                var productRelationshipInfoList = field.GetProductRelationships();
                var productModelList = this.GroupRelationshipsByDescription(storefront, field, relationshipNames, productRelationshipInfoList, rendering);
                model.RelatedProducts.AddRange(productModelList);

                var categoryRelationshipInfoList = field.GetCategoryRelationships();
                var categoryModelList = this.GroupRelationshipsByDescription(storefront, field, relationshipNames, categoryRelationshipInfoList, rendering);
                model.RelatedCategories.AddRange(categoryModelList);
            }

            model.Initialize(rendering);

            return model;
        }

        /// <summary>
        /// Registers an event specifying that the category page has been visited.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="categoryName">The category name.</param>
        /// <param name="catalogName">The catalog name.</param>
        /// <returns>
        /// A <see cref="VisitedCategoryPageResult" /> specifying the result of the service request.
        /// </returns>
        public VisitedCategoryPageResult VisitedCategoryPage([NotNull] CommerceStorefront storefront, [NotNull] string categoryName, string catalogName)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new VisitedCategoryPageRequest(storefront.ShopName, categoryName);
            request.CatalogName = catalogName;
            return this.CatalogServiceProvider.VisitedCategoryPage(request);
        }

        /// <summary>
        /// Registers an event specifying that the product details page has been visited.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="productId">the product id.</param>
        /// <param name="parentCategoryName">the parent category name.</param>
        /// <param name="catalogName">the catalog name.</param>
        /// <returns>
        /// A <see cref="VisitedProductDetailsPageResult" /> specifying the result of the service request.
        /// </returns>
        public VisitedProductDetailsPageResult VisitedProductDetailsPage([NotNull] CommerceStorefront storefront, [NotNull] string productId, string parentCategoryName, string catalogName)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new VisitedProductDetailsPageRequest(storefront.ShopName, productId);
            request.ParentCategoryName = parentCategoryName;
            request.CatalogName = catalogName;
            return this.CatalogServiceProvider.VisitedProductDetailsPage(request);
        }

        /// <summary>
        /// This method returns the ProductSearchResults for a datasource
        /// </summary>
        /// <param name="dataSource">The datasource to perform the search with</param>
        /// <param name="productSearchOptions">The search options.</param>
        /// <returns>A ProductSearchResults</returns>     
        public SearchResults GetProductSearchResults(Item dataSource, CommerceSearchOptions productSearchOptions)
        {
            Assert.ArgumentNotNull(productSearchOptions, "productSearchOptions");

            if (dataSource != null)
            {
                int totalProductCount = 0;
                int totalPageCount = 0;
                string error = string.Empty;

                if (dataSource.TemplateName == "Commerce Named Search" || dataSource.TemplateName == "Named Search")
                {
                    var returnList = new List<Item>();
                    IEnumerable<CommerceQueryFacet> facets = null;
                    var searchOptions = new CommerceSearchOptions(10, 0);
                    var defaultBucketQuery = dataSource[CommerceConstants.KnownSitecoreFieldNames.DefaultBucketQuery];
                    var persistendBucketFilter = CleanLanguageFromFilter(dataSource[CommerceConstants.KnownSitecoreFieldNames.PersistentBucketFilter]);

                    try
                    {
                        var searchResponse = SearchNavigation.SearchCatalogItems(defaultBucketQuery, persistendBucketFilter, searchOptions);

                        if (searchResponse != null)
                        {
                            returnList.AddRange(searchResponse.ResponseItems);

                            totalProductCount = searchResponse.TotalItemCount;
                            totalPageCount = searchResponse.TotalPageCount;
                            facets = searchResponse.Facets;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }

                    return new SearchResults(returnList, totalProductCount, totalPageCount, searchOptions.StartPageIndex, facets);
                }

                var childProducts = GetChildProducts(productSearchOptions, dataSource).SearchResultItems;
                return new SearchResults(childProducts, totalProductCount, totalPageCount, productSearchOptions.StartPageIndex, new List<CommerceQueryFacet>());
            }

            return null;
        }

        /// <summary>
        /// This method returns a list of ProductSearchResults for a datasource
        /// </summary>
        /// <param name="dataSource">The datasource to perform the searches with</param>
        /// <param name="productSearchOptions">The search options.</param>
        /// <returns>A list of ProductSearchResults</returns>       
        public MultipleProductSearchResults GetMultipleProductSearchResults(BaseItem dataSource, CommerceSearchOptions productSearchOptions)
        {
            Assert.ArgumentNotNull(productSearchOptions, "productSearchOptions");

            MultilistField searchesField = dataSource.Fields["Named Searches"];
            var searches = searchesField.GetItems();
            var productsSearchResults = new List<SearchResults>();
            foreach (Item search in searches)
            {
                var productsSearchResult = GetProductSearchResults(search, productSearchOptions);
                if (productsSearchResult != null)
                {
                    productsSearchResult.DisplayName = search["title"];
                    productsSearchResults.Add(productsSearchResult);
                }
            }

            return new MultipleProductSearchResults(productsSearchResults);
        }

        /// <summary>
        /// This method returns the current category by URL
        /// </summary>
        /// <returns>The category.</returns>
        public Category GetCurrentCategoryByUrl()
        {
            Category currentCategory;

            var categoryId = CatalogUrlManager.ExtractItemIdFromCurrentUrl();

            string virtualCategoryCacheKey = string.Format(CultureInfo.InvariantCulture, "VirtualCategory_{0}", categoryId);

            if (CurrentSiteContext.Items.Contains(virtualCategoryCacheKey))
            {
                currentCategory = CurrentSiteContext.Items[virtualCategoryCacheKey] as Category;
            }
            else
            {
                currentCategory = GetCategory(categoryId);
                CurrentSiteContext.Items.Add(virtualCategoryCacheKey, currentCategory);
            }

            return currentCategory;
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns>The category.</returns>
        public Category GetCategory(string categoryId) 
        {
            var categoryItem = SearchNavigation.GetCategory(categoryId, CurrentCatalog.Name);
            return GetCategory(categoryItem);
        }

        /// <summary>
        /// Get category by item
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The catgory.</returns>
        public Category GetCategory(Item item)
        {
            var category = new Category(item)
            {
                RequiredFacets = CurrentSearchManager.GetFacetFieldsForItem(item).ToList(),
                SortFields = CurrentSearchManager.GetSortFieldsForItem(item).ToList(),
                ItemsPerPage = CurrentSearchManager.GetItemsPerPageForItem(item)
            };

            return category;
        }

        /// <summary>
        /// Gets the product price.
        /// </summary>
        /// <param name="productViewModel">The product view model.</param>
        public virtual void GetProductPrice(ProductViewModel productViewModel)
        {
            if (productViewModel == null)
            {
                return;
            }

            bool includeVariants = productViewModel.Variants != null && productViewModel.Variants.Count > 0;
            var pricesResponse = this.PricingManager.GetProductPrices(StorefrontManager.CurrentStorefront, productViewModel.CatalogName, productViewModel.ProductId, includeVariants, null);
            if (pricesResponse != null && pricesResponse.ServiceProviderResult.Success && pricesResponse.Result != null)
            {
                Price price;
                Dictionary<string, Price> prices;
                if (pricesResponse.Result.TryGetValue(productViewModel.ProductId, out prices))
                {                    
                    if (prices.TryGetValue("List", out price))
                    {
                        productViewModel.ListPrice = price.Amount;
                    }

                    if (prices.TryGetValue("Adjusted", out price))
                    {
                        productViewModel.AdjustedPrice = price.Amount;
                    }
                }

                if (includeVariants)
                {
                    foreach (var variant in productViewModel.Variants)
                    {
                        if (pricesResponse.Result.TryGetValue(variant.VariantId, out prices))
                        {
                            if (prices.TryGetValue("List", out price))
                            {
                                variant.ListPrice = price.Amount;
                            }

                            if (prices.TryGetValue("Adjusted", out price))
                            {
                                variant.AdjustedPrice = price.Amount;
                            }
                        }
                    }
                }                
            } 
        }

        /// <summary>
        /// Gets the product price.
        /// </summary>
        /// <param name="productViewModels">The product view models.</param>
        public virtual void GetProductBulkPrices(List<ProductViewModel> productViewModels)
        {
            if (productViewModels == null || !productViewModels.Any())
            {
                return;
            }

            var catalogName = productViewModels.Select(p => p.CatalogName).First().ToString();
            var productIds = productViewModels.Select(p => p.ProductId).ToList();           

            var pricesResponse = this.PricingManager.GetProductBulkPrices(StorefrontManager.CurrentStorefront, catalogName, productIds, null);
            var prices = pricesResponse != null && pricesResponse.Result != null ? pricesResponse.Result : new Dictionary<string, Dictionary<string, Price>>();

            foreach (var productViewModel in productViewModels)
            {
                Price price;
                Dictionary<string, Price> productPrices;
                if (prices.Any() && prices.TryGetValue(productViewModel.ProductId, out productPrices))
                {
                    if (productPrices.TryGetValue("List", out price))
                    {
                        productViewModel.ListPrice = price.Amount;
                    }

                    if (productPrices.TryGetValue("Adjusted", out price))
                    {
                        productViewModel.AdjustedPrice = price.Amount;
                    }
                }     
            }
        }

        /// <summary>
        /// Gets the product rating.
        /// </summary>
        /// <param name="productItem">The product item.</param>
        /// <returns>The product rating</returns>
        public virtual decimal GetProductRating(Item productItem)
        {
            var ratingString = productItem["Rating"];
            decimal rating;
            if (decimal.TryParse(ratingString, out rating))
            {
                return rating;
            }

            return 0;
        }

        /// <summary>
        /// Visiteds the product details page.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <returns>
        /// The manager response
        /// </returns>
        public virtual ManagerResponse<VisitedProductDetailsPageResult, bool> VisitedProductDetailsPage([NotNull] CommerceStorefront storefront)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new VisitedProductDetailsPageRequest(storefront.ShopName, CatalogUrlManager.ExtractItemIdFromCurrentUrl())
            {
                ParentCategoryName = CatalogUrlManager.ExtractCategoryNameFromCurrentUrl(),
                CatalogName = CatalogUrlManager.ExtractCatalogNameFromCurrentUrl()
            };

            var result = this.CatalogServiceProvider.VisitedProductDetailsPage(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<VisitedProductDetailsPageResult, bool>(result, result.Success);
        }

        /// <summary>
        /// Visiteds the category page.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <returns>
        /// The manager response
        /// </returns>
        public virtual ManagerResponse<VisitedCategoryPageResult, bool> VisitedCategoryPage([NotNull] CommerceStorefront storefront)
        {
            Assert.ArgumentNotNull(storefront, "storefront");

            var request = new VisitedCategoryPageRequest(storefront.ShopName, CatalogUrlManager.ExtractItemIdFromCurrentUrl())
            {
                CatalogName = CatalogUrlManager.ExtractCatalogNameFromCurrentUrl()
            };

            var result = this.CatalogServiceProvider.VisitedCategoryPage(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<VisitedCategoryPageResult, bool>(result, result.Success);
        }

        /// <summary>
        /// Registers the search event.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="numberOfHits">The number of hits.</param>
        /// <returns>
        /// The manager response
        /// </returns>
        public virtual ManagerResponse<SearchInitiatedResult, bool> RegisterSearchEvent([NotNull] CommerceStorefront storefront, string searchKeyword, int numberOfHits)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNullOrEmpty(searchKeyword, "searchKeyword");

            var request = new SearchInitiatedRequest(storefront.ShopName, searchKeyword, numberOfHits);
            var result = this.CatalogServiceProvider.SearchInitiated(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<SearchInitiatedResult, bool>(result, result.Success);
        }
        
        #region Protected helper methods

        /// <summary>
        /// This method returns child products for this category
        /// </summary>
        /// <param name="searchOptions">The options to perform the search with</param>
        /// <param name="categoryItem">The category item whose children to retrieve</param>
        /// <returns>A list of child products</returns>        
        protected SearchResults GetChildProducts(CommerceSearchOptions searchOptions, Item categoryItem)
        {
            IEnumerable<CommerceQueryFacet> facets = null;
            var returnList = new List<Item>();
            var totalPageCount = 0;
            var totalProductCount = 0;

            if (RenderingContext.Current.Rendering.Item != null)
            {
                SearchResponse searchResponse = null;
                if (CatalogUtility.IsItemDerivedFromCommerceTemplate(categoryItem, CommerceConstants.KnownTemplateIds.CommerceDynamicCategoryTemplate) || categoryItem.TemplateName == "Commerce Named Search")
                {
                    try
                    {
                        var defaultBucketQuery = categoryItem[CommerceConstants.KnownSitecoreFieldNames.DefaultBucketQuery];
                        var persistendBucketFilter = categoryItem[CommerceConstants.KnownSitecoreFieldNames.PersistentBucketFilter];
                        persistendBucketFilter = CleanLanguageFromFilter(persistendBucketFilter);
                        searchResponse = SearchNavigation.SearchCatalogItems(defaultBucketQuery, persistendBucketFilter, searchOptions);
                    }
                    catch (Exception ex)
                    {
                        Helpers.LogException(ex, this);
                    }
                }
                else
                {
                    searchResponse = SearchNavigation.GetCategoryProducts(categoryItem.ID, searchOptions);
                }

                if (searchResponse != null)
                {
                    returnList.AddRange(searchResponse.ResponseItems);

                    totalProductCount = searchResponse.TotalItemCount;
                    totalPageCount = searchResponse.TotalPageCount;
                    facets = searchResponse.Facets;
                }
            }

            var results = new SearchResults(returnList, totalProductCount, totalPageCount, searchOptions.StartPageIndex, facets);
            return results;
        }

        /// <summary>
        /// Cleans the language from filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The updated filter.</returns>
        protected string CleanLanguageFromFilter(string filter)
        {
            if (filter.IndexOf("language:", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return filter;
            }

            var newFilter = new StringBuilder();

            var statementList = filter.Split(';');
            foreach (var statement in statementList)
            {
                if (statement.IndexOf("language", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }

                if (newFilter.Length > 0)
                {
                    newFilter.Append(';');
                }

                newFilter.Append(statement);
            }

            return newFilter.ToString();
        }

        /// <summary>
        /// Groups the provided relationships by name, and converts them into a list of <see cref="CategoryViewModel" /> objects.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="field">The relationship field.</param>
        /// <param name="relationshipNames">The names of the relationships to retrieve.</param>
        /// <param name="productRelationshipInfoList">The list of rerlationships to group and convert.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>
        /// The grouped relationships converted into a list of <see cref="CategoryViewModel" /> objects.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Specific type is desired here.")]
        protected IEnumerable<CategoryViewModel> GroupRelationshipsByDescription([NotNull] CommerceStorefront storefront, RelationshipField field, IEnumerable<string> relationshipNames, IEnumerable<CatalogRelationshipInformation> productRelationshipInfoList, Rendering rendering)
        {
            Dictionary<string, CategoryViewModel> relationshipGroups = new Dictionary<string, CategoryViewModel>(StringComparer.OrdinalIgnoreCase);

            if (field != null && productRelationshipInfoList != null)
            {
                foreach (var relationshipInfo in productRelationshipInfoList)
                {
                    if (!relationshipNames.Any() || relationshipNames.Contains(relationshipInfo.RelationshipName, StringComparer.OrdinalIgnoreCase))
                    {
                        var relationshipDescription = string.IsNullOrWhiteSpace(relationshipInfo.RelationshipDescription) ? relationshipInfo.RelationshipName : relationshipInfo.RelationshipDescription;
                        CategoryViewModel categoryModel = null;
                        if (!relationshipGroups.TryGetValue(relationshipDescription, out categoryModel))
                        {
                            categoryModel = new CategoryViewModel
                            {
                                ChildProducts = new List<ProductViewModel>(),
                                RelationshipName = relationshipInfo.RelationshipName,
                                RelationshipDescription = relationshipDescription
                            };

                            relationshipGroups[relationshipDescription] = categoryModel;
                        }

                        var targetItemId = ID.Parse(relationshipInfo.ToItemExternalId);
                        var targetItem = field.InnerField.Database.GetItem(targetItemId);
                        var productModel = new ProductViewModel(targetItem);
                        productModel.Initialize(rendering);

                        this.GetProductRating(targetItem);

                        categoryModel.ChildProducts.Add(productModel);
                    }
                }
            }

            if (relationshipGroups.Count > 0)
            {
                List<ProductViewModel> productViewModelList = new List<ProductViewModel>();

                foreach (string key in relationshipGroups.Keys)
                {
                    CategoryViewModel viewModel = relationshipGroups[key];
                    var childProducts = viewModel.ChildProducts;
                    if (childProducts != null && childProducts.Count > 0)
                    {
                        productViewModelList.AddRange(childProducts);
                    }
                }

                if (productViewModelList.Count > 0)
                {
                    this.GetProductBulkPrices(productViewModelList);
                    this.InventoryManager.GetProductsStockStatus(storefront, productViewModelList);
                }
            }

            return relationshipGroups.Values;
        }

       #endregion
    }
}