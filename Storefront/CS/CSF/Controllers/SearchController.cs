//-----------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SearchController class.</summary>
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

namespace Sitecore.Reference.Storefront.Controllers
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Models.RenderingModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Presentation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Manages all search related requests
    /// </summary>
    public class SearchController : BaseController
    {
        #region Variables

        private const string CurrentCategoryViewModelKeyName = "CurrentCategoryViewModel";
        private const string CurrentSearchProductResultsKeyName = "CurrentSearchProductResults";
        private const string CurrentSearchContentResultsKeyName = "CurrentSearchContentResults";
        private const string CurrentSearchInfoKeyName = "CurrentSearchInfo";

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="catalogManager">The catalog manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public SearchController([NotNull] CatalogManager catalogManager, [NotNull] ContactFactory contactFactory)
            : base(contactFactory)
        {
            Assert.ArgumentNotNull(catalogManager, "catalogManager");
            Assert.ArgumentNotNull(contactFactory, "contactFactory");

            this.CatalogManager = catalogManager;
        }

        /// <summary>
        /// Gets or sets the catalog manager.
        /// </summary>
        /// <value>
        /// The catalog manager.
        /// </value>
        public CatalogManager CatalogManager { get; protected set; } 

        #endregion

        #region Controller actions

        /// <summary>
        /// The action for rendering the search bar.
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <returns>The search view.</returns>
        public ActionResult SearchBar(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword)
        {
            var model = new SearchBarModel { SearchKeyword = searchKeyword };
            return this.View(this.CurrentRenderingView, model);
        }

        /// <summary>
        /// The action for rendering the search results facets view
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The product search facets.</returns>
        public ActionResult ProductSearchResultsFacets(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection);
            var viewModel = this.GetProductFacetsViewModel(searchInfo.SearchOptions, searchKeyword, searchInfo.Catalog.Name, this.CurrentRendering);
            return this.View(this.GetAbsoluteRenderingView("/Catalog/ProductFacets"), viewModel);
        }

        /// <summary>
        /// The action for rendering the search results list header view
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The product search result list header view.</returns>
        public ActionResult ProductSearchResultsListHeader(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection);
            var viewModel = this.GetProductListHeaderViewModel(searchInfo.SearchOptions, searchInfo.SortFields, searchInfo.SearchKeyword, searchInfo.Catalog.Name, this.CurrentRendering);
            return this.View(this.GetAbsoluteRenderingView("/Catalog/ProductListHeader"), viewModel);
        }

        /// <summary>
        /// An action to manage data for the search results list
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>
        /// The view that represents the search results list
        /// </returns>
        public ActionResult ProductSearchResultsList(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection);
            var viewModel = GetProductListViewModel(searchInfo.SearchOptions, searchInfo.SortFields, searchInfo.SearchKeyword, searchInfo.Catalog.Name, this.CurrentRendering);
            return this.View(this.GetAbsoluteRenderingView("/Catalog/ProductList"), viewModel);
        }

        /// <summary>
        /// The action for rendering the pagination view
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The pagination view.</returns>
        public ActionResult ProductSearchResultsPagination(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, facetValues, sortField, pageSize, sortDirection);
            var viewModel = this.GetPaginationViewModel(searchInfo.SearchOptions, searchInfo.SearchKeyword, searchInfo.Catalog.Name, this.CurrentRendering);
            return this.View(this.GetRenderingView("Pagination"), viewModel);
        }

        /// <summary>
        /// returns the the view for site content search results list.
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// The view that represents the site content search results list.
        /// </returns>
        public ActionResult SiteContentSearchResultsList(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPaging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPageSize)] int? pageSize)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, null, null, pageSize, null);
            var viewModel = this.GetSiteContentListViewModel(searchInfo.SearchOptions, searchKeyword, this.CurrentRendering);
            return this.View(this.GetAbsoluteRenderingView("/Catalog/SiteContentList"), viewModel);
        }

        /// <summary>
        /// The action for rendering the search results list header view
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>The search result view.</returns>
        public ActionResult SiteContentSearchResultsListHeader(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPaging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPageSize)] int? pageSize)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, null, null, pageSize, null);
            var viewModel = this.GetSiteContentListHeaderViewModel(searchInfo.SearchOptions, searchInfo.SearchKeyword, this.CurrentRendering);
            return this.View(this.GetAbsoluteRenderingView("/Catalog/ProductListHeader"), viewModel);
        }

        /// <summary>
        /// The action for rendering the pagination view
        /// </summary>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>The pagination view.</returns>
        public ActionResult SiteContentSearchResultsPagination(
            [Bind(Prefix = StorefrontConstants.QueryStrings.SearchKeyword)] string searchKeyword,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPaging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SiteContentPageSize)] int? pageSize)
        {
            var searchInfo = this.GetSearchInfo(searchKeyword, pageNumber, null, null, pageSize, null);
            var viewModel = this.GetSiteContentPaginationModel(searchInfo.SearchOptions, searchInfo.SearchKeyword, this.CurrentRendering);
            return this.View(this.GetRenderingView("Pagination"), viewModel);
        }

        #endregion

        #region Protected and helper methods

        /// <summary>
        /// Builds a product list header view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="sortFields">The sort fields.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The product list header view model.</returns>
        protected virtual ProductListHeaderViewModel GetProductListHeaderViewModel(CommerceSearchOptions productSearchOptions, IEnumerable<CommerceQuerySort> sortFields, string searchKeyword, string catalogName, Rendering rendering)
        {
            var viewModel = new ProductListHeaderViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = this.GetChildProducts(productSearchOptions, searchKeyword, catalogName);
            }

            viewModel.Initialize(rendering, childProducts, sortFields, productSearchOptions);
            return viewModel;
        }

        /// <summary>
        /// Builds a product facets view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The product facet view model.</returns>
        protected virtual ProductFacetsViewModel GetProductFacetsViewModel(CommerceSearchOptions productSearchOptions, string searchKeyword, string catalogName, Rendering rendering)
        {
            var viewModel = new ProductFacetsViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = this.GetChildProducts(productSearchOptions, searchKeyword, catalogName);
            }

            viewModel.Initialize(rendering, childProducts, productSearchOptions);

            return viewModel;
        }

        /// <summary>
        /// Builds a pagination view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The pagination view model.</returns>
        protected virtual PaginationViewModel GetPaginationViewModel(CommerceSearchOptions productSearchOptions, string searchKeyword, string catalogName, Rendering rendering)
        {
            var viewModel = new PaginationViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = this.GetChildProducts(productSearchOptions, searchKeyword, catalogName);
            }

            viewModel.Initialize(rendering, childProducts, productSearchOptions);
            return viewModel;
        }

        /// <summary>
        /// Builds a category view model or retrieves it if it already exists
        /// </summary>
        /// <param name="productSearchOptions">The product options class for finding child products</param>
        /// <param name="sortFields">The fields to sort he results on</param>
        /// <param name="searchKeyword">The keyword to search for</param>
        /// <param name="catalogName">The name of the catalog to search against</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <returns>A category view model</returns>
        protected virtual CategoryViewModel GetProductListViewModel(CommerceSearchOptions productSearchOptions, IEnumerable<CommerceQuerySort> sortFields, string searchKeyword, string catalogName, Rendering rendering)
        {
            if (this.CurrentSiteContext.Items[CurrentCategoryViewModelKeyName] == null)
            {
                var categoryViewModel = new CategoryViewModel();

                var childProducts = GetChildProducts(productSearchOptions, searchKeyword, catalogName);

                categoryViewModel.Initialize(rendering, childProducts, sortFields, productSearchOptions);
                if (childProducts != null && childProducts.SearchResultItems.Count > 0)
                {
                    this.CatalogManager.GetProductBulkPrices(categoryViewModel.ChildProducts);
                    this.CatalogManager.GetProductsStockStatus(categoryViewModel.ChildProducts);
                    foreach (var productViewModel in categoryViewModel.ChildProducts)
                    {
                        Item productItem = childProducts.SearchResultItems.Where(item => item.Name == productViewModel.ProductId).Single();
                        productViewModel.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem);
                    }
                }

                this.CurrentSiteContext.Items[CurrentCategoryViewModelKeyName] = categoryViewModel;
            }

            var viewModel = (CategoryViewModel)this.CurrentSiteContext.Items[CurrentCategoryViewModelKeyName];
            return viewModel;
        }

        /// <summary>
        /// This method returns child products for this category
        /// </summary>
        /// <param name="searchOptions">The options to perform the search with</param>
        /// <param name="searchKeyword">The keyword to search for</param>
        /// <param name="catalogName">The name of the catalog to search against</param>
        /// <returns>A list of child products</returns>        
        protected SearchResults GetChildProducts(CommerceSearchOptions searchOptions, string searchKeyword, string catalogName)
        {
            if (this.CurrentSiteContext.Items[CurrentSearchProductResultsKeyName] != null)
            {
                return (SearchResults)this.CurrentSiteContext.Items[CurrentSearchProductResultsKeyName];
            }

            Assert.ArgumentNotNull(searchKeyword, "searchOptions");
            Assert.ArgumentNotNull(searchKeyword, "searchKeyword");
            Assert.ArgumentNotNull(searchKeyword, "catalogName");

            var returnList = new List<Item>();
            var totalPageCount = 0;
            var totalProductCount = 0;
            IEnumerable<CommerceQueryFacet> facets = Enumerable.Empty<CommerceQueryFacet>();

            if (Item != null && !string.IsNullOrEmpty(searchKeyword.Trim()))
            {
                SearchResponse searchResponse = null;
                searchResponse = SearchNavigation.SearchCatalogItemsByKeyword(searchKeyword, catalogName, searchOptions);

                if (searchResponse != null)
                {
                    returnList.AddRange(searchResponse.ResponseItems);
                    totalProductCount = searchResponse.TotalItemCount;
                    totalPageCount = searchResponse.TotalPageCount;
                    facets = searchResponse.Facets;
                }
            }

            var results = new SearchResults(returnList, totalProductCount, totalPageCount, searchOptions.StartPageIndex, facets);
            this.CurrentSiteContext.Items[CurrentSearchProductResultsKeyName] = results;
            return results;
        }

        /// <summary>
        /// Gets a <see cref="SearchResults" /> that represents the site content search results.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The search results.</returns>
        protected virtual SearchResults GetSiteContentSearchResults(CommerceSearchOptions searchOptions, string searchKeyword, Rendering rendering)
        {
            if (this.CurrentSiteContext.Items[CurrentSearchContentResultsKeyName] != null)
            {
                return (SearchResults)this.CurrentSiteContext.Items[CurrentSearchContentResultsKeyName];
            }

            var searchResults = new SearchResults();
            var searchResponse = SearchNavigation.SearchSiteByKeyword(searchKeyword, searchOptions);
            if (searchResponse != null)
            {
                searchResults = new SearchResults(searchResponse.ResponseItems, searchResponse.TotalItemCount, searchResponse.TotalPageCount, searchOptions.StartPageIndex, searchResponse.Facets);
            }

            this.CurrentSiteContext.Items[CurrentSearchContentResultsKeyName] = searchResults;
            return searchResults;
        }

        /// <summary>
        /// Gets a <see cref="SiteContentSearchResultsViewModel" /> object that represents the site content search results.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The site content search result view model.</returns>
        protected virtual SiteContentSearchResultsViewModel GetSiteContentListViewModel(CommerceSearchOptions searchOptions, string searchKeyword, Rendering rendering)
        {
            var model = new SiteContentSearchResultsViewModel();
            model.Initialize(rendering);

            var searchResults = this.GetSiteContentSearchResults(searchOptions, searchKeyword, rendering);
            if (searchResults != null)
            {
                model.ContentItems = searchResults.SearchResultItems
                    .Select(item => SiteContentViewModel.Create(item))
                    .ToList();
            }

            return model;
        }

        /// <summary>
        /// Gets a <see cref="PaginationViewModel" /> that represents the site content search results.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The pagination view model.</returns>
        protected virtual PaginationViewModel GetSiteContentPaginationModel(CommerceSearchOptions searchOptions, string searchKeyword, Rendering rendering)
        {
            var viewModel = new PaginationViewModel();

            SearchResults searchResults = null;
            if (searchOptions != null)
            {
                searchResults = this.GetSiteContentSearchResults(searchOptions, searchKeyword, rendering);
            }

            viewModel.Initialize(rendering, searchResults, searchOptions);
            viewModel.QueryStringToken = StorefrontConstants.QueryStrings.SiteContentPaging;

            return viewModel;
        }

        /// <summary>
        /// Builds a product list header view model
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        /// <param name="searchKeyword">The search keyword.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The product list header view model.</returns>
        protected virtual ProductListHeaderViewModel GetSiteContentListHeaderViewModel(CommerceSearchOptions searchOptions, string searchKeyword, Rendering rendering)
        {
            var viewModel = new ProductListHeaderViewModel { PageSizeClass = StorefrontConstants.StyleClasses.ChangeSiteContentPageSize };
            SearchResults searchResults = null;
            if (searchOptions != null)
            {
                searchResults = this.GetSiteContentSearchResults(searchOptions, searchKeyword, rendering);
            }

            viewModel.Initialize(rendering, searchResults, null, searchOptions);

            return viewModel;
        }

        /// <summary>
        /// Takes a collection of a facets and a querystring of facet values and adds the values to the collection
        /// </summary>
        /// <param name="facets">The facet collection</param>
        /// <param name="valueQueryString">The values to add to the collection in querystring format</param>
        /// <param name="productSearchOptions">The options to update with facets</param>
        protected void UpdateOptionsWithFacets(IEnumerable<CommerceQueryFacet> facets, string valueQueryString, CommerceSearchOptions productSearchOptions)
        {
            if (facets != null && facets.Any())
            {
                if (!string.IsNullOrEmpty(valueQueryString))
                {
                    var facetValuesCombos = valueQueryString.Split(new char[] { '&' });

                    foreach (var facetValuesCombo in facetValuesCombos)
                    {
                        var facetValues = facetValuesCombo.Split(new char[] { '=' });

                        var name = facetValues[0];

                        var existingFacet = facets.FirstOrDefault(item => item.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

                        if (existingFacet != null)
                        {
                            var values = facetValues[1].Split(new char[] { StorefrontConstants.QueryStrings.FacetsSeparator });

                            foreach (var value in values)
                            {
                                existingFacet.Values.Add(value);
                            }
                        }
                    }
                }

                productSearchOptions.FacetFields = facets;
            }
        }

        /// <summary>
        /// Adds and required sorting to the options class
        /// </summary>
        /// <param name="sortField">The field to sort on</param>
        /// <param name="sortDirection">The direction to perform the sorting</param>
        /// <param name="productSearchOptions">The options class to add the sorting to.</param>
        protected void UpdateOptionsWithSorting(string sortField, CommerceConstants.SortDirection? sortDirection, CommerceSearchOptions productSearchOptions)
        {
            if (!string.IsNullOrEmpty(sortField))
            {
                productSearchOptions.SortField = sortField;

                if (sortDirection.HasValue)
                {
                    productSearchOptions.SortDirection = sortDirection.Value;
                }

                ViewBag.SortField = sortField;
                ViewBag.SortDirection = sortDirection;
            }
        }

        private SearchInfo GetSearchInfo(string searchKeyword, int? pageNumber, string facetValues, string sortField, int? pageSize, CommerceConstants.SortDirection? sortDirection)
        {
            if (this.CurrentSiteContext.Items[CurrentSearchInfoKeyName] != null)
            {
                return (SearchInfo)this.CurrentSiteContext.Items[CurrentSearchInfoKeyName];
            }

            var searchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>();
            var searchInfo = new SearchInfo
                                {
                                    SearchKeyword = searchKeyword ?? string.Empty,
                                    RequiredFacets = searchManager.GetFacetFieldsForItem(this.Item),
                                    SortFields = searchManager.GetSortFieldsForItem(this.Item),
                                    Catalog = StorefrontManager.CurrentStorefront.DefaultCatalog,
                                    ItemsPerPage = pageSize ?? searchManager.GetItemsPerPageForItem(this.Item)
                                };
            if (searchInfo.ItemsPerPage == 0)
            {
                searchInfo.ItemsPerPage = StorefrontConstants.Settings.DefaultItemsPerPage;
            }

            var productSearchOptions = new CommerceSearchOptions(searchInfo.ItemsPerPage, pageNumber.GetValueOrDefault(0));
            this.UpdateOptionsWithFacets(searchInfo.RequiredFacets, facetValues, productSearchOptions);
            this.UpdateOptionsWithSorting(sortField, sortDirection, productSearchOptions);
            searchInfo.SearchOptions = productSearchOptions;

            this.CurrentSiteContext.Items[CurrentSearchInfoKeyName] = searchInfo;
            return searchInfo;
        }

        private class SearchInfo
        {
            public string SearchKeyword { get; set; }

            public IEnumerable<CommerceQueryFacet> RequiredFacets { get; set; }
            
            public IEnumerable<CommerceQuerySort> SortFields { get; set; }
            
            public int ItemsPerPage { get; set; }
            
            public Catalog Catalog { get; set; }
            
            public CommerceSearchOptions SearchOptions { get; set; }
        }

        #endregion
    }
}