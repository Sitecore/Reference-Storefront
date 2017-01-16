//-----------------------------------------------------------------------
// <copyright file="OrderManager.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The manager class responsible for encapsulating the order business logic for the site.</summary>
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
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Orders;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;

    /// <summary>
    /// Defines the OrderManager class.
    /// </summary>
    public class OrderManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManager" /> class.
        /// </summary>
        /// <param name="orderServiceProvider">The order service provider.</param>
        /// <param name="cartManager">The cart manager.</param>
        public OrderManager(CommerceOrderServiceProvider orderServiceProvider, [NotNull] CartManager cartManager)
        {
            Assert.ArgumentNotNull(orderServiceProvider, "orderServiceProvider");
            Assert.ArgumentNotNull(cartManager, "cartManager");

            this.OrderServiceProvider = orderServiceProvider;
            this.CartManager = cartManager;
        }

        /// <summary>
        /// Gets or sets the order service provider.
        /// </summary>
        /// <value>
        /// The order service provider.
        /// </value>
        public OrderServiceProvider OrderServiceProvider { get; protected set; }

        /// <summary>
        /// Gets or sets the cart manager.
        /// </summary>
        /// <value>
        /// The cart manager.
        /// </value>
        public CartManager CartManager { get; protected set; }

        /// <summary>
        /// Submits the visitor order.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The manager response where the new CommerceOrder is returned in the Result.
        /// </returns>
        public ManagerResponse<SubmitVisitorOrderResult, CommerceOrder> SubmitVisitorOrder([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] SubmitOrderInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");

            SubmitVisitorOrderResult errorResult = new SubmitVisitorOrderResult { Success = false };

            var response = this.CartManager.GetCurrentCart(storefront, visitorContext, true);
            if (!response.ServiceProviderResult.Success || response.Result == null)
            {
                response.ServiceProviderResult.SystemMessages.ToList().ForEach(m => errorResult.SystemMessages.Add(m));
                return new ManagerResponse<SubmitVisitorOrderResult, CommerceOrder>(errorResult, null);
            }

            var cart = (CommerceCart)response.ServiceProviderResult.Cart;

            if (cart.Lines.Count == 0)
            {
                errorResult.SystemMessages.Add(new SystemMessage { Message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.SubmitOrderHasEmptyCart) });
                return new ManagerResponse<SubmitVisitorOrderResult, CommerceOrder>(errorResult, null);
            }

            cart.Email = inputModel.UserEmail;

            var request = new SubmitVisitorOrderRequest(cart);
            request.RefreshCart(true);
            errorResult = this.OrderServiceProvider.SubmitVisitorOrder(request);
            if (errorResult.Success && errorResult.Order != null && errorResult.CartWithErrors == null)
            {
                var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
                cartCache.InvalidateCartCache(visitorContext.GetCustomerId());
            }

            Helpers.LogSystemMessages(errorResult.SystemMessages, errorResult);
            return new ManagerResponse<SubmitVisitorOrderResult, CommerceOrder>(errorResult, errorResult.Order as CommerceOrder);
        }

        /// <summary>
        /// Gets the available states.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns>
        /// The manager response where the available states are returned in the Result.
        /// </returns>
        public ManagerResponse<GetAvailableRegionsResult, Dictionary<string, string>> GetAvailableRegions([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string countryCode)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(countryCode, "countryCode");

            var request = new GetAvailableRegionsRequest(countryCode);
            var result = ((CommerceOrderServiceProvider)this.OrderServiceProvider).GetAvailableRegions(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetAvailableRegionsResult, Dictionary<string, string>>(result, new Dictionary<string, string>(result.AvailableRegions));
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="shopName">Name of the shop.</param>
        /// <returns>The manager response where list of order headers are returned in the Result.</returns>
        public ManagerResponse<GetVisitorOrdersResult, IEnumerable<OrderHeader>> GetOrders(string customerId, string shopName)
        {
            Assert.ArgumentNotNullOrEmpty(customerId, "customerId");
            Assert.ArgumentNotNullOrEmpty(shopName, "shopName");

            var request = new GetVisitorOrdersRequest(customerId, shopName);
            var result = this.OrderServiceProvider.GetVisitorOrders(request);
            if (result.Success && result.OrderHeaders != null && result.OrderHeaders.Count > 0)
            {
                return new ManagerResponse<GetVisitorOrdersResult, IEnumerable<OrderHeader>>(result, result.OrderHeaders.ToList());
            }

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetVisitorOrdersResult, IEnumerable<OrderHeader>>(result, new List<OrderHeader>());
        }

        /// <summary>
        /// Reorders one or more items from an existing order.
        /// </summary>
        /// <param name="storefront">The storefront context.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The reorder input model.</param>
        /// <returns>The result of the operation.</returns>
        public ManagerResponse<CartResult, CommerceCart> Reorder([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, ReorderInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");
            Assert.ArgumentNotNullOrEmpty(inputModel.OrderId, "inputModel.OrderId");

            var request = new ReorderByCartNameRequest()
            {
                CustomerId = visitorContext.GetCustomerId(),
                OrderId = inputModel.OrderId,
                ReorderLineExternalIds = inputModel.ReorderLineExternalIds,
                CartName = storefront.DefaultCartName,
                OrderShippingPreferenceType = ShippingOptionType.ShipToAddress,
            };

            var cartCache = CommerceTypeLoader.CreateInstance<CartCacheHelper>();
            cartCache.InvalidateCartCache(visitorContext.GetCustomerId());

            var result = this.OrderServiceProvider.Reorder(request);

            if (result.Success && result.Cart != null)
            {
                cartCache.AddCartToCache(result.Cart as CommerceCart);
            }

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<CartResult, CommerceCart>(result, result.Cart as CommerceCart);
        }

        /// <summary>
        /// Cancels one or more items from an existing order.
        /// </summary>
        /// <param name="storefront">The storefront context.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="inputModel">The order cancellation input model.</param>
        /// <returns>The result of the operation.</returns>
        public ManagerResponse<VisitorCancelOrderResult, bool> CancelOrder([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, CancelOrderInputModel inputModel)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNull(inputModel, "inputModel");
            Assert.ArgumentNotNullOrEmpty(inputModel.OrderId, "inputModel.OrderId");
            
            var request = new VisitorCancelOrderRequest(inputModel.OrderId, visitorContext.GetCustomerId(), storefront.ShopName);
            request.OrderLineExternalIds = inputModel.OrderLineExternalIds;
            var result = this.OrderServiceProvider.VisitorCancelOrder(request);

            if (!result.Success)
            {
                Helpers.LogSystemMessages(result.SystemMessages, result);
            }

            return new ManagerResponse<VisitorCancelOrderResult, bool>(result, result.Success);
        }

        /// <summary>
        /// Gets the order details.
        /// </summary>
        /// <param name="storefront">The storefront.</param>
        /// <param name="visitorContext">The visitor context.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// The manager response where the order detail returned in the Result.
        /// </returns>
        public ManagerResponse<GetVisitorOrderResult, CommerceOrder> GetOrderDetails([NotNull] CommerceStorefront storefront, [NotNull] VisitorContext visitorContext, [NotNull] string orderId)
        {
            Assert.ArgumentNotNull(storefront, "storefront");
            Assert.ArgumentNotNull(visitorContext, "visitorContext");
            Assert.ArgumentNotNullOrEmpty(orderId, "orderId");

            var customerId = visitorContext.GetCustomerId();
            var request = new GetVisitorOrderRequest(orderId, customerId, storefront.ShopName);
            var result = this.OrderServiceProvider.GetVisitorOrder(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetVisitorOrderResult, CommerceOrder>(result, result.Order as CommerceOrder);
        }

        /// <summary>
        /// Gets the available countries.
        /// </summary>
        /// <returns>The manager response where the available country dictionary is returned in the Result.</returns>
        public ManagerResponse<GetAvailableCountriesResult, Dictionary<string, string>> GetAvailableCountries()
        {
            var request = new GetAvailableCountriesRequest();
            var result = ((CommerceOrderServiceProvider)this.OrderServiceProvider).GetAvailableCountries(request);

            Helpers.LogSystemMessages(result.SystemMessages, result);
            return new ManagerResponse<GetAvailableCountriesResult, Dictionary<string, string>>(result, new Dictionary<string, string>(result.AvailableCountries));
        }
    }
}