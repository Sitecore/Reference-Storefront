//-----------------------------------------------------------------------
// <copyright file="WishListManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the wishlist business logic for the site.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.WishLists;
    using Sitecore.Commerce.Services.WishLists;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Commerce.Entities.Inventory;

    /// <summary>
    /// Defines the WishListManager class.
    /// </summary>
    public class WishListManager : BaseManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListManager" /> class.
        /// </summary>
        /// <param name="wishListServiceProvider">The wish list service provider.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="inventoryManager">The inventory manager.</param>
        public WishListManager([NotNull] WishListServiceProvider wishListServiceProvider, [NotNull] AccountManager accountManager, [NotNull] InventoryManager inventoryManager)
        {
            Assert.ArgumentNotNull(wishListServiceProvider, "wishListServiceProvider");
            Assert.ArgumentNotNull(accountManager, "accountManager");
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");

            this.WishListServiceProvider = wishListServiceProvider;
            this.AccountManager = accountManager;
            this.InventoryManager = inventoryManager;
        }

        #endregion

        #region Properties (public)

        /// <summary>
        /// Gets or sets the wish list service provider.
        /// </summary>
        /// <value>
        /// The wish list service provider.
        /// </value>
        public WishListServiceProvider WishListServiceProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the account manager.
        /// </summary>
        /// <value>
        /// The account manager.
        /// </value>
        public AccountManager AccountManager { get; protected set; }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        #endregion

        #region Methods (public, virtual)

        /// <summary>
        /// Creates the wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListName">Name of the wish list.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<CreateWishListResult, WishList> CreateWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string wishListName)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(wishListName, "wishListName");

            var request = new CreateWishListRequest(visitorContext.UserId, wishListName, storefront.ShopName);
            var result = this.WishListServiceProvider.CreateWishList(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<CreateWishListResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Gets the wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<GetWishListResult, WishList> GetWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string wishListId)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var request = new GetWishListRequest(visitorContext.UserId, wishListId, storefront.ShopName);
            var result = this.WishListServiceProvider.GetWishList(request);
            if (result.Success && result.WishList != null)
            {
                this.PopulateStockInformation(storefront, result.WishList);
            }
            else if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<GetWishListResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Gets the wish lists.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>> GetWishLists([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");

            var request = new GetWishListsRequest(visitorContext.UserId, storefront.ShopName);
            var result = this.WishListServiceProvider.GetWishLists(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<GetWishListsResult, IEnumerable<WishListHeader>>(result, result.WishLists.ToList());
        }

        /// <summary>
        /// Deletes the wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<DeleteWishListResult, WishList> DeleteWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string wishListId)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var request = new DeleteWishListRequest(new WishList { UserId = visitorContext.UserId, CustomerId = visitorContext.UserId, ExternalId = wishListId, ShopName = storefront.ShopName });
            var result = this.WishListServiceProvider.DeleteWishList(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<DeleteWishListResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Removes the wish list lines.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        /// <param name="models">The models.</param>
        /// <returns>
        /// The manager response with the wish list as the result.
        /// </returns>
        public virtual ManagerResponse<RemoveWishListLinesResult, WishList> RemoveWishListLines([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string wishListId, IEnumerable<WishListLineInputModel> models)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(models, "models");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var lineIds = models.Select(model => model.ExternalLineId).ToList();
            var request = new RemoveWishListLinesRequest(new WishList { UserId = visitorContext.UserId, CustomerId = visitorContext.UserId, ExternalId = wishListId, ShopName = storefront.ShopName }, lineIds);
            var result = this.WishListServiceProvider.RemoveWishListLines(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<RemoveWishListLinesResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Updates the wish list lines.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<UpdateWishListLinesResult, WishList> UpdateWishListLines([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string wishListId, IEnumerable<WishListLine> lines)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(lines, "lines");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var request = new UpdateWishListLinesRequest(new WishList { UserId = visitorContext.UserId, CustomerId = visitorContext.UserId, ExternalId = wishListId, ShopName = storefront.ShopName }, lines);
            var result = this.WishListServiceProvider.UpdateWishListLines(request);
            if (result.Success && result.WishList != null)
            {
                this.PopulateStockInformation(storefront, result.WishList);
            }
            else if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<UpdateWishListLinesResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Updates a single wish list line.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="model">The model.</param>
        /// <returns>The manager response with the updated wish list as the result.</returns>
        public virtual ManagerResponse<UpdateWishListLinesResult, WishList> UpdateWishListLine([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] WishListLineInputModel model)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(model, "model");
            Assert.ArgumentNotNullOrEmpty(model.WishListId, "model.WishListId");
            Assert.ArgumentNotNullOrEmpty(model.ProductId, "model.ProductId");

            var wishListLine = new WishListLine
            {
                Product = new CommerceCartProduct { ProductId = model.ProductId, ProductVariantId = model.VariantId },
                Quantity = model.Quantity
            };

            return this.UpdateWishListLines(storefront, visitorContext, model.WishListId, new List<WishListLine> { wishListLine });
        }

        /// <summary>
        /// Adds the lines to wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="wishListId">The wish list identifier.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<AddLinesToWishListResult, WishList> AddLinesToWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string wishListId, IEnumerable<WishListLine> lines)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(lines, "lines");
            Assert.ArgumentNotNullOrEmpty(wishListId, "wishListId");

            var request = new AddLinesToWishListRequest(new WishList { UserId = visitorContext.UserId, CustomerId = visitorContext.UserId, ExternalId = wishListId, ShopName = storefront.ShopName }, lines);
            var result = this.WishListServiceProvider.AddLinesToWishList(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<AddLinesToWishListResult, WishList>(result, result.WishList);
        }

        /// <summary>
        /// Adds the lines to wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The manager response with the wish list as the result.
        /// </returns>
        public virtual ManagerResponse<AddLinesToWishListResult, WishList> AddLinesToWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, AddToWishListInputModel model)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(model, "model");

            var product = new CommerceCartProduct
            {
                ProductCatalog = model.ProductCatalog,
                ProductId = model.ProductId,
                ProductVariantId = model.VariantId
            };

            var line = new WishListLine
            {
                Product = product,
                Quantity = model.Quantity == null ? 1 : (uint)model.Quantity
            };

            if (line.Product.ProductId.Equals(storefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase))
            {
                line.Properties.Add("GiftCardAmount", model.GiftCardAmount);
            }

            // create wish list
            if (model.WishListId == null && !string.IsNullOrEmpty(model.WishListName))
            {
                var newList = this.CreateWishList(storefront, visitorContext, model.WishListName).Result;
                if (newList == null)
                {
                    return new ManagerResponse<AddLinesToWishListResult, WishList>(new AddLinesToWishListResult { Success = false }, null);
                }

                model.WishListId = newList.ExternalId;
            }

            var result = this.AddLinesToWishList(storefront, visitorContext, model.WishListId, new List<WishListLine> { line });

            return new ManagerResponse<AddLinesToWishListResult, WishList>(result.ServiceProviderResult, result.ServiceProviderResult.WishList);
        }
        
        /// <summary>
        /// Updates the wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="model">The model.</param>
        /// <returns>The manager response with the wish list as the result.</returns>
        public virtual ManagerResponse<UpdateWishListResult, WishList> UpdateWishList([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, UpdateWishListInputModel model)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(model, "model");

            var request = new UpdateWishListRequest(new WishList { UserId = visitorContext.UserId, CustomerId = visitorContext.UserId, ExternalId = model.ExternalId, Name = model.Name, IsFavorite = model.IsFavorite, ShopName = storefront.ShopName });
            var result = this.WishListServiceProvider.UpdateWishList(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<UpdateWishListResult, WishList>(result, result.WishList);
        }

        #endregion

        #region Methods (protected, virtual)

        /// <summary>
        /// Populates the stock information of the given wish list.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="wishList">The wish list.</param>
        protected virtual void PopulateStockInformation(CommerceStorefront storefront, WishList wishList)
        {
            var productList = wishList.Lines.Select(line => new CommerceInventoryProduct { ProductId = line.Product.ProductId, CatalogName = ((CommerceCartProduct)line.Product).ProductCatalog }).ToList();

            var stockInfos = this.InventoryManager.GetStockInformation(storefront, productList, StockDetailsLevel.Status).Result;
            if (stockInfos == null)
            {
                return;
            }

            foreach (var line in wishList.Lines)
            {
                var stockInfo = stockInfos.ToList().FirstOrDefault(si => si.Product.ProductId.Equals(line.Product.ProductId, StringComparison.OrdinalIgnoreCase));
                if (stockInfo == null)
                {
                    continue;
                }

                line.Product.StockStatus = new Sitecore.Commerce.Entities.Inventory.StockStatus(Convert.ToInt32((decimal)stockInfo.Properties["OnHandQuantity"]), stockInfo.Status.Name);
                this.InventoryManager.VisitedProductStockStatus(storefront, stockInfo, string.Empty);
            }
        }
        
        #endregion
    }
}