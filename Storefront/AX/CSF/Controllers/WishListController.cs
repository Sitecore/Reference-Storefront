//-----------------------------------------------------------------------
// <copyright file="WishListController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the WishListController class.</summary>
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

namespace Sitecore.Reference.Storefront.Controllers
{
    using Sitecore;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.WishLists;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Sitecore.Reference.Storefront.ExtensionMethods;

    /// <summary>
    /// Used to handle all Wish List Actions
    /// </summary>
    public class WishListController : AXBaseController
    {
        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListController" /> class.
        /// </summary>
        /// <param name="inventoryManager">The inventory manager.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="wishListManager">The wish list manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public WishListController(
            [NotNull] InventoryManager inventoryManager,
            [NotNull] AccountManager accountManager,
            [NotNull] WishListManager wishListManager,
            [NotNull] ContactFactory contactFactory)
            : base(accountManager, contactFactory)
        {
            Assert.ArgumentNotNull(contactFactory, "contactFactory");
            Assert.ArgumentNotNull(inventoryManager, "inventoryManager");
            Assert.ArgumentNotNull(wishListManager, "wishListManager");

            this.InventoryManager = inventoryManager;
            this.WishListManager = wishListManager;
        }

        /// <summary>
        /// Gets or sets the wish list manager.
        /// </summary>
        /// <value>
        /// The wish list manager.
        /// </value>
        public WishListManager WishListManager { get; protected set; }

        /// <summary>
        /// Gets or sets the inventory manager.
        /// </summary>
        /// <value>
        /// The inventory manager.
        /// </value>
        public InventoryManager InventoryManager { get; protected set; }

        #endregion

        #region Controller actions

        /// <summary>
        ///  Main  controller action
        /// </summary>
        /// <returns>My wish lists view</returns>
        [HttpGet]
        public override ActionResult Index()
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// View a Wish List based on it's Id
        /// </summary>
        /// <param name="id">The Id of the Wish List</param>
        /// <returns>
        /// Returns the view with wish lists details
        /// </returns>
        [HttpGet]
        public ActionResult ViewWishList(string id)
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Show Active Wish Lists
        /// </summary>
        /// <param name="filter">if set to <c>true</c> [filter].</param>
        /// <returns>
        /// Returns active wish lists
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult ActiveWishLists(bool filter = false)
        {
            try
            {
                var wishLists = new List<WishListHeader>();
                var userResponse = this.AccountManager.GetUser(Context.User.Name);
                var result = new WishListsBaseJsonResult(userResponse.ServiceProviderResult);
                if (userResponse.ServiceProviderResult.Success && userResponse.Result != null)
                {
                    wishLists = filter ? this.WishListsHeaders(result).Take(5).ToList() : this.WishListsHeaders(result);
                }

                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("ActiveWishLists", this, e);
                return Json(new BaseJsonResult("ActiveWishLists", e), JsonRequestBehavior.AllowGet);
            }           
        }

        /// <summary>
        /// Gets the wish lists.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A wish list
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetWishList(WishListInputModel model)
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

                var userResponse = this.AccountManager.GetUser(Context.User.Name);
                var result = new WishListBaseJsonResult(userResponse.ServiceProviderResult);
                if (userResponse.ServiceProviderResult.Success && userResponse.Result != null)
                {
                    this.WishList(model.ExternalId, result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetWishList", this, e);
                return Json(new BaseJsonResult("GetWishList", e), JsonRequestBehavior.AllowGet);
            }            
        }

        /// <summary>
        /// Create Wish List
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Creates wish list
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult CreateWishList(CreateWishListInputModel model)
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

