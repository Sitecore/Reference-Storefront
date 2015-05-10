//-----------------------------------------------------------------------
// <copyright file="CartManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The manager class responsible for encapsulating the cart business logic for the site.</summary>
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Sitecore.Commerce.Connect.CommerceServer.Inventory.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Entities.Carts;
    using Sitecore.Commerce.Entities.Inventory;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Services;
    using Sitecore.Globalization;
    using RefSFModels = Sitecore.Reference.Storefront.Connect.Models;
    using System.Globalization;

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
                var result = new CartResult { Cart = cart };
                this.AddBasketErrorsToResult(result.Cart as CommerceCart, result);
                return new ManagerResponse<CartResult, CommerceCart>(result, cart);
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
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
            }

            this.AddBasketErrorsToResult(cartResult.Cart as CommerceCart, cartResult);

            return new ManagerResponse<CartResult, CommerceCart>(cartResult, cart);
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
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
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

                //// Special handling for a Gift Card
                if (inputModel.ProductId.Equals(storefront.GiftCardProductId, StringComparison.OrdinalIgnoreCase))
                {
                    inputModel.VariantId = inputModel.GiftCardAmount.Value.ToString("000", CultureInfo.InvariantCulture);
                }

                var cartLine = new CommerceCartLine(inputModel.CatalogName, inputModel.ProductId, inputModel.VariantId == "-1" ? null : inputModel.VariantId, quantity);
                cartLine.Properties["ProductUrl"] = inputModel.ProductUrl;
                cartLine.Properties["ImageUrl"] = inputModel.ImageUrl;
                //// UpdateStockInformation(storefront, visitorContext, cartLine, inputModel.CatalogName);      

                lines.Add(cartLine);
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            var cart = cartResult.Cart as CommerceCart;
            var addLinesRequest = new AddCartLinesRequest(cart, lines);
            addLinesRequest.RefreshCart(true);
            var addLinesResult = this.CartServiceProvider.AddCartLines(addLinesRequest);
            if (addLinesResult.Success && addLinesResult.Cart != null)
            {
                cartCache.AddCartToCache(addLinesResult.Cart as CommerceCart);
            }

            this.AddBasketErrorsToResult(addLinesResult.Cart as CommerceCart, addLinesResult);

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

            // We remove the curly brackets from the JSON for the lineitem id.  We need to add them back.
            externalCartLineId = Guid.Parse(externalCartLineId).ToString("B");

            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, visitorContext.UserId, false);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
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

            var removeLinesRequest = new RemoveCartLinesRequest(cart, new[] { lineToRemove });
            removeLinesRequest.RefreshCart(true);
            var removeLinesResult = this.CartServiceProvider.RemoveCartLines(removeLinesRequest);
            if (removeLinesResult.Success && removeLinesResult.Cart != null)
            {
                cartCache.AddCartToCache(removeLinesResult.Cart as CommerceCart);
            }

            this.AddBasketErrorsToResult(removeLinesResult.Cart as CommerceCart, removeLinesResult);

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
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
                return new ManagerResponse<CartResult, CommerceCart>(cartResult, cartResult.Cart as CommerceCart);
            }

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            // We remove the curly brackets from the JSON for the lineitem id.  We need to add them back.
            inputModel.ExternalCartLineId = Guid.Parse(inputModel.ExternalCartLineId).ToString("B");

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

            this.AddBasketErrorsToResult(result.Cart as CommerceCart, result);

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
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
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
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
                return new ManagerResponse<RemovePromoCodeResult, CommerceCart>(result, cartResult.Cart as CommerceCart);
            }

            var cart = cartResult.Cart as CommerceCart;

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            var request = new RemovePromoCodeRequest(cart, promoCode);
            request.RefreshCart(true);  // We need the CS pipelines to run.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
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

            CommerceParty allShippingParty = null;

            var emailPartyList = this.GetEmailAddressPartiesFromShippingMethods(inputModel.ShippingMethods);

            var cart = (CommerceCart)response.ServiceProviderResult.Cart;
            if ((inputModel.ShippingAddresses != null && inputModel.ShippingAddresses.Any()) || (emailPartyList != null && emailPartyList.Any()))
            {
                var addPartiesResult = this.SetShippingAddresses(storefront, visitorContext, cart, inputModel.ShippingAddresses, emailPartyList);
                if (!addPartiesResult.Success)
                {
                    result.SystemMessages.Add(addPartiesResult.SystemMessages[0]);
                    return new ManagerResponse<AddShippingInfoResult, CommerceCart>(result, result.Cart as CommerceCart);
                }

                cart = (CommerceCart)addPartiesResult.Cart;
                allShippingParty = (CommerceParty)addPartiesResult.Parties.ElementAt(0);
            }

            var internalShippingList = inputModel.ShippingMethods.ToShippingInfoList();
            var orderPreferenceType = InputModelExtension.GetShippingOptionType(inputModel.OrderShippingPreferenceType);
            if (orderPreferenceType != RefSFModels.ShippingOptionType.DeliverItemsIndividually)
            {
                foreach (var shipping in internalShippingList)
                {
                    shipping.LineIDs = (from CommerceCartLine lineItem in cart.Lines select lineItem.ExternalCartLineId).ToList().AsReadOnly();
                    shipping.PartyID = allShippingParty.ExternalId;
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
        /// Sets the payment methods.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The input model.</param>
        /// <returns>The manager response with a cart in the result.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> SetPaymentMethods([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] PaymentInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");

            var addPaymentResponse = new ManagerResponse<CartResult, CommerceCart>(new AddPaymentInfoResult { Success = true }, null);
            var errorResponse = new AddPaymentInfoResult { Success = false };
            var response = this.GetCurrentCart(storefront, visitorContext, true);
            if (!response.ServiceProviderResult.Success || response.Result == null)
            {
                return new ManagerResponse<CartResult, CommerceCart>(errorResponse, response.Result);
            }

            var cart = (CommerceCart)response.ServiceProviderResult.Cart;
            if (inputModel.CreditCardPayment != null && !string.IsNullOrEmpty(inputModel.CreditCardPayment.PaymentMethodID) && inputModel.BillingAddress != null)
            {
                var removeBillingResponse = this.RemoveAllBillingParties(storefront, visitorContext, cart);
                if (!removeBillingResponse.ServiceProviderResult.Success)
                {
                    errorResponse.SystemMessages.Add(removeBillingResponse.ServiceProviderResult.SystemMessages[0]);
                    return new ManagerResponse<CartResult, CommerceCart>(errorResponse, errorResponse.Cart as CommerceCart);
                }

                cart = removeBillingResponse.Result;

                CommerceParty billingParty = inputModel.BillingAddress.ToNewBillingParty();

                var addPartyResponse = this.AddPartyToCart(storefront, visitorContext, cart, billingParty);
                if (!addPartyResponse.ServiceProviderResult.Success)
                {
                    errorResponse.SystemMessages.Add(addPartyResponse.ServiceProviderResult.SystemMessages[0]);
                    return new ManagerResponse<CartResult, CommerceCart>(errorResponse, errorResponse.Cart as CommerceCart);
                }

                cart = addPartyResponse.Result;

                var removePaymentResponse = this.RemoveAllPaymentMethods(storefront, visitorContext, cart);
                if (!removePaymentResponse.ServiceProviderResult.Success)
                {
                    errorResponse.SystemMessages.Add(removePaymentResponse.ServiceProviderResult.SystemMessages[0]);
                    return new ManagerResponse<CartResult, CommerceCart>(errorResponse, errorResponse.Cart as CommerceCart);
                }

                cart = removePaymentResponse.Result;

                CommerceCreditCardPaymentInfo creditCardPayment = inputModel.CreditCardPayment.ToCreditCardPaymentInfo();

                addPaymentResponse = this.AddPaymentInfoToCart(storefront, visitorContext, cart, creditCardPayment, billingParty, true);
                if (!addPaymentResponse.ServiceProviderResult.Success)
                {
                    errorResponse.SystemMessages.Add(addPaymentResponse.ServiceProviderResult.SystemMessages[0]);
                    return new ManagerResponse<CartResult, CommerceCart>(errorResponse, errorResponse.Cart as CommerceCart);
                }

                cart = addPaymentResponse.Result;
            }

            return addPaymentResponse;
        }

        /// <summary>
        /// Merges the carts.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="anonymousVisitorId">The anonymous visitor identifier.</param>
        /// <returns>
        /// The manager response where the merged cart is returned in the result.
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> MergeCarts([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, string anonymousVisitorId)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(anonymousVisitorId, "anonymousVisitorId");

            var userId = visitorContext.UserId;
            var cartResult = this.LoadCartByName(storefront.ShopName, storefront.DefaultCartName, userId, true);
            if (!cartResult.Success || cartResult.Cart == null)
            {
                var message = StorefrontManager.GetSystemMessage("CartNotFoundError");
                cartResult.SystemMessages.Add(new SystemMessage { Message = message });
                return new ManagerResponse<CartResult, CommerceCart>(cartResult, cartResult.Cart as CommerceCart);
            }

            CommerceCart currentCart = (CommerceCart)cartResult.Cart;
            var result = new CartResult { Cart = currentCart, Success = true };
            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();

            if (userId != anonymousVisitorId)
            {
                cartCache.InvalidateCartCache(userId);
                CommerceCart anonymousCart = cartCache.GetCart(anonymousVisitorId);

                if (anonymousCart != null && anonymousCart.Lines.Any())
                {
                    cartCache.InvalidateCartCache(anonymousVisitorId);
                    if ((currentCart.ShopName == anonymousCart.ShopName) || (currentCart.ExternalId != anonymousCart.ExternalId))
                    {
                        var mergeCartRequest = new MergeCartRequest(anonymousCart, currentCart);
                        result = this.CartServiceProvider.MergeCart(mergeCartRequest);
                        if (result.Success)
                        {
                            var updateCartRequest = new UpdateCartLinesRequest(result.Cart, anonymousCart.Lines);
                            updateCartRequest.RefreshCart(true);
                            result = this.CartServiceProvider.UpdateCartLines(updateCartRequest);
                            if (result.Success)
                            {
                                this.CartServiceProvider.DeleteCart(new DeleteCartRequest(anonymousCart));
                            }
                        }
                    }
                }
            }

            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Adds a party to a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="party">the party info</param>
        /// <returns>the updated cart</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> AddPartyToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] CommerceParty party)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(party, "party");

            var request = new AddPartiesRequest(cart, new List<Party> { party });
            var result = this.CartServiceProvider.AddParties(request);

            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Removes party info from a cart
        /// </summary>
        /// <param name="storefront">The Storefront Context</param>
        /// <param name="visitorContext">The Visitor Context</param>
        /// <param name="cart">the cart</param>
        /// <param name="parties">The parties.</param>
        /// <returns>
        /// the updated cart
        /// </returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemovePartiesFromCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] List<Party> parties)
        {
            Assert.ArgumentNotNull(cart, "cart");
            Assert.ArgumentNotNull(parties, "parties");

            var request = new RemovePartiesRequest(cart, parties);
            var result = this.CartServiceProvider.RemoveParties(request);
            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Removes all shipping parties.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <returns>The manager response with a cart in the result.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemoveAllShippingParties([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(cart, "cart");

            var parties = this.GetPartiesForPrefix(cart, CommerceServerStorefrontConstants.CartConstants.ShippingAddressNamePrefix);
            var request = new RemovePartiesRequest(cart, parties);
            var response = this.CartServiceProvider.RemoveParties(request);

            return new ManagerResponse<CartResult, CommerceCart>(response, response.Cart as CommerceCart);
        }

        /// <summary>
        /// Removes all billing parties.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <returns>The manager response with a cart in the result.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemoveAllBillingParties([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(cart, "cart");

            var parties = this.GetPartiesForPrefix(cart, CommerceServerStorefrontConstants.CartConstants.BillingAddressNamePrefix);
            var request = new RemovePartiesRequest(cart, parties);
            var response = this.CartServiceProvider.RemoveParties(request);

            return new ManagerResponse<CartResult, CommerceCart>(response, response.Cart as CommerceCart);
        }

        /// <summary>
        /// Removes all payment methods.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <returns>The manager response with a cart in the result.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemoveAllPaymentMethods([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(cart, "cart");

            if (cart.Payment != null && cart.Payment.Any())
            {
                var request = new RemovePaymentInfoRequest(cart, cart.Payment.ToList());
                var response = this.CartServiceProvider.RemovePaymentInfo(request);

                return new ManagerResponse<CartResult, CommerceCart>(response, response.Cart as CommerceCart);
            }

            return new ManagerResponse<CartResult, CommerceCart>(new RemovePaymentInfoResult { Success = true }, cart);
        }

        /// <summary>
        /// Removes all shipping methods.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <returns>The manager response with the cart result and returned cart.</returns>
        public virtual ManagerResponse<CartResult, CommerceCart> RemoveAllShippingMethods([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(cart, "cart");

            if (cart.Shipping != null && cart.Shipping.Any())
            {
                var request = new RemoveShippingInfoRequest(cart, cart.Shipping.ToList());
                var response = this.CartServiceProvider.RemoveShippingInfo(request);

                return new ManagerResponse<CartResult, CommerceCart>(response, response.Cart as CommerceCart);
            }

            return new ManagerResponse<CartResult, CommerceCart>(new RemoveShippingInfoResult { Success = true }, cart);
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
        public virtual ManagerResponse<CartResult, CommerceCart> AddPaymentInfoToCart([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] CommerceCart cart, [NotNull] PaymentInfo info, [NotNull] CommerceParty party, bool refreshCart = false)
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
            var request = new LoadCartByNameRequest(shopName, cartName, userName);
            request.RefreshCart(refreshCart);
            var result = this.CartServiceProvider.LoadCart(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
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

            Helpers.LogSystemMessages(result.SystemMessages, result);
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

            var request = new AddShippingInfoRequest(cart, shipments.ToList());
            request.RefreshCart(true);
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

        /// <summary>
        /// Gets the email address parties from shipping methods.
        /// </summary>
        /// <param name="inputModelList">The input model list.</param>
        /// <returns>A list of email parties if the email shipping method has been specified.</returns>
        protected virtual List<RefSFModels.EmailParty> GetEmailAddressPartiesFromShippingMethods(List<ShippingMethodInputModelItem> inputModelList)
        {
            List<RefSFModels.EmailParty> emailPartyList = null;

            if (inputModelList != null && inputModelList.Any())
            {
                int i = 1;
                foreach (var inputModel in inputModelList)
                {
                    if (ShippingOptionType.ElectronicDelivery == Convert.ToInt32(inputModel.ShippingPreferenceType, CultureInfo.InvariantCulture))
                    {
                        if (emailPartyList == null)
                        {
                            emailPartyList = new List<RefSFModels.EmailParty>();
                        }

                        RefSFModels.EmailParty party = new RefSFModels.EmailParty();

                        party.ExternalId = Guid.NewGuid().ToString();
                        party.Name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", CommerceServerStorefrontConstants.CartConstants.EmailAddressNamePrefix, i);
                        party.Email = inputModel.ElectronicDeliveryEmail;
                        party.Text = inputModel.ElectronicDeliveryEmailContent;

                        emailPartyList.Add(party);

                        // Set the party id to the newly created email party in order to create the association in CS.
                        inputModel.PartyID = party.ExternalId;

                        i++;
                    }
                }
            }

            return emailPartyList;
        }

        /// <summary>
        /// Sets the shipping addresses.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="cart">The cart.</param>
        /// <param name="shippingAddresses">The shipping addresses.</param>
        /// <param name="emailPartyList">The email party list.</param>
        /// <returns>
        /// The manager response with a cart in the result.
        /// </returns>
        protected virtual AddPartiesResult SetShippingAddresses(CommerceStorefront storefront, VisitorContext visitorContext, CommerceCart cart, List<PartyInputModelItem> shippingAddresses, List<RefSFModels.EmailParty> emailPartyList)
        {
            var errorResult = new AddPartiesResult { Success = false };

            var removePartiesResponse = this.RemoveAllShippingParties(storefront, visitorContext, cart);
            if (!removePartiesResponse.ServiceProviderResult.Success)
            {
                errorResult.SystemMessages.Add(removePartiesResponse.ServiceProviderResult.SystemMessages[0]);
                return errorResult;
            }

            List<Party> partyList = new List<Party>();

            if (shippingAddresses != null && shippingAddresses.Any())
            {
                partyList.AddRange(shippingAddresses.ToNewShippingParties());
            }

            if (emailPartyList != null && emailPartyList.Any())
            {
                partyList.AddRange(emailPartyList.Cast<Party>().ToList());
            }

            AddPartiesRequest addPartiesRequest = new AddPartiesRequest(cart, partyList);
            var addPartiesResponse = this.CartServiceProvider.AddParties(addPartiesRequest);

            return addPartiesResponse;
        }

        /// <summary>
        /// Gets the parties for prefix.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The list of Party instances that match the given prefix value.</returns>
        protected virtual List<Party> GetPartiesForPrefix(CommerceCart cart, string prefix)
        {
            List<Party> partyList = new List<Party>();

            foreach (Party party in cart.Parties)
            {
                if (party is CommerceParty)
                {
                    if (((CommerceParty)party).Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        partyList.Add(party);
                    }
                }
                else if (party is RefSFModels.EmailParty)
                {
                    if (((RefSFModels.EmailParty)party).Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        partyList.Add(party);
                    }
                }
            }

            return partyList;
        }

        /// <summary>
        /// Adds the basket errors to result.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <param name="result">The result.</param>
        protected virtual void AddBasketErrorsToResult(CommerceCart cart, ServiceProviderResult result)
        {
            if (cart != null && cart.Properties["_Basket_Errors"] != null)
            {
                List<string> basketErrors = cart.Properties["_Basket_Errors"] as List<string>;
                basketErrors.ForEach(m => result.SystemMessages.Add(new SystemMessage { Message = m }));
            }
        }
        #endregion
    }
}