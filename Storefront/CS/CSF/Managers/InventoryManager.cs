//-----------------------------------------------------------------------
// <copyright file="InventoryManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the inventory business logic for the site.</summary>
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
    using System;
    using System.Linq;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Multishop;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using Sitecore.Reference.Storefront.Models;

    /// <summary>
    /// Defines the InventoryManager class.
    /// </summary>
    public class InventoryManager : BaseManager
    {
        private readonly CommerceContextBase _obecContext;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryManager" /> class.
        /// </summary>
        /// <param name="inventoryServiceProvider">The inventory service provider.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public InventoryManager([NotNull] InventoryServiceProvider inventoryServiceProvider, [NotNull] ContactFactory contactFactory)
        {
            Assert.ArgumentNotNull(inventoryServiceProvider, "inventoryServiceProvider");
            Assert.ArgumentNotNull(contactFactory, "contactFactory");

            this.InventoryServiceProvider = inventoryServiceProvider;
            this.ContactFactory = contactFactory;
            this._obecContext = (CommerceContextBase)Factory.CreateObject("commerceContext", true);
        }

        #endregion

        #region Properties (public)

        /// <summary>
        /// Gets or sets the inventory service provider.
        /// </summary>
        /// <value>
        /// The inventory service provider.
        /// </value>
        public InventoryServiceProvider InventoryServiceProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the contact factory.
        /// </summary>
        /// <value>
        /// The contact factory.
        /// </value>
        public ContactFactory ContactFactory { get; protected set; }

        #endregion

        #region Methods (public, virtual)

        /// <summary>
        /// Gets the product stock status.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="productViewModels">The product view models.</param>
        public virtual void GetProductsStockStatus([NotNull] CommerceStorefront storefront, List<ProductViewModel> productViewModels)
        {
            if (productViewModels == null || !productViewModels.Any())
            {
                return;
            }

            var products = new List<CommerceInventoryProduct>();
            foreach (var viewModel in productViewModels)
            {
                if (viewModel.Variants != null && viewModel.Variants.Any())
                {
                    foreach (var variant in viewModel.Variants)
                    {
                        products.Add(new CommerceInventoryProduct
                        {
                            ProductId = viewModel.ProductId,
                            CatalogName = viewModel.CatalogName,
                            VariantId = variant.VariantId
                        });
                    }
                }
                else
                {
                    products.Add(new CommerceInventoryProduct { ProductId = viewModel.ProductId, CatalogName = viewModel.CatalogName });
                }
            }

            if (products.Any())
            {
                var response = this.GetStockInformation(storefront, products, StockDetailsLevel.All);
                if (response.Result != null)
                {
                    var stockInfoList = response.Result.ToList();

                    foreach (var viewModel in productViewModels)
                    {
                        StockInformation foundItem = null;
                        if (viewModel.Variants != null && viewModel.Variants.Any())
                        {
                            foreach (var variant in viewModel.Variants)
                            {
                                foundItem = stockInfoList.Find(p => p.Product.ProductId == viewModel.ProductId && ((CommerceInventoryProduct)p.Product).VariantId == variant.VariantId);
                            }
                        }
                        else
                        {
                            foundItem = stockInfoList.Find(p => p.Product.ProductId == viewModel.ProductId);
                        }

                        if (foundItem != null)
                        {
                            viewModel.StockStatus = foundItem.Status;
                            viewModel.StockStatusName = StorefrontManager.GetProductStockStatusName(foundItem.Status);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the stock information.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="products">The products.</param>
        /// <param name="detailsLevel">The details level.</param>
        /// <returns>
        /// The manager response which returns an enumerable collection of StockInformation in the Result.
        /// </returns>
        public virtual ManagerResponse<GetStockInformationResult, IEnumerable<StockInformation>> GetStockInformation([NotNull] CommerceStorefront storefront, IEnumerable<InventoryProduct> products, StockDetailsLevel detailsLevel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(products, "products");

            var request = new GetStockInformationRequest(storefront.ShopName, products, detailsLevel) { Location = this._obecContext.InventoryLocation, VisitorId = this.ContactFactory.GetContact() };
            var result = this.InventoryServiceProvider.GetStockInformation(request);

            // Currently, both Categories and Products are passed in and are waiting for a fix to filter the categories out.  Until then, this code is commented
            // out as it generates an unecessary Error event indicating the product cannot be found.
            // Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetStockInformationResult, IEnumerable<StockInformation>>(result, result.StockInformation ?? new List<StockInformation>());
        }

        /// <summary>
        /// Gets the pre orderable information.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="products">The products.</param>
        /// <returns>The manager response which returns an enumerable collection of OrderableInformation in the Result.</returns>
        public virtual ManagerResponse<GetPreOrderableInformationResult, IEnumerable<OrderableInformation>> GetPreOrderableInformation([NotNull] CommerceStorefront storefront, IEnumerable<InventoryProduct> products)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(products, "products");

            var request = new GetPreOrderableInformationRequest(storefront.ShopName, products);
            var result = this.InventoryServiceProvider.GetPreOrderableInformation(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetPreOrderableInformationResult, IEnumerable<OrderableInformation>>(result, !result.Success || result.OrderableInformation == null ? new List<OrderableInformation>() : result.OrderableInformation);
        }

        /// <summary>
        /// Gets the back orderable information.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="products">The products.</param>
        /// <returns>The manager response which returns an enumerable collection of OrderableInformation in the Result.</returns>
        public virtual ManagerResponse<GetBackOrderableInformationResult, IEnumerable<OrderableInformation>> GetBackOrderableInformation([NotNull] CommerceStorefront storefront, IEnumerable<InventoryProduct> products)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(products, "products");
            
            var request = new GetBackOrderableInformationRequest(storefront.ShopName, products);
            var result = this.InventoryServiceProvider.GetBackOrderableInformation(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetBackOrderableInformationResult, IEnumerable<OrderableInformation>>(result, !result.Success || result.OrderableInformation == null ? new List<OrderableInformation>() : result.OrderableInformation);
        }

        /// <summary>
        /// Visiteds the product stock status.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="stockInformation">The stock information.</param>
        /// <param name="location">The location.</param>
        /// <returns>The manager response which returns success flag in the Result.</returns>
        public virtual ManagerResponse<VisitedProductStockStatusResult, bool> VisitedProductStockStatus([NotNull] CommerceStorefront storefront, StockInformation stockInformation, string location)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(stockInformation, "stockInformation");

            var request = new VisitedProductStockStatusRequest(storefront.ShopName, stockInformation) { Location = location };
            var result = this.InventoryServiceProvider.VisitedProductStockStatus(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<VisitedProductStockStatusResult, bool>(result, result.Success);
        }

        /// <summary>
        /// Visitors the sign up for stock notification.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="model">The model.</param>
        /// <param name="location">The location.</param>
        /// <returns>
        /// The manager response which returns success flag in the Result.
        /// </returns>
        public virtual ManagerResponse<VisitorSignUpForStockNotificationResult, bool> VisitorSignupForStockNotification([NotNull] CommerceStorefront storefront, SignUpForNotificationInputModel model, string location)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNullOrEmpty(model.ProductId, "model.ProductId");
            Assert.ArgumentNotNullOrEmpty(model.Email, "model.Email");

            var visitorId = this.ContactFactory.GetContact();
            var builder = new CommerceInventoryProductBuilder();
            CommerceInventoryProduct inventoryProduct = (CommerceInventoryProduct)builder.CreateInventoryProduct(model.ProductId);
            if (string.IsNullOrEmpty(model.VariantId))
            {
                (inventoryProduct).VariantId = model.VariantId;
            }

            if (string.IsNullOrEmpty(inventoryProduct.CatalogName))
            {
                (inventoryProduct).CatalogName = model.CatalogName;
            }

            DateTime interestDate;
            var isDate = DateTime.TryParse(model.InterestDate, out interestDate);
            var request = new VisitorSignUpForStockNotificationRequest(storefront.ShopName, visitorId, model.Email, inventoryProduct) { Location = location };
            if (isDate)
            {
                request.InterestDate = interestDate;
            }

            var result = this.InventoryServiceProvider.VisitorSignUpForStockNotification(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<VisitorSignUpForStockNotificationResult, bool>(result, result.Success);
        }

        #endregion
    }
}
