//-----------------------------------------------------------------------
// <copyright file="CatalogController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CheckoutController class.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Models.RenderingModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Reference.Storefront.SitecorePipelines;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using Sitecore.Reference.Storefront.ExtensionMethods;

    /// <summary>
    /// Used to manage the data and view retrieval for catalog pages
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class CatalogController : CSBaseController
    {
        #region Variables

        /// <summary>
        /// The commerce named search template
        /// </summary>
        public static ID CommerceNamedSearchTemplate = new ID("{9F7D719A-3A05-4A64-AA74-3C46D8D0D20D}");

        private const string CurrentProductViewModelKeyName = "CurrentProductViewModel";

        #endregion

        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogController" /> class.
        /// </summary>
        /// <param name="inventoryManager">The inventory manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="catalogManager">The catalog manager.</param>
        /// <param name="giftCardManager">The gift card manager.</param>
        public CatalogController(
            [NotNull] InventoryManager inventoryManager,
            [NotNull] ContactFactory contactFactory,
            [NotNull] AccountManager accountManager,
            [NotNull] CatalogManager catalogManager,
            [NotNull] GiftCardManager giftCardManager)
            : base(accountManager, contactFactory)
        {
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");
            Assert.ArgumentNotNull(catalogManager, "catalogManager");
            Assert.ArgumentNotNull(giftCardManager, "giftCardManager");

            this.InventoryManager = inventoryManager;
            this.CatalogManager = catalogManager;
            this.GiftCardManager = giftCardManager;
        }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        /// <summary>
        /// Gets the catalog manager.
        /// </summary>
        public CatalogManager CatalogManager { get; private set; }
        
        /// <summary>
        /// Gets or sets the gift card manager.
        /// </summary>
        /// <value>
        /// The gift card manager.
        /// </value>
        public GiftCardManager GiftCardManager { get; protected set; }

        #endregion

        #region Controller actions

        /// <summary>
        /// An action to manage data for the home page
        /// </summary>
        /// <returns>The view that represents the home page</returns>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HomePage", Justification = "This is a valid name.")]
        public ActionResult HomePage()
        {
            return View(CurrentRenderingView);
        }

        /// <summary>
        /// An action to manage data for the CategoryList
        /// </summary>
        /// <returns>The view that represents the CategoryList</returns>
        public ActionResult CategoryList()
        {
            var datasource = CurrentRendering.DataSource;

            if (!string.IsNullOrEmpty(datasource))
            {
                var datasourceItem = Context.Database.GetItem(ID.Parse(datasource));
                var categoryViewModel = GetCategoryViewModel(null, null, datasourceItem, CurrentRendering, string.Empty);

                return View(CurrentRenderingView, categoryViewModel);
            }

            return View(CurrentRenderingView);
        }

        /// <summary>
        /// PageTitle View
        /// </summary>
        /// <returns>The page title view.</returns>
        public ActionResult PageTitle()
        {
            return View();
        }

        /// <summary>
        /// An action to manage data for the ProductList
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>
        /// The view that represents the ProductList
        /// </returns>
        public ActionResult MultipleProductLists(
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var productSearchOptions = new CommerceSearchOptions
            {
                NumberOfItemsToReturn = StorefrontConstants.Settings.DefaultItemsPerPage,
                StartPageIndex = 0,
                SortField = sortField
            };

            var currentRendering = RenderingContext.Current.Rendering;
            if (string.IsNullOrEmpty(currentRendering.DataSource))
            {
                return GetNoDataSourceView();
            }

            var datasource = currentRendering.Item;

            var multipleProductSearchResults = this.CatalogManager.GetMultipleProductSearchResults(datasource, productSearchOptions);
            if (multipleProductSearchResults != null)
            {
                multipleProductSearchResults.Initialize(this.CurrentRendering);
                multipleProductSearchResults.DisplayName = datasource.DisplayName;

                var products = multipleProductSearchResults.ProductSearchResults.SelectMany(productSearchResult => productSearchResult.Products).ToList();
                this.CatalogManager.GetProductBulkPrices(products);
                this.CatalogManager.GetProductsStockStatus(products);

                foreach (var productViewModel in products)
                {
                    Item productItem = multipleProductSearchResults.SearchResults
                        .SelectMany(productSearchResult => productSearchResult.SearchResultItems)
                        .Where(item => item.Name == productViewModel.ProductId).FirstOrDefault();
                    productViewModel.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem);
                } 
                
                return View(CurrentRenderingView, multipleProductSearchResults);
            }

            return View(CurrentRenderingView);
        }

        /// <summary>
        /// The action for rendering the category view
        /// </summary>
        /// <param name="pageNumber">The product page number</param>
        /// <param name="facetValues">A facet query string</param>
        /// <param name="sortField">The field to sort on</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="sortDirection">The direction to sort</param>
        /// <returns>The category view</returns>
        public ActionResult Category(
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            Category currentCategory;

            //This is a Wild Card - Wild card pages are named "*"
            if (Item.Name == "*")
            {
                //Supported option - pass in a categoryid
                currentCategory = this.CatalogManager.GetCurrentCategoryByUrl();
                ViewBag.Title = currentCategory.Name;
            }
            else
            {
                currentCategory = this.CatalogManager.GetCategory(Item);
            }

            if (currentCategory == null)
            {
                return View(CurrentRenderingView, null);
            }

            var productSearchOptions = new CommerceSearchOptions(pageSize.GetValueOrDefault(currentCategory.ItemsPerPage), pageNumber.GetValueOrDefault(0));

            UpdateOptionsWithFacets(currentCategory.RequiredFacets, facetValues, productSearchOptions);
            UpdateOptionsWithSorting(sortField, sortDirection, productSearchOptions);

            var viewModel = GetCategoryViewModel(productSearchOptions, currentCategory.SortFields, currentCategory.InnerItem, this.CurrentRendering, currentCategory.InnerItem.DisplayName);

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// The action for rendering the product list header view
        /// </summary>
        /// <returns>The navigation view.</returns>
        public ActionResult Navigation()
        {
            var dataSourcePath = Item["CategoryDatasource"];

            if (string.IsNullOrEmpty(dataSourcePath))
            {
                return View(CurrentRenderingView, null);
            }

            var dataSource = Item.Database.GetItem(dataSourcePath);

            if (dataSource == null)
            {
                return View(CurrentRenderingView, null);
            }

            Category currentCategory = this.CatalogManager.GetCategory(dataSource);

            if (currentCategory == null)
            {
                return View(CurrentRenderingView, null);
            }

            var viewModel = GetNavigationViewModel(currentCategory.InnerItem, this.CurrentRendering);

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// Childs the category navigation.
        /// </summary>
        /// <returns>The children category navigation view.</returns>
        public ActionResult ChildCategoryNavigation()
        {
            Item categoryItem = this.CurrentSiteContext.CurrentCatalogItem;
            Template t = TemplateManager.GetTemplate(categoryItem.TemplateID, Context.Database);
            Assert.IsTrue(t.DescendsFrom(CommerceConstants.KnownTemplateIds.CommerceCategoryTemplate), "Current item must be a Category.");

            var viewModel = GetNavigationViewModel(categoryItem, this.CurrentRendering);
            if (viewModel.ChildCategories.Count == 0)
            {
                viewModel = GetNavigationViewModel(categoryItem.Parent, this.CurrentRendering);
            }

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// The action for rendering the product list header view
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <param name="sortField">The sort field.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The product list header view.</returns>
        public ActionResult ProductListHeader(
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Sort)] string sortField,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.SortDirection)] CommerceConstants.SortDirection? sortDirection)
        {
            var currentCategory = this.CatalogManager.GetCurrentCategoryByUrl();
            var productSearchOptions = new CommerceSearchOptions(pageSize.GetValueOrDefault(currentCategory.ItemsPerPage), pageNumber.GetValueOrDefault(0));

            UpdateOptionsWithFacets(currentCategory.RequiredFacets, facetValues, productSearchOptions);
            UpdateOptionsWithSorting(sortField, sortDirection, productSearchOptions);

            var viewModel = GetProductListHeaderViewModel(productSearchOptions, currentCategory.SortFields, currentCategory.InnerItem, this.CurrentRendering);

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// The action for rendering the pagination view
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <returns>The pagination view.</returns>
        public ActionResult Pagination(
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues)
        {
            var currentCategory = this.CatalogManager.GetCurrentCategoryByUrl();
            var productSearchOptions = new CommerceSearchOptions(pageSize.GetValueOrDefault(currentCategory.ItemsPerPage), pageNumber.GetValueOrDefault(0));

            UpdateOptionsWithFacets(currentCategory.RequiredFacets, facetValues, productSearchOptions);
            var viewModel = GetPaginationViewModel(productSearchOptions, currentCategory.InnerItem, CurrentRendering);

            return View(CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// The action for rendering the product facets view
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="facetValues">The facet values.</param>
        /// <returns>The product facet view.</returns>
        public ActionResult ProductFacets(
            [Bind(Prefix = StorefrontConstants.QueryStrings.Paging)] int? pageNumber,
            [Bind(Prefix = StorefrontConstants.QueryStrings.PageSize)] int? pageSize,
            [Bind(Prefix = StorefrontConstants.QueryStrings.Facets)] string facetValues)
        {
            var currentCategory = this.CatalogManager.GetCurrentCategoryByUrl();
            var productSearchOptions = new CommerceSearchOptions(pageSize.GetValueOrDefault(currentCategory.ItemsPerPage), pageNumber.GetValueOrDefault(0));

            UpdateOptionsWithFacets(currentCategory.RequiredFacets, facetValues, productSearchOptions);
            var viewModel = GetProductFacetsViewModel(productSearchOptions, currentCategory.InnerItem, this.CurrentRendering);

            return View(this.CurrentRenderingView, viewModel);
        }

        /// <summary>
        /// Returns a curated set of product Images based on the Datasource
        /// </summary>
        /// <returns>The curated product image view.</returns>
        public ActionResult CuratedProductImages()
        {
            var images = new List<MediaItem>();

            MultilistField field = Item.Fields["ProductImages"];

            if (field != null)
            {
                images.AddRange(field.TargetIDs.Select(id => Item.Database.GetItem(id)).Select(mediaItem => (MediaItem)mediaItem));
            }

            return View(CurrentRenderingView, images);
        }

        /// <summary>
        /// The action for logging a visit to the product details page.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult VisitedProductDetailsPage()
        {
            this.CatalogManager.VisitedProductDetailsPage();
            return this.View(CurrentRenderingView);
        }

        /// <summary>
        /// The action for logging a visit to a category page.
        /// </summary>
        /// <returns>The action result.</returns>
        public ActionResult VisitedCategoryPage()
        {
            this.CatalogManager.VisitedCategoryPage();
            return this.View(CurrentRenderingView);
        }

        /// <summary>
        /// The action for rendering the product view
        /// </summary>
        /// <returns>The product view</returns>
        public ActionResult Product()
        {
            //Wild card pages are named "*"
            if (this.Item.Name == "*")
            {
                // This is a Wild Card
                var productViewModel = this.GetWildCardProductViewModel();
                return this.View(CurrentRenderingView, productViewModel);
            }

            //Special handling for gift card
            if (this.Item.Name.ToLower(CultureInfo.InvariantCulture) == ProductItemResolver.BuyGiftCardUrlRoute)
            {
                this.Item = SearchNavigation.GetProduct(StorefrontManager.CurrentStorefront.GiftCardProductId, CurrentCatalog.Name);
            }

            return this.View(CurrentRenderingView, GetProductViewModel(this.Item, CurrentRendering));
        }

        /// <summary>
        /// The action for rendering the related catalog items.
        /// </summary>
        /// <returns>The related catalog items view</returns>
        public ActionResult RelatedCatalogItems()
        {
            //Wild card pages are named "*"
            if (this.Item.Name == "*")
            {
                // This is a Wild Card
                var productViewModel = this.GetWildCardProductViewModel();
                var relatedCatalogItemsModel = this.CatalogManager.GetRelationshipsFromItem(productViewModel.Item, this.CurrentRendering);
                return this.View(CurrentRenderingView, relatedCatalogItemsModel);
            }
            else
            {
                var relatedCatalogItemsModel = this.CatalogManager.GetRelationshipsFromItem(this.Item, this.CurrentRendering);
                return this.View(CurrentRenderingView, relatedCatalogItemsModel);
            }
        }
        
        /// <summary>
        /// Checks the gift card balance.
        /// </summary>
        /// <returns>Gift card balance view</returns>
        [HttpGet]
        public ActionResult CheckGiftCardBalance()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Gift card presentation.
        /// </summary>
        /// <returns>GiftCardPresentation view</returns>
        [HttpGet]
        public ActionResult GiftCardPresentation()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Product presentation.
        /// </summary>
        /// <returns>ProductPresentation view</returns>
        [HttpGet]
        public ActionResult ProductPresentation()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the current product stock count.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A json result
        /// </returns>
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetCurrentProductStockInfo(ProductStockInfoInputModel model)
        {
            try
            {
                Assert.ArgumentNotNull(model, "model");

                var validationResult = new BaseJsonResult();
                this.ValidateModel(validationResult);
                if (validationResult.HasErrors)
                {
                    return Json(validationResult, JsonRequestBehavior.AllowGet);
                }

                var currentProductItem = SearchNavigation.GetProduct(model.ProductId, CurrentCatalog.Name);
                var productId = currentProductItem.Name;
                var catalogName = currentProductItem["CatalogName"];
                var products = new List<CommerceInventoryProduct>();
                if (currentProductItem.HasChildren)
                {
                    foreach (Item item in currentProductItem.Children)
                    {
                        products.Add(new CommerceInventoryProduct
                        {
                            ProductId = productId,
                            CatalogName = catalogName,
                            VariantId = item["VariantId"]
                        });
                    }
                }
                else
                {
                    products.Add(new CommerceInventoryProduct { ProductId = productId, CatalogName = catalogName });
                }

                var response = this.InventoryManager.GetStockInformation(this.CurrentStorefront, products, StockDetailsLevel.All);
                var result = new StockInfoListBaseJsonResult(response.ServiceProviderResult);
                if (response.Result == null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Initialize(response.Result);
                var stockInfo = response.Result.FirstOrDefault();
                if (stockInfo != null)
                {
                    this.InventoryManager.VisitedProductStockStatus(this.CurrentStorefront, stockInfo, string.Empty);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetCurrentProductStockInfo", this, e);
                return Json(new BaseJsonResult("GetCurrentProductStockInfo", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the gift card balance.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// A response containing the gift card balance.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult CheckGiftCardBalance(GetGiftCardBalanceInputModel inputModel)
        {
            try
            {
                Assert.ArgumentNotNull(inputModel, "inputModel");

                var validationResult = new BaseJsonResult();
                this.ValidateModel(validationResult);
                if (validationResult.HasErrors)
                {
                    return Json(validationResult, JsonRequestBehavior.AllowGet);
                }

                var response = this.GiftCardManager.GetGiftCardBalance(CurrentStorefront, CurrentVisitorContext, inputModel.GiftCardId);
                var result = new GiftCardBaseJsonResult(response.ServiceProviderResult);
                if (!response.ServiceProviderResult.Success || response.ServiceProviderResult.GiftCard == null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var giftCard = response.ServiceProviderResult.GiftCard;
                result.Initialize(giftCard);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("CheckGiftCardBalance", this, e);
                return Json(new BaseJsonResult("CheckGiftCardBalance", e), JsonRequestBehavior.AllowGet);
            }
        }
        
        /// <summary>
        /// Sign up visitor for product back in stock notification.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A base json result</returns>
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SignUp")]
        public JsonResult SignUpForBackInStockNotification(SignUpForNotificationInputModel model)
        {
            try
            {
                Assert.ArgumentNotNull(model, "model");

                var result = new BaseJsonResult();
                this.ValidateModel(result);
                if (result.HasErrors)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var response = this.InventoryManager.VisitorSignupForStockNotification(this.CurrentStorefront, model, string.Empty);
                result = new BaseJsonResult(response.ServiceProviderResult);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetCurrentUser", this, e);
                return Json(new BaseJsonResult("GetCurrentUser", e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Protected helper methods

        /// <summary>
        /// Builds a product list header view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="sortFields">The sort fields.</param>
        /// <param name="categoryItem">The category item.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The product list header view model.</returns>
        public ProductListHeaderViewModel GetProductListHeaderViewModel(CommerceSearchOptions productSearchOptions, IEnumerable<CommerceQuerySort> sortFields, Item categoryItem, Rendering rendering)
        {
            var viewModel = new ProductListHeaderViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = GetChildProducts(productSearchOptions, categoryItem);
            }

            viewModel.Initialize(rendering, childProducts, sortFields, productSearchOptions);

            return viewModel;
        }

        /// <summary>
        /// Builds a pagination view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="categoryItem">The category item.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The pagination view model.</returns>
        public PaginationViewModel GetPaginationViewModel(CommerceSearchOptions productSearchOptions, Item categoryItem, Rendering rendering)
        {
            var viewModel = new PaginationViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = GetChildProducts(productSearchOptions, categoryItem);
            }

            viewModel.Initialize(rendering, childProducts, productSearchOptions);

            return viewModel;
        }

        /// <summary>
        /// Builds a product facets view model
        /// </summary>
        /// <param name="productSearchOptions">The product search options.</param>
        /// <param name="categoryItem">The category item.</param>
        /// <param name="rendering">The rendering.</param>
        /// <returns>The product facets view model.</returns>
        public ProductFacetsViewModel GetProductFacetsViewModel(CommerceSearchOptions productSearchOptions, Item categoryItem, Rendering rendering)
        {
            var viewModel = new ProductFacetsViewModel();

            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = GetChildProducts(productSearchOptions, categoryItem);
            }

            viewModel.Initialize(rendering, childProducts, productSearchOptions);

            return viewModel;
        }

        /// <summary>
        /// This method returns child categories for this category
        /// </summary>
        /// <param name="searchOptions">The options to perform the search with</param>
        /// <param name="categoryItem">The category item whose children to retrieve</param>
        /// <returns>A list of child categories</returns>       
        protected CategorySearchResults GetChildCategories(CommerceSearchOptions searchOptions, Item categoryItem)
        {
            var returnList = new List<Item>();
            var totalPageCount = 0;
            var totalCategoryCount = 0;

            if (Item != null)
            {
                var searchResponse = SearchNavigation.GetCategoryChildCategories(categoryItem.ID, searchOptions);

                returnList.AddRange(searchResponse.ResponseItems);

                totalCategoryCount = searchResponse.TotalItemCount;
                totalPageCount = searchResponse.TotalPageCount;
            }

            var results = new CategorySearchResults(returnList, totalCategoryCount, totalPageCount, searchOptions.StartPageIndex, new List<FacetCategory>());

            return results;
        }

        /// <summary>
        /// Creates a product view model based on an Item and Rendering
        /// </summary>
        /// <param name="productItem">The product item to based the model on</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <returns>A Product View Model</returns>
        protected ProductViewModel GetProductViewModel(Item productItem, Rendering rendering)
        {
            if (this.CurrentSiteContext.Items[CurrentProductViewModelKeyName] != null)
            {
                return (ProductViewModel)this.CurrentSiteContext.Items[CurrentProductViewModelKeyName];
            }

            var variants = new List<VariantViewModel>();
            if (productItem != null && productItem.HasChildren)
            {
                foreach (Item item in productItem.Children)
                {
                    var v = new VariantViewModel(item);                   
                    variants.Add(v);
                }
            }

            var productViewModel = new ProductViewModel(productItem);
            productViewModel.Initialize(rendering, variants);

            //Special handling for gift card
            if (productViewModel.ProductId == StorefrontManager.CurrentStorefront.GiftCardProductId)
            {
                productViewModel.GiftCardAmountOptions = GetGiftCardAmountOptions(productViewModel);
            }
            else
            {
                this.CatalogManager.GetProductPrice(productViewModel);
                productViewModel.CustomerAverageRating = this.CatalogManager.GetProductRating(productItem);
            }

            this.CurrentSiteContext.Items[CurrentProductViewModelKeyName] = productViewModel;
            return productViewModel;
        }

        /// <summary>
        /// Populates the stock information
        /// </summary>
        /// <param name="model">The product model</param>
        protected void PopulateStockInformation(ProductViewModel model)
        {
            //Check for Gift Card
            if (model.ProductId == StorefrontManager.CurrentStorefront.GiftCardProductId)
            {
                //Gift cards are always in stock..
                model.StockStatus = StockStatus.InStock;
                model.StockStatusName = StorefrontManager.GetProductStockStatusName(StockStatus.InStock);
                return;
            }

            var inventoryProducts = new List<CommerceInventoryProduct> { new CommerceInventoryProduct { ProductId = model.ProductId, CatalogName = model.CatalogName } };
            var response = this.InventoryManager.GetStockInformation(this.CurrentStorefront, inventoryProducts, StockDetailsLevel.StatusAndAvailability);
            if (!response.ServiceProviderResult.Success || response.Result == null)
            {
                return;
            }

            var stockInfos = response.Result;
            var stockInfo = stockInfos.FirstOrDefault();
            if (stockInfo == null || stockInfo.Status == null)
            {
                return;
            }

            model.StockStatus = stockInfo.Status;
            model.StockStatusName = StorefrontManager.GetProductStockStatusName(model.StockStatus);
            if (stockInfo.AvailabilityDate != null)
            {
                model.StockAvailabilityDate = stockInfo.AvailabilityDate.Value.ToShortDateString();
            }
        }

        /// <summary>
        /// Builds a category view model or retrieves it if it already exists
        /// </summary>
        /// <param name="productSearchOptions">The product options class for finding child products</param>
        /// <param name="sortFields">The fields to sort he results on</param>
        /// <param name="categoryItem">The category item to base the view model on</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <param name="cacheName">Name of the cache.</param>
        /// <returns>
        /// A category view model
        /// </returns>
        protected virtual CategoryViewModel GetCategoryViewModel(CommerceSearchOptions productSearchOptions, IEnumerable<CommerceQuerySort> sortFields, Item categoryItem, Rendering rendering, string cacheName)
        {
            string cacheKey = "Category/" + cacheName;
            bool noCache = (string.IsNullOrEmpty(cacheName));

            if (this.CurrentSiteContext.Items[cacheKey] != null && !noCache)
            {
                return (CategoryViewModel)this.CurrentSiteContext.Items[cacheKey];
            }

            var categoryViewModel = new CategoryViewModel(categoryItem);
            SearchResults childProducts = null;
            if (productSearchOptions != null)
            {
                childProducts = this.GetChildProducts(productSearchOptions, categoryItem);
            }

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

            if (!noCache)
            {
                this.CurrentSiteContext.Items[cacheKey] = categoryViewModel;
            }

            return categoryViewModel;
        }

        /// <summary>
        /// Builds a navigation view model or retrieves it if it already exists
        /// </summary>
        /// <param name="categoryItem">The category item to base the view model on</param>
        /// <param name="rendering">The rendering to initialize the model with</param>
        /// <returns>
        /// A category view model
        /// </returns>
        protected virtual NavigationViewModel GetNavigationViewModel(Item categoryItem, Rendering rendering)
        {
            string cacheKey = "Navigation/" + categoryItem.Name;
            bool noCache = (string.IsNullOrEmpty(categoryItem.Name));

            if (this.CurrentSiteContext.Items[cacheKey] != null && !noCache)
            {
                return (NavigationViewModel)this.CurrentSiteContext.Items[cacheKey];
            }

            var navigationViewModel = new NavigationViewModel();
            CategorySearchResults childCategories = GetChildCategories(new CommerceSearchOptions(), categoryItem);
            navigationViewModel.Initialize(rendering, childCategories);
            if (noCache)
            {
                return navigationViewModel;
            }

            this.CurrentSiteContext.Items[cacheKey] = navigationViewModel;
            return navigationViewModel;
        }

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

            if (Item != null)
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

                        var existingFacet = facets.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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

        private ProductViewModel GetWildCardProductViewModel()
        {
            ProductViewModel productViewModel;
            var productId = CatalogUrlManager.ExtractItemIdFromCurrentUrl();
            var virtualProductCacheKey = string.Format(CultureInfo.InvariantCulture, "VirtualProduct_{0}", productId);
            if (this.CurrentSiteContext.Items.Contains(virtualProductCacheKey))
            {
                productViewModel = this.CurrentSiteContext.Items[virtualProductCacheKey] as ProductViewModel;
            }
            else
            {
                if (string.IsNullOrEmpty(productId))
                {
                    //No ProductId passed in on the URL
                    //Use to Storefront DefaultProductId
                    productId = StorefrontManager.CurrentStorefront.DefaultProductId;
                }

                var productItem = SearchNavigation.GetProduct(productId, this.CurrentCatalog.Name);
                if (productItem == null)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "The requested product '{0}' does not exist in the catalog '{1}' or cannot be displayed in the language '{2}'", productId, this.CurrentCatalog.Name, Context.Language);
                    Log.Error(message, this);
                    throw new InvalidOperationException(message);
                }

                this.Item = productItem;
                productViewModel = this.GetProductViewModel(this.Item, this.CurrentRendering);
                this.CurrentSiteContext.Items.Add(virtualProductCacheKey, productViewModel);
            }

            return productViewModel;
        }

        private List<KeyValuePair<string, decimal?>> GetGiftCardAmountOptions(ProductViewModel productViewModel)
        {
            var giftCardAmountOptions = new Dictionary<string, decimal?>();

            if (productViewModel != null && productViewModel.Variants != null)
            {
                this.CatalogManager.GetProductPrice(productViewModel);
                foreach (var variant in productViewModel.Variants)
                {
                    giftCardAmountOptions.Add(variant.VariantId, Math.Round(variant.AdjustedPrice.Value, 2));
                }

                return giftCardAmountOptions.ToList();
            }           

            return null;
        }

        #endregion
    }
}