                var wishLists = new List<WishListHeader>();
                var response = this.WishListManager.CreateWishList(this.CurrentStorefront, this.CurrentVisitorContext, model.Name);
                var result = new WishListsBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    wishLists = this.WishListsHeaders(result);
                }

                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("CreateWishList", this, e);
                return Json(new BaseJsonResult("CreateWishList", e), JsonRequestBehavior.AllowGet);
            }            
        }

        /// <summary>
        /// Deletes the wish list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Deletes wish list
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteWishList(WishListInputModel model)
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

                var wishLists = new List<WishListHeader>();
                var response = this.WishListManager.DeleteWishList(this.CurrentStorefront, this.CurrentVisitorContext, model.ExternalId);
                var result = new WishListsBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {                    
                    wishLists = this.WishListsHeaders(result);                   
                }

                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("DeleteWishList", this, e);
                return Json(new BaseJsonResult("DeleteWishList", e), JsonRequestBehavior.AllowGet);
            }         
        }

        /// <summary>
        /// Add the wish lists to the cart.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>
        /// The Json result.
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddWishListsToCart(List<WishListInputModel> models)
        {
            try
            {
                Assert.ArgumentNotNull(models, "models");

                var validationResult = new BaseJsonResult();
                this.ValidateModel(validationResult);
                if (validationResult.HasErrors)
                {
                    return Json(validationResult, JsonRequestBehavior.AllowGet);
                }

                var wishLists = new List<WishListHeader>();

                //// TODO: ADD ALL THE ITEMS ON EACH WISH LIST TO THE CART

                var result = new WishListsBaseJsonResult();
                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddWishListsToCart", this, e);
                return Json(new BaseJsonResult("AddWishListsToCart", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the wish list.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// Updates wish list
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult UpdateWishList(UpdateWishListInputModel model)
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

                var wishLists = new List<WishListHeader>();
                var response = this.WishListManager.UpdateWishList(this.CurrentStorefront, this.CurrentVisitorContext, model);
                var result = new WishListsBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    wishLists = this.WishListsHeaders(result);                    
                }

                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("UpdateWishList", this, e);
                return Json(new BaseJsonResult("UpdateWishList", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Adds to wish list.
        /// </summary>
        /// <param name="model">The view model.</param>
        /// <returns>
        /// true if the product was added
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddToWishList(AddToWishListInputModel model)
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

                var wishLists = new List<WishListHeader>();
                var response = this.WishListManager.AddLinesToWishList(this.CurrentStorefront, this.CurrentVisitorContext, model);
                var result = new WishListsBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {            
                    wishLists = this.WishListsHeaders(result);
                }

                result.Initialize(wishLists);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddToWishList", this, e);
                return Json(new BaseJsonResult("AddToWishList", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the wish list line item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Returns json result with delete line item operation status</returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult DeleteLineItem(WishListLineInputModel model)
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

                var response = this.WishListManager.RemoveWishListLines(this.CurrentStorefront, this.CurrentVisitorContext, model.WishListId, new List<WishListLineInputModel> { model });
                var result = new WishListBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    this.WishList(model.WishListId, result);                   
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
        /// Updates the wish list line item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Returns the view with update wish list</returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public ActionResult UpdateLineItem(WishListLineInputModel model)
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

                var response = this.WishListManager.UpdateWishListLine(this.CurrentStorefront, this.CurrentVisitorContext, model);
                var result = new WishListBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    this.WishList(model.WishListId, result);       
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("UpdateLineItem", this, e);
                return Json(new BaseJsonResult("UpdateLineItem", e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Helpers

        private void WishList(string wishListId, WishListBaseJsonResult result)
        {
            var response = this.WishListManager.GetWishList(this.CurrentStorefront, this.CurrentVisitorContext, wishListId);
            var wishList = new WishList();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                wishList = response.Result;
            }

            result.Initialize(wishList);
            result.SetErrors(response.ServiceProviderResult);
        }

        private List<WishListHeader> WishListsHeaders(WishListsBaseJsonResult result)
        {
            var response = this.WishListManager.GetWishLists(this.CurrentStorefront, this.CurrentVisitorContext);
            var wishLists = new List<WishListHeader>();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                wishLists = response.Result.ToList();
            }

            result.SetErrors(response.ServiceProviderResult);
            return wishLists;
        }

        #endregion
    }
}