//-----------------------------------------------------------------------
// <copyright file="CheckoutController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
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
    using System.Linq;
    using System.Web.Mvc;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Payments;
    using Sitecore.Commerce.Entities.Shipping;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using Sitecore.Reference.Storefront.Models.RenderingModels;
    using CommerceParty = Sitecore.Commerce.Connect.DynamicsRetail.Entities.CommerceParty;
    using Sitecore.Reference.Storefront.ExtensionMethods;

    /// <summary>
    /// Handles all calls to checkout
    /// </summary>
    public class CheckoutController : AXBaseController
    {
        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutController" /> class.
        /// </summary>
        /// <param name="cartManager">The cart manager.</param>
        /// <param name="orderManager">The order manager.</param>
        /// <param name="loyaltyProgramManager">The loyalty program manager.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="paymentManager">The payment manager.</param>
        /// <param name="shippingManager">The shipping manager.</param>
        /// <param name="storeManager">The store manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public CheckoutController(
            [NotNull] CartManager cartManager,
            [NotNull] OrderManager orderManager,
            [NotNull] LoyaltyProgramManager loyaltyProgramManager,
            [NotNull] AccountManager accountManager,
            [NotNull] PaymentManager paymentManager,
            [NotNull] ShippingManager shippingManager,
            [NotNull] StoreManager storeManager,
            [NotNull] ContactFactory contactFactory)
            : base(accountManager, contactFactory)
        {
            Assert.ArgumentNotNull(cartManager, "cartManager");
            Assert.ArgumentNotNull(orderManager, "orderManager");
            Assert.ArgumentNotNull(loyaltyProgramManager, "loyaltyProgramManager");
            Assert.ArgumentNotNull(paymentManager, "paymentManager");
            Assert.ArgumentNotNull(shippingManager, "shippingManager");
            Assert.ArgumentNotNull(storeManager, "storeManager");

            this.CartManager = cartManager;
            this.OrderManager = orderManager;
            this.LoyaltyProgramManager = loyaltyProgramManager;
            this.PaymentManager = paymentManager;
            this.ShippingManager = shippingManager;
            this.StoreManager = storeManager;
        }

        /// <summary>
        /// Gets or sets the cart manager.
        /// </summary>
        /// <value>
        /// The cart manager.
        /// </value>
        public CartManager CartManager { get; protected set; }

        /// <summary>
        /// Gets or sets the payment manager.
        /// </summary>
        /// <value>
        /// The payment manager.
        /// </value>
        public PaymentManager PaymentManager { get; protected set; }

        /// <summary>
        /// Gets or sets the shipping manager.
        /// </summary>
        /// <value>
        /// The shipping manager.
        /// </value>
        public ShippingManager ShippingManager { get; protected set; }

        /// <summary>
        /// Gets or sets the order manager.
        /// </summary>
        /// <value>
        /// The order manager.
        /// </value>
        public OrderManager OrderManager { get; protected set; }

        /// <summary>
        /// Gets or sets the loyalty program manager.
        /// </summary>
        /// <value>
        /// The loyalty program manager.
        /// </value>
        public LoyaltyProgramManager LoyaltyProgramManager { get; protected set; }

        /// <summary>
        /// Gets or sets the store manager.
        /// </summary>
        /// <value>
        /// The store manager.
        /// </value>
        public StoreManager StoreManager { get; protected set; }

        #endregion

        #region Controller actions

        /// <summary>
        /// Handles the index view of the controller
        /// </summary>
        /// <returns>The action for this view</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult StartCheckout()
        {
            var viewRequested = this.Request.QueryString["view"];
            if (viewRequested != null && viewRequested == "DynamicsCheckout")
            {
                return View(this.GetRenderingView("DynamicsCheckout"));
            }

            var response = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, true);
            var cart = (CommerceCart)response.ServiceProviderResult.Cart;
            if (cart.Lines == null || !cart.Lines.Any())
            {
                var cartPageUrl = StorefrontManager.StorefrontUri("/shoppingcart");
                return Redirect(cartPageUrl);
            }

            return View(this.CurrentRenderingView, new CartRenderingModel(response.Result));
        }

        /// <summary>
        /// Gets Dynamicses checkout controller.
        /// </summary>
        /// <returns>Dynamics Checkout controller view</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult DynamicsCheckout()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Orders confirmation.
        /// </summary>
        /// <returns>Order Confirmation view</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult OrderConfirmation()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Retrieves data required to start the checkout process.
        /// </summary>
        /// <returns>Data required to start the checkout process.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetCheckoutData()
        {
            try
            {
                var result = new CheckoutDataBaseJsonResult();
                var response = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, true);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    var cart = (CommerceCart)response.ServiceProviderResult.Cart;
                    if (cart.Lines != null && cart.Lines.Any())
                    {
                        result.Cart = new AXCartBaseJsonResult(response.ServiceProviderResult);
                        result.Cart.Initialize(response.ServiceProviderResult.Cart);

                        result.ShippingMethods = new List<ShippingMethod>();
                        result.CartLoyaltyCardNumber = cart.LoyaltyCardID;
                        result.CurrencySymbol = Context.Language.CultureInfo.NumberFormat.CurrencySymbol;

                        this.AddShippingOptionsToResult(result, cart);
                        if (result.Success)
                        {
                            this.AddShippingMethodsToResult(result, cart);
                            if (result.Success)
                            {
                                this.GetAvailableCountries(result);
                                if (result.Success)
                                {
                                    this.GetPaymentOptions(result);
                                    if (result.Success)
                                    {
                                        this.GetPaymentMethods(result);
                                        if (result.Success)
                                        {
                                            this.GetUserInfo(result);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result.SetErrors(response.ServiceProviderResult);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetCheckoutData", this, e);
                return Json(new BaseJsonResult("GetCheckoutData", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Submits the order in json.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>The result in Json format.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult SubmitOrder(SubmitOrderInputModel inputModel)
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

                var response = this.OrderManager.SubmitVisitorOrder(CurrentStorefront, CurrentVisitorContext, inputModel);
                var result = new SubmitOrderBaseJsonResult(response.ServiceProviderResult);
                if (!response.ServiceProviderResult.Success || response.Result == null || response.ServiceProviderResult.CartWithErrors != null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Initialize(string.Concat(StorefrontManager.StorefrontUri("checkout/OrderConfirmation"), "?confirmationId=", (response.Result.TrackingNumber)));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("SubmitOrder", this, e);
                return Json(new BaseJsonResult("SubmitOrder", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the available shipping methods.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The available shipping methods.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetShippingMethods(GetShippingMethodsInputModel inputModel)
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

                var response = this.ShippingManager.GetShippingMethods(CurrentStorefront, CurrentVisitorContext, inputModel);
                var result = new ShippingMethodsJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    result.Initialize(response.ServiceProviderResult.ShippingMethods, response.ServiceProviderResult.ShippingMethodsPerItems);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetShippingMethods", this, e);
                return Json(new BaseJsonResult("GetShippingMethods", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Sets the shipping methods.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>The action for this view</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult SetShippingMethods(SetShippingMethodsInputModel inputModel)
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

                var response = this.CartManager.SetShippingMethods(CurrentStorefront, CurrentVisitorContext, inputModel);
                var result = new AXCartBaseJsonResult(response.ServiceProviderResult);
                if (!response.ServiceProviderResult.Success || response.Result == null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // QUERYING FOR THE CART AGAIN BECAUSE THE SHIPPING COST AND TAX TOTAL ARE NOT BEING UPDATED 
                // ON THE RETURNED CART OF THE SETSHIPPINGMETHOD REQUEST 
                var cartResponse = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, true);
                result.SetErrors(cartResponse.ServiceProviderResult);
                if (cartResponse.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(cartResponse.Result);
                }
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("SetShippingMethods", this, e);
                return Json(new BaseJsonResult("SetShippingMethods", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Sets the payment methods.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>The AX version returns the current cart.  No processing required.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult SetPaymentMethods(PaymentInputModel inputModel)
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

                var response = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, false);
                AXCartBaseJsonResult cartResult = new AXCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    cartResult.Initialize(response.ServiceProviderResult.Cart);
                }

                return Json(cartResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("SetPaymentMethods", this, e);
                return Json(new BaseJsonResult("SetPaymentMethods", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the nearby stores.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// A list of stores
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetNearbyStores(GetNearbyStoresInputModel inputModel)
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

                var response = this.StoreManager.GetNearbyStores(CurrentStorefront, CurrentVisitorContext, inputModel);
                var result = new NearbyStoresJsonResult(response.ServiceProviderResult);
                if (!response.ServiceProviderResult.Success || response.Result == null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Initialize(response.Result.ToList());
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetNearbyStores", this, e);
                return Json(new BaseJsonResult("GetNearbyStores", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the available states.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A list of states based on the country
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetAvailableStates(GetAvailableStatesInputModel model)
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

                var response = this.OrderManager.GetAvailableStates(CurrentStorefront, CurrentVisitorContext, model.CountryCode);
                var result = new AvailableStatesBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetAvailableStates", this, e);
                return Json(new BaseJsonResult("GetAvailableStates", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the loytalty card.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A response indicating whether the loyalty card was successfully updated or not.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UpdateLoyaltyCard(LoyaltyCardInputModelItem model)
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

                var response = this.LoyaltyProgramManager.AssociateLoyaltyCardWithCurrentCart(CurrentStorefront, CurrentVisitorContext, model.LoyaltyCardNumber);
                var result = new UpdateLoyaltyCardBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("UpdateLoyaltyCard", this, e);
                return Json(new BaseJsonResult("UpdateLoyaltyCard", e), JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion

        #region Helpers

        private void GetUserInfo(CheckoutDataBaseJsonResult result)
        {
            var isUserAuthenticated = Context.User.IsAuthenticated;
            result.IsUserAuthenticated = isUserAuthenticated;
            result.UserEmail = isUserAuthenticated && !Context.User.Profile.IsAdministrator ? this.AccountManager.ResolveCommerceUser().Result.Email : string.Empty;
            if (!isUserAuthenticated)
            {
                return;
            }

            var addresses = new List<CommerceParty>();
            var response = this.AccountManager.GetCurrentUserParties(this.CurrentStorefront, this.CurrentVisitorContext);
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                addresses = response.Result.ToList();
            }

            var addressesResult = new AddressListItemJsonResult();
            addressesResult.Initialize(addresses, null);
            result.UserAddresses = addressesResult;
            result.SetErrors(response.ServiceProviderResult);
        }

        private void AddShippingOptionsToResult(CheckoutDataBaseJsonResult result, CommerceCart cart)
        {
            var response = this.ShippingManager.GetShippingPreferences(cart);
            var orderShippingOptions = new List<ShippingOption>();
            var lineShippingOptions = new List<LineShippingOption>();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                orderShippingOptions = response.ServiceProviderResult.ShippingOptions.ToList();
                lineShippingOptions = response.ServiceProviderResult.LineShippingPreferences.ToList();
            }

            result.OrderShippingOptions = orderShippingOptions;
            result.LineShippingOptions = lineShippingOptions;
            if (result.LineShippingOptions != null && result.LineShippingOptions.Any())
            {
                foreach (var line in result.Cart.Lines)
                {
                    var lineShippingOption = result.LineShippingOptions.FirstOrDefault(l => l.LineId.Equals(line.ExternalCartLineId, StringComparison.OrdinalIgnoreCase));
                    if (lineShippingOption != null)
                    {
                        line.ShippingOptions = lineShippingOption.ShippingOptions;
                    }
                }
            }

            result.SetErrors(response.ServiceProviderResult);
        }

        private void AddShippingMethodsToResult(CheckoutDataBaseJsonResult result, CommerceCart cart)
        {
            var shippingRequest = new GetShippingMethodsInputModel { ShippingPreferenceType = ShippingOptionType.None.Name };
            var response = this.ShippingManager.GetShippingMethods(this.CurrentStorefront, this.CurrentVisitorContext, shippingRequest);

            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                foreach (var sm in response.Result)
                {
                    var isEmailMethod = sm.GetPropertyValue("IsEmailShippingMethod") != null && (bool)sm.GetPropertyValue("IsEmailShippingMethod");
                    var isShipToStoreMethod = sm.GetPropertyValue("IsShipToStoreShippingMethod") != null && (bool)sm.GetPropertyValue("IsShipToStoreShippingMethod");

                    if (isEmailMethod)
                    {
                        result.EmailDeliveryMethod = sm;
                    }

                    if (isShipToStoreMethod)
                    {
                        result.ShipToStoreDeliveryMethod = sm;
                    }
                }

                return;
            }

            result.EmailDeliveryMethod = new ShippingMethod();
            result.ShipToStoreDeliveryMethod = new ShippingMethod();
            result.SetErrors(response.ServiceProviderResult);
        }

        private void GetAvailableCountries(CheckoutDataBaseJsonResult result)
        {
            var response = this.OrderManager.GetAvailableCountries();
            var countries = new Dictionary<string, string>();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                countries = response.Result;
            }

            result.Countries = countries;
            result.SetErrors(response.ServiceProviderResult);
        }

        private void GetPaymentOptions(CheckoutDataBaseJsonResult result)
        {
            var response = this.PaymentManager.GetPaymentOptions(this.CurrentStorefront, this.CurrentVisitorContext);
            var paymentOptions = new List<PaymentOption>();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                paymentOptions = response.Result.ToList();
            }

            result.PaymentOptions = paymentOptions;
            result.SetErrors(response.ServiceProviderResult);
        }

        private void GetPaymentMethods(CheckoutDataBaseJsonResult result)
        {
            var response = this.PaymentManager.GetPaymentMethods(this.CurrentStorefront, this.CurrentVisitorContext, new PaymentOption());
            var paymentMethods = new List<PaymentMethod>();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                paymentMethods = response.Result.ToList();
            }

            result.PaymentMethods = paymentMethods;
            result.SetErrors(response.ServiceProviderResult);
        }

        #endregion
    }
}
