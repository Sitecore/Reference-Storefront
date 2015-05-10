//-----------------------------------------------------------------------
// <copyright file="CartController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CartController class.</summary>
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
    using System.Web.Mvc;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using Sitecore.Reference.Storefront.ExtensionMethods;
    using System.Web.UI;

    /// <summary>
    /// Defines the shopping cart controller type.
    /// </summary>
    public class CartController : CSBaseController
    {
        #region Properties

        /// <summary>
        ///     Initializes a new instance of the <see cref="CartController" /> class.
        /// </summary>
        /// <param name="cartManager">The cart manager.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public CartController([NotNull] CartManager cartManager, [NotNull] AccountManager accountManager, [NotNull] ContactFactory contactFactory)
            : base(accountManager, contactFactory)
        {
            Assert.ArgumentNotNull(cartManager, "cartManager");

            CartManager = cartManager;
        }

        /// <summary>
        ///     Gets or sets the cart manager.
        /// </summary>
        /// <value>
        ///     The cart manager.
        /// </value>
        public CartManager CartManager { get; protected set; }

        #endregion

        #region Controller actions

        /// <summary>
        ///     main cart controller action
        /// </summary>
        /// <returns>the cart view</returns>
        [HttpGet]
        public override ActionResult Index()
        {
            return View(this.GetRenderingView("ShoppingCart"));
        }

        /// <summary>
        /// The action for rendering the basket view
        /// </summary>
        /// <param name="updateCart">if set to <c>true</c> [update cart].</param>
        /// <returns>
        /// The MiniCart view.
        /// </returns>
        public ActionResult MiniCart(bool updateCart = false)
        {
            return PartialView(this.GetRenderingView("MiniCart"));
        }

        /// <summary>
        /// Get the CheckoutButtons view.
        /// </summary>
        /// <returns>CheckoutButtons view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CheckoutButtons()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Baskets the update.
        /// </summary>
        /// <param name="updateCart">if set to <c>true</c> [update cart].</param>
        /// <returns>
        /// Returns the Json cart result.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult UpdateMiniCart(bool updateCart = false)
        {
            try
            {
                var response = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, updateCart);
                var result = new MiniCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.ServiceProviderResult.Cart);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("BasketUpdate", this, e);
                return Json(new BaseJsonResult("BasketUpdate", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the current cart.
        /// </summary>
        /// <returns>
        /// Returns the Json cart result.
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult GetCurrentCart()
        {
            try
            {
                var response = this.CartManager.GetCurrentCart(CurrentStorefront, CurrentVisitorContext, false);
                CSCartBaseJsonResult cartResult = new CSCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    cartResult.Initialize(response.ServiceProviderResult.Cart);
                }

                return Json(cartResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetCurrentCart", this, e);
                return Json(new BaseJsonResult("GetCurrentCart", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Adds a product to the cart
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// true if the product was added
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult AddCartLine(AddCartLineInputModel inputModel)
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

                var response = this.CartManager.AddLineItemsToCart(CurrentStorefront, CurrentVisitorContext, new List<AddCartLineInputModel> { inputModel });
                var result = new BaseJsonResult(response.ServiceProviderResult);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddCartLine", this);
                return Json(new BaseJsonResult("AddCartLine", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Adds the items to cart.
        /// </summary>
        /// <param name="inputModels">The input model.</param>
        /// <returns>
        /// Returns json result with add items to cart operation status
        /// </returns>
        [Authorize]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult AddCartLines(IEnumerable<AddCartLineInputModel> inputModels)
        {
            try
            {
                Assert.ArgumentNotNull(inputModels, "inputModels");

                var validationResult = new BaseJsonResult();
                this.ValidateModel(validationResult);
                if (validationResult.HasErrors)
                {
                    return Json(validationResult, JsonRequestBehavior.AllowGet);
                }

                var response = this.CartManager.AddLineItemsToCart(CurrentStorefront, CurrentVisitorContext, inputModels);
                var result = new BaseJsonResult(response.ServiceProviderResult);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddCartLines", this);
                return Json(new BaseJsonResult("AddCartLines", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes a line item from a cart
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// the partial view of the updated cart
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult DeleteLineItem(DeleteCartLineInputModel model)
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

                var response = this.CartManager.RemoveLineItemFromCart(CurrentStorefront, CurrentVisitorContext, model.ExternalCartLineId);
                var result = new CSCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("DeleteLineItem", this, e);
                return Json(new BaseJsonResult("DeleteLineItem", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update a cart line item
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The partial view of the updated cart
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult UpdateLineItem(UpdateCartLineInputModel inputModel)
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

                var response = this.CartManager.ChangeLineQuantity(CurrentStorefront, CurrentVisitorContext, inputModel);
                var result = new CSCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("UpdateLineItem", this, e);
                return Json(new BaseJsonResult("UpdateLineItem", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Applies the discount.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The partial view of the updated cart
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult ApplyDiscount(DiscountInputModel model)
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

                var response = this.CartManager.AddPromoCodeToCart(CurrentStorefront, CurrentVisitorContext, model.PromoCode);
                var result = new CSCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("ApplyDiscount", this, e);
                return Json(new BaseJsonResult("ApplyDiscount", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Removes a discount.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The partial view of the updated cart
        /// </returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult RemoveDiscount(DiscountInputModel model)
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

                var response = this.CartManager.RemovePromoCodeFromCart(CurrentStorefront, CurrentVisitorContext, model.PromoCode);
                var result = new CSCartBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("RemoveDiscount", this, e);
                return Json(new BaseJsonResult("RemoveDiscount", e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}