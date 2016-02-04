//-----------------------------------------------------------------------
// <copyright file="CartManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The manager class responsible for encapsulating the cart business logic for the site.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Connect.DynamicsRetail.Entities;
    using Sitecore.Commerce.Connect.DynamicsRetail.Services.Carts;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Sitecore.Reference.Storefront.Extensions;

    /// <summary>
    /// Defines the CartManager class.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class CartManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartManager" /> class.
        /// </summary>
        /// <param name="inventoryManager">The inventory manager.</param>
        /// <param name="cartServiceProvider">The cart service provider.</param>
        public CartManager([NotNull] InventoryManager inventoryManager, [NotNull] CommerceCartServiceProvider cartServiceProvider)
        {
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");
            Assert.ArgumentNotNull(cartServiceProvider, "cartServiceProvider");

            this.InventoryManager = inventoryManager;
            this.CartServiceProvider = cartServiceProvider;
        }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        /// <summary>
        /// Gets or sets the cart service provider.
        /// </summary>
        /// <value>
        /// The cart service provider.
        /// </value>
        public CartServiceProvider CartServiceProvider { get; protected set; }

        #region Methods (public, non-static)

        /// <summary>
        /// Returns the current user cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> GetCurrentCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, bool refresh = false)
        {
            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            string customerId = visitorContext.GetCustomerId();

            if (refresh)
            {
                cartCache.InvalidateCartCache(customerId);
            }

            var cart = cartCache.GetCart(customerId);
            if (cart != null)
            {
                return new ManagerResponse<CartResult, CommerceCart>(new CartResult { Cart = cart }, cart);
            }

            CartResult cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId, refresh);
            if (cartResult.Success && cartResult.Cart != null)
            {
                cart = cartResult.Cart as CommerceCart;
                cartResult.Cart = cart;
                cartCache.AddCartToCache(cart);
            }
            else
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
            }

            return new ManagerResponse<CartResult, CommerceCart>(cartResult, cart);
        }

        /// <summary>
        /// Updates the cart currency.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns>The manager response.</returns>
        public virtual ManagerResponse<CartResult, bool> UpdateCartCurrency([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string currencyCode)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(currencyCode, "currencyCode");

            var result = this.GetCurrentCart(storefront, visitorContext);
            if (!result.ServiceProviderResult.Success)
            {
                return new ManagerResponse<CartResult, bool>(new CartResult { Success = false }, false);
            }

            var cart = result.Result;
            var changes = new CommerceCart() { CurrencyCode = currencyCode };

            var updateCartResult = this.UpdateCart(storefront, visitorContext, cart, changes);
            if (updateCartResult.ServiceProviderResult.Success)
            {
                var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
                string customerId = visitorContext.GetCustomerId();

                cartCache.InvalidateCartCache(customerId);
            }

            return new ManagerResponse<CartResult, bool>(updateCartResult.ServiceProviderResult, updateCartResult.ServiceProviderResult.Success);
        }

        /// <summary>
        /// Adds the line item to cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModelList">The input model.</param>
        /// <returns>
        /// The manager response where the result is retuned indicating the success or failure of the operation.
        /// </returns>
        public virtual ManagerResponse<CartResult, bool> AddLineItemsToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, IEnumerable<AddCartLineInputModel> inputModelList)
        {
            Assert.ArgumentNotNull(inputModelList, "inputModelList");
            
            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId, false);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
                return new ManagerResponse<CartResult, bool>(cartResult, cartResult.Success);
            }

            var lines = new List<CartLine>();
            foreach (var inputModel in inputModelList)
            {
                Assert.ArgumentNotNullOrEmpty(inputModel.ProductId, "inputModel.ProductId");
                Assert.ArgumentNotNullOrEmpty(inputModel.CatalogName, "inputModel.CatalogName");
                Assert.ArgumentNotNull(inputModel.Quantity, "inputModel.Quantity");

                var quantity = (uint)inputModel.Quantity;
                var cartLine = new CommerceCartLine(inputModel.CatalogName, inputModel.ProductId, inputModel.VariantId == "-1" ? null : inputModel.VariantId, quantity);
                cartLine.Properties["ProductUrl"] = inputModel.ProductUrl;
                cartLine.Properties["ImageUrl"] = inputModel.ImageUrl;
                //// UpdateStockInformation(storefront, visitorContext, cartLine, inputModel.CatalogName);

                //// Special handling for a Gift Card
                if (inputModel.ProductId.Equals(storefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase))
                {
                    cartLine.Properties.Add("GiftCardAmount", inputModel.GiftCardAmount);
                    var totalQuantity = cartLine.Quantity;
                    if (totalQuantity > 1)
                    {
                        cartLine.Quantity = 1;
                        for (var i = 0; i < totalQuantity - 1; i++)
                        {
                            lines.Add(cartLine);
                        }
                    }
                }

                lines.Add(cartLine);
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            var cart = cartResult.Cart as CommerceCart;
            var addLinesRequest = new AddCartLinesRequest(cart, lines);
            addLinesRequest.RefreshCart(true);
            var addLinesResult = this.CartServiceProvider.AddCartLines(addLinesRequest);
            if (addLinesResult.Success && addLinesResult.Cart != null && addLinesResult.Cart.Properties["_Basket_Errors"] == null)
            {
                cartCache.AddCartToCache(addLinesResult.Cart as CommerceCart);
            }

            Helpers.LogSystemMessages(addLinesResult.SystemMessages, addLinesResult);
            return new ManagerResponse<CartResult, bool>(addLinesResult, addLinesResult.Success);
        }

        /// <summary>
        /// Removes the line item from cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="externalCartLineId">The external cart line identifier.</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemoveLineItemFromCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string externalCartLineId)
        {
            Assert.ArgumentNotNullOrEmpty(externalCartLineId, "externalCartLineId");

            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId, false);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage
                {
                    Message = message
                });
                return new ManagerResponse<CartResult, CommerceCart>(cartResult, cartResult.Cart as CommerceCart);
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            CommerceCart cart = cartResult.Cart as CommerceCart;
            var lineToRemove = cart.Lines.SingleOrDefault(cl => cl.ExternalCartLineId == externalCartLineId);
            if (lineToRemove == null)
            {
                return new ManagerResponse<CartResult, CommerceCart>(new CartResult { Success = true }, cart);
            }

            var removeLinesRequest = new RemoveCartLinesRequest(cart, new[] { new CartLine { ExternalCartLineId = externalCartLineId, Quantity = lineToRemove.Quantity, Product = lineToRemove.Product } });
            removeLinesRequest.RefreshCart(true);
            var removeLinesResult = this.CartServiceProvider.RemoveCartLines(removeLinesRequest);
            if (removeLinesResult.Success && removeLinesResult.Cart != null)
            {
                cartCache.AddCartToCache(removeLinesResult.Cart as CommerceCart);
            }

            Helpers.LogSystemMessages(removeLinesResult.SystemMessages, removeLinesResult);
            return new ManagerResponse<CartResult, CommerceCart>(removeLinesResult, removeLinesResult.Cart as CommerceCart);
        }

        /// <summary>
        /// Changes the line quantity.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> ChangeLineQuantity([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] UpdateCartLineInputModel inputModel)
        {
            Assert.ArgumentNotNull(inputModel, "inputModel");
            Assert.ArgumentNotNullOrEmpty(inputModel.ExternalCartLineId, "inputModel.ExternalCartLineId");

            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage
                {
                    Message = message
                });
                return new ManagerResponse<CartResult, CommerceCart>(cartResult, cartResult.Cart as CommerceCart);
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            var cart = cartResult.Cart;
            var result = new CartResult { Cart = cart, Success = true };
            var cartLineToChange = cart.Lines.SingleOrDefault(cl => cl.Product != null && cl.ExternalCartLineId == inputModel.ExternalCartLineId);
            if (inputModel.Quantity == 0 && cartLineToChange != null)
            {
                result = this.RemoveCartLines(cart, new[] { cartLineToChange }, true);
            }
            else if (cartLineToChange != null)
            {
                cartLineToChange.Quantity = inputModel.Quantity;
                var request = new UpdateCartLinesRequest(cart, new[] { cartLineToChange });
                request.RefreshCart(true);
                result = this.CartServiceProvider.UpdateCartLines(request);
            }

            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Adds the promo code to cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<AddPromoCodeResult, CommerceCart> AddPromoCodeToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string promoCode)
        {
            Assert.ArgumentNotNullOrEmpty(promoCode, "promoCode");

            AddPromoCodeResult result = new AddPromoCodeResult { Success = false };
            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage
                {
                    Message = message
                });
                return new ManagerResponse<AddPromoCodeResult, CommerceCart>(result, cartResult.Cart as CommerceCart);
            }
            
            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());
            
            var cart = cartResult.Cart as CommerceCart;
            var request = new AddPromoCodeRequest(cart, promoCode);
            request.RefreshCart(true);
            result = ((CommerceCartServiceProvider)this.CartServiceProvider).AddPromoCode(request);
            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<AddPromoCodeResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Removes the promo code from cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="promoCode">The promo code.</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<RemovePromoCodeResult, CommerceCart> RemovePromoCodeFromCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string promoCode)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(promoCode, "promoCode");

            var result = new RemovePromoCodeResult { Success = false };
            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage
                {
                    Message = message
                });
                return new ManagerResponse<RemovePromoCodeResult, CommerceCart>(result, cartResult.Cart as CommerceCart);
            }
            
            var cart = cartResult.Cart as CommerceCart;

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());
            
            var request = new RemovePromoCodeRequest(cart, promoCode);
            result = ((CommerceCartServiceProvider)this.CartServiceProvider).RemovePromoCode(request);
            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }
            
            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<RemovePromoCodeResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Sets the shipping methods.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The manager response where the modified CommerceCart is returned in the Result.
        /// </returns>
        public virtual ManagerResponse<AddShippingInfoResult, CommerceCart> SetShippingMethods([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] SetShippingMethodsInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");

            var result = new AddShippingInfoResult { Success = false };
            var response = this.GetCurrentCart(storefront, visitorContext, true);
            if (!response.ServiceProviderResult.Success || response.Result == null)
            {
                return new ManagerResponse<AddShippingInfoResult, CommerceCart>(result, response.Result);
            }

            var cart = (CommerceCart)response.ServiceProviderResult.Cart;
            if (inputModel.ShippingAddresses != null && inputModel.ShippingAddresses.Any())
            {
                var cartParties = cart.Parties.ToList();
                cartParties.AddRange(inputModel.ShippingAddresses.ToParties());
                cart.Parties = cartParties.AsReadOnly();
            }

            var internalShippingList = inputModel.ShippingMethods.ToShippingInfoList();
            var orderPreferenceType = InputModelExtension.GetShippingOptionType(inputModel.OrderShippingPreferenceType);
            if (orderPreferenceType != ShippingOptionType.DeliverItemsIndividually)
            {
                foreach (var shipping in internalShippingList)
                {
                    shipping.LineIDs = (from CommerceCartLine lineItem in cart.Lines select lineItem.ExternalCartLineId).ToList().AsReadOnly();
                }
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());
            
            result = this.AddShippingInfoToCart(cart, orderPreferenceType, internalShippingList);
            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            return new ManagerResponse<AddShippingInfoResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Merges the carts.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="anonymousVisitorId">The anonymous visitor identifier.</param>
        /// <param name="anonymousVisitorCart">The anonymous visitor cart.</param>
        /// <returns>
        /// The manager response where the merged cart is returned in the result.
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> MergeCarts([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string anonymousVisitorId, Cart anonymousVisitorCart)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(anonymousVisitorId, "anonymousVisitorId");

            var userId = visitorContext.UserId;
            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, userId);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CartNotFoundError);
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
                return new ManagerResponse<CartResult, CommerceCart>(cartResult, cartResult.Cart as CommerceCart);
            }

            CommerceCart currentCart = (CommerceCart)cartResult.Cart;
            var result = new CartResult { Cart = currentCart, Success = true };

            if (userId != anonymousVisitorId)
            {
                if (anonymousVisitorCart != null && anonymousVisitorCart.Lines.Any())
                {
                    if ((currentCart.ShopName == anonymousVisitorCart.ShopName) || (currentCart.ExternalId != anonymousVisitorCart.ExternalId))
                    {
                        var mergeCartRequest = new MergeCartRequest(anonymousVisitorCart, currentCart);
                        result = this.CartServiceProvider.MergeCart(mergeCartRequest);
                    }
                }
            }

            if (result.Success && result.Cart != null)
            {
                var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
                cartCache.InvalidateCartCache(userId);
                cartCache.InvalidateCartCache(anonymousVisitorId);
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Updates the cart.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <param name="cartChanges">The cart changes.</param>
        /// <returns>The manager response with the updated cart.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> UpdateCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] CommerceCart cartChanges)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(cartChanges, "cartChanges");

            var updateCartRequest = new UpdateCartRequest(cart, cartChanges);
            var result = this.CartServiceProvider.UpdateCart(updateCartRequest);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        #endregion

        #region Methods (protected, virtual)

        /// <summary>
        /// Loads the the cart by given its cart name.
        /// </summary>
        /// <param name="shopName">Name of the shop.</param>
        /// <param name="cartName">Name of the cart.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="refreshCart">if set to <c>true</c> [refresh cart].</param>
        /// <returns>The cart result.</returns>
        protected virtual CartResult LoadCartByName(string shopName, string cartName, string userName, bool refreshCart = false)
        {
            Assert.ArgumentNotNull(shopName, "shopName");
            Assert.ArgumentNotNull(cartName, "cartName");
            Assert.ArgumentNotNull(userName, "userName");

            var request = new LoadCartByNameRequest(shopName, cartName, userName);
            request.RefreshCart(refreshCart);
            var result = this.CartServiceProvider.LoadCart(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Removes product from the visitor's cart.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="cartLines">The cart lines.</param>
        /// <param name="refreshCart">if set to <c>true</c> [refresh cart].</param>
        /// <returns>
        /// the cart result.
        /// </returns>
        protected virtual CartResult RemoveCartLines(Cart cart, IEnumerable<CartLine> cartLines, bool refreshCart = false)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(cartLines, "cartLine");

            var request = new RemoveCartLinesRequest(cart, cartLines);
            request.RefreshCart(refreshCart);
            var result = this.CartServiceProvider.RemoveCartLines(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Adds a party to a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="party">the party info</param>
        /// <returns>the updated cart</returns>
        protected virtual CartResult AddPartyToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] CommerceParty party)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(party, "party");

            var request = new AddPartiesRequest(cart, new List<Party> { party });
            var result = this.CartServiceProvider.AddParties(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Removes party info from a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="party">the party info</param>
        /// <returns>the updated cart</returns>
        protected virtual CartResult RemovePartyFromCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] CommerceParty party)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(party, "party");

            var request = new RemovePartiesRequest(cart, new List<Party> { party });
            var result = this.CartServiceProvider.RemoveParties(request); 
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }
            
            return result;
        }

        /// <summary>
        /// Updates party info on a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="parties">the party info</param>
        /// <returns>the updated cart</returns>
        protected virtual CartResult UpdatePartiesInCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] List<CommerceParty> parties)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(parties, "parties");

            var request = new UpdatePartiesRequest(cart, parties.Cast<Party>().ToList());
            var result = this.CartServiceProvider.UpdateParties(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Adds payment info to a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="info">the payment info</param>
        /// <param name="party">the party info</param>
        /// <param name="refreshCart">if set to <c>true</c> the cart will be re-calculated using the Commerce Server pipelines.</param>
        /// <returns>
        /// the updated cart
        /// </returns>
        protected virtual CartResult AddPaymentInfoToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] PaymentInfo info, [NotNull] CommerceParty party, bool refreshCart = false)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(info, "info");
            Assert.ArgumentNotNull(party, "party");
            Assert.ArgumentCondition(info is CommerceCreditCardPaymentInfo, "info", "info is CommerceCreditCardPaymentInfo");

            var creditCardInfo = info as CommerceCreditCardPaymentInfo;
            if (creditCardInfo != null)
            {
                creditCardInfo.PartyID = party.ExternalId;
                creditCardInfo.Amount = cart.Total.Amount;
            }

            var request = new AddPaymentInfoRequest(cart, new List<PaymentInfo> { creditCardInfo });
            request.RefreshCart(refreshCart);
            var result = this.CartServiceProvider.AddPaymentInfo(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Removes Payment info from a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="info">the payment info</param>
        /// <param name="refreshCart">if set to <c>true</c> the cart will be re-calculated using the Commerce Server pipelines.</param>
        /// <returns>
        /// the updated cart
        /// </returns>
        protected virtual CartResult RemovePaymentInfoFromCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] List<PaymentInfo> info, bool refreshCart = false)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(info, "info");

            var request = new RemovePaymentInfoRequest(cart, info);
            request.RefreshCart(refreshCart);
            var result = this.CartServiceProvider.RemovePaymentInfo(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Adds shipping info to a cart
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="orderShippingPreferenceType">Type of the order shipping preference.</param>
        /// <param name="shipments">The shipments.</param>
        /// <returns>
        /// the updated cart
        /// </returns>
        protected virtual AddShippingInfoResult AddShippingInfoToCart([NotNull] CommerceCart cart, [NotNull] ShippingOptionType orderShippingPreferenceType, [NotNull] IEnumerable<ShippingInfo> shipments)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(orderShippingPreferenceType, "orderShippingPreferenceType");
            Assert.ArgumentNotNull(shipments, "shipments");

            var request = new AddShippingInformationRequest(cart, shipments.ToList(), orderShippingPreferenceType);
            var result = this.CartServiceProvider.AddShippingInfo(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return result;
        }

        /// <summary>
        /// Updates the stock information.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cartLine">The cart line.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        protected virtual void UpdateStockInformation([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCartLine cartLine, [NotNull] string catalogName)
        {
            Assert.ArgumentNotNull(cartLine, "cartLine");

            var products = new List<InventoryProduct> { new CommerceInventoryProduct { ProductId = cartLine.Product.ProductId, CatalogName = catalogName } };
            var stockInfoResult = this.InventoryManager.GetStockInformation(storefront, products, StockDetailsLevel.Status).ServiceProviderResult;
            if (stockInfoResult.StockInformation == null || !stockInfoResult.StockInformation.Any())
            {
                return;
            }

            var stockInfo = stockInfoResult.StockInformation.FirstOrDefault();
            var orderableInfo = new OrderableInformation();
            if (stockInfo != null && stockInfo.Status != null)
            {
                if (Equals(stockInfo.Status, StockStatus.PreOrderable))
                {
                    var preOrderableResult = this.InventoryManager.GetPreOrderableInformation(storefront, products).ServiceProviderResult;
                    if (preOrderableResult.OrderableInformation != null && preOrderableResult.OrderableInformation.Any())
                    {
                        orderableInfo = preOrderableResult.OrderableInformation.FirstOrDefault();
                    }
                }
                else if (Equals(stockInfo.Status, StockStatus.BackOrderable))
                {
                    var backOrderableResult = this.InventoryManager.GetBackOrderableInformation(storefront, products).ServiceProviderResult;
                    if (backOrderableResult.OrderableInformation != null && backOrderableResult.OrderableInformation.Any())
                    {
                        orderableInfo = backOrderableResult.OrderableInformation.FirstOrDefault();
                    }
                }
            }

            if (stockInfo != null)
            {
                cartLine.Product.StockStatus = stockInfo.Status;
            }

            if (orderableInfo == null)
            {
                return;
            }

            cartLine.Product.InStockDate = orderableInfo.InStockDate;
            cartLine.Product.ShippingDate = orderableInfo.ShippingDate;
        }

        #endregion
    }
}