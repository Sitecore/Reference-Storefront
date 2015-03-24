//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AccountController class.</summary>
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
    using System.Linq;
    using System.Web.Mvc;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Configuration;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Customers;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Links;
    using CSFConnectModels = Sitecore.Reference.Storefront.Connect.Models;
    using Sitecore.Reference.Storefront.ExtensionMethods;

    /// <summary>
    /// Used to handle all account actions
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class AccountController : CSBaseController
    {
        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="orderManager">The order manager.</param>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public AccountController(
            [NotNull] OrderManager orderManager,
            [NotNull] AccountManager accountManager,
            [NotNull] ContactFactory contactFactory)
            : base(accountManager, contactFactory)
        {
            Assert.ArgumentNotNull(orderManager, "orderManager");

            this.OrderManager = orderManager;
        }

        /// <summary>
        /// Gives the various types of messages
        /// </summary>
        public enum ManageMessageId
        {
            /// <summary>
            /// Indicates a successful password change
            /// </summary>
            ChangePasswordSuccess,

            /// <summary>
            /// Indicates a successful password set
            /// </summary>
            SetPasswordSuccess,

            /// <summary>
            /// Indicates a successful account delete
            /// </summary>
            RemoveLoginSuccess,
        }

        /// <summary>
        /// Gets or sets the order manager.
        /// </summary>
        /// <value>
        /// The order manager.
        /// </value>
        public OrderManager OrderManager { get; protected set; }

        #endregion

        #region Controller actions

        /// <summary>
        /// The default action for the main page for the account section
        /// </summary>
        /// <returns>The view for the section</returns>
        [HttpGet]       
        public override ActionResult Index()
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            return View(this.GetRenderingView("Index"));
        }

        /// <summary>
        /// An action to handle displaying the login form
        /// </summary>
        /// <param name="returnUrl">A location to redirect the user to</param>
        /// <returns>The view to display to the user</returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "url not required in webpage")]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Handles a user trying to log off
        /// </summary>
        /// <returns>The view to display to the user after logging off</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            this.AccountManager.Logout();

            return RedirectToLocal(StorefrontManager.StorefrontHome);
        }

        /// <summary>
        /// Handles displaying a form for the user to login
        /// </summary>
        /// <returns>The view to display to the user</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Addressees this instance.
        /// </summary>
        /// <returns>The view to display address book</returns>
        [HttpGet]
        [Authorize]
        public ActionResult Addresses()
        {
            return View(this.GetRenderingView("Addresses"));
        }

        /// <summary>
        /// Displays the Profile Edit Page.
        /// </summary>
        /// <returns>
        /// Profile Edit Page
        /// </returns>
        [HttpGet]
        [Authorize]
        public ActionResult EditProfile()
        {
            var model = new ProfileModel();

            if (!Context.User.IsAuthenticated || Context.User.Profile.IsAdministrator)
            {
                return View(this.GetRenderingView("EditProfile"), model);
            }

            var commerceUser = this.AccountManager.GetUser(this.CurrentVisitorContext.UserName).Result;

            if (commerceUser == null)
            {
                return View(this.GetRenderingView("EditProfile"), model);
            }

            model.FirstName = commerceUser.FirstName;
            model.Email = commerceUser.Email;
            model.EmailRepeat = commerceUser.Email;
            model.LastName = commerceUser.LastName;
            model.TelephoneNumber = commerceUser.GetPropertyValue("Phone") as string;

            return View(this.GetRenderingView("EditProfile"), model);
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <returns>The view to display.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(this.CurrentRenderingView);
        }
        
        /// <summary>
        /// Changes the password confirmation.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>The forgot password confirmation view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(string userName)
        {
            ViewBag.UserName = userName;

            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <returns>Chagne password view</returns>
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View(this.CurrentRenderingView);
        }
        
        /// <summary>
        /// Handles a user trying to register
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The view to display to the user after they register
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Register(RegisterUserInputModel inputModel)
        {
            try
            {
                Assert.ArgumentNotNull(inputModel, "RegisterInputModel");
                RegisterBaseJsonResult result = new RegisterBaseJsonResult();

                this.ValidateModel(result);
                if (result.HasErrors)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var response = this.AccountManager.RegisterUser(this.CurrentStorefront, inputModel);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                    this.AccountManager.Login(CurrentStorefront, CurrentVisitorContext, response.Result.UserName, inputModel.Password, false);
                }
                else
                {
                    result.SetErrors(response.ServiceProviderResult);
                }

                return Json(result);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("Register", this, e);
                return Json(new BaseJsonResult("Register", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Handles a user trying to login
        /// </summary>
        /// <param name="model">The user's login details</param>
        /// <returns>The view to display to the user</returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "No required for this scenario")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && this.AccountManager.Login(CurrentStorefront, CurrentVisitorContext, UpdateUserName(model.UserName), model.Password, model.RememberMe))
            {
                return RedirectToLocal(StorefrontManager.StorefrontHome);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return View(this.GetRenderingView("Login"));
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The result in Json format.
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePassword(ChangePasswordInputModel inputModel)
        {
            try
            {
                Assert.ArgumentNotNull(inputModel, "ChangePasswordInputModel");
                ChangePasswordBaseJsonResult result = new ChangePasswordBaseJsonResult();

                this.ValidateModel(result);
                if (result.HasErrors)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var response = this.AccountManager.UpdateUserPassword(this.CurrentStorefront, this.CurrentVisitorContext, inputModel);
                result = new ChangePasswordBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    result.Initialize(this.CurrentVisitorContext.UserName);
                }

                return Json(result);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("ChangePassword", this, e);
                return Json(new BaseJsonResult("ChangePassword", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Orderses the history.
        /// </summary>
        /// <returns>
        /// The view to display all orders for current user
        /// </returns>
        [HttpGet]
        [Authorize]
        public ActionResult MyOrders()
        {
            var commerceUser = this.AccountManager.GetUser(Context.User.Name).Result;
            var orders = this.OrderManager.GetOrders(commerceUser.ExternalId, Context.Site.Name).Result;
            return View(this.CurrentRenderingView, orders.ToList());
        }

        /// <summary>
        /// Orders the detail.
        /// </summary>
        /// <param name="id">The order confirmation Id.</param>
        /// <returns>
        /// The view to display order details
        /// </returns>
        [HttpGet]
        [Authorize]
        public ActionResult MyOrder(string id)
        {
            var response = this.OrderManager.GetOrderDetails(CurrentStorefront, CurrentVisitorContext, id);
            ViewBag.IsItemShipping = response.Result.Shipping != null && response.Result.Shipping.Count > 1 && response.Result.Lines.Count > 1;
            return View(this.CurrentRenderingView, response.Result);
        }

        /// <summary>
        /// Recent Orders PlugIn for Account Management Home Page
        /// </summary>
        /// <returns>The view to display recent orders</returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult RecentOrders()
        {
            try
            {
                var recentOrders = new List<OrderHeader>();

                var userResponse = this.AccountManager.GetUser(Context.User.Name);
                var result = new OrdersBaseJsonResult(userResponse.ServiceProviderResult);
                if (userResponse.ServiceProviderResult.Success && userResponse.Result != null)
                {
                    var commerceUser = userResponse.Result;
                    var response = this.OrderManager.GetOrders(commerceUser.ExternalId, Context.Site.Name);
                    result.SetErrors(response.ServiceProviderResult);
                    if (response.ServiceProviderResult.Success && response.Result != null)
                    {
                        var orders = response.Result.ToList();
                        recentOrders = orders.Where(order => (order as CommerceOrderHeader).LastModified > DateTime.Today.AddDays(-30)).Take(5).ToList();
                    }
                }

                result.Initialize(recentOrders);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("RecentOrders", this, e);
                return Json(new BaseJsonResult("RecentOrders", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Profile PlugIn for Account Management Home Page
        /// </summary>
        /// <returns>The view to display profile page</returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AccountHomeProfile()
        {
            var model = new ProfileModel();

            if (Context.User.IsAuthenticated && !Context.User.Profile.IsAdministrator)
            {
                var commerceUser = this.AccountManager.GetUser(this.CurrentVisitorContext.UserName).Result;
                if (commerceUser != null)
                {
                    model.FirstName = commerceUser.FirstName;
                    model.Email = commerceUser.Email;
                    model.LastName = commerceUser.LastName;
                    model.TelephoneNumber = commerceUser.GetPropertyValue("Phone") as string;
                }
            }

            Item item = Context.Item.Children.SingleOrDefault(p => p.Name == "EditProfile");

            if (item != null)
            {
                //If there is a specially EditProfile then use it
                ViewBag.EditProfileLink = LinkManager.GetDynamicUrl(item);
            }
            else
            {
                //Else go global Edit Profile
                item = Context.Item.Database.GetItem("/sitecore/content/Home/MyAccount/Profile");
                ViewBag.EditProfileLink = LinkManager.GetDynamicUrl(item);
            }

            return View(CurrentRenderingView, model);
        }

        /// <summary>
        /// Address Book in the Home Page
        /// </summary>
        /// <returns>The list of addresses</returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddressList()
        {
            try
            {
                var result = new AddressListItemJsonResult();
                var addresses = this.AllAddresses(result);
                var countries = this.GetAvailableCountries(result);
                result.Initialize(addresses, countries);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddressList", this, e);
                return Json(new BaseJsonResult("AddressList", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Handles deleting of an address and removing it from a user's profile
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The JsonResult with deleting operation status
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddressDelete(DeletePartyInputModelItem model)
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

                var addresses = new List<CSFConnectModels.CommerceParty>();
                var response = this.AccountManager.RemovePartiesFromCurrentUser(this.CurrentStorefront, this.CurrentVisitorContext, model.ExternalId);
                var result = new AddressListItemJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    addresses = this.AllAddresses(result);
                }

                result.Initialize(addresses, null);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddressDelete", this, e);
                return Json(new BaseJsonResult("AddressDelete", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Handles updates to an address
        /// </summary>
        /// <param name="model">Any changes to the address</param>
        /// <returns>
        /// The view to display the updated address
        /// </returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AddressModify(PartyInputModelItem model)
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

                var addresses = new List<CSFConnectModels.CommerceParty>();
                var userResponse = this.AccountManager.GetUser(Context.User.Name);
                var result = new AddressListItemJsonResult(userResponse.ServiceProviderResult);
                if (userResponse.ServiceProviderResult.Success && userResponse.Result != null)
                {
                    var commerceUser = userResponse.Result;
                    var customer = new CommerceCustomer { ExternalId = commerceUser.ExternalId };
                    var party = new CSFConnectModels.CommerceParty
                            {
                                ExternalId = model.ExternalId,
                                Name = model.Name,
                                Address1 = model.Address1,
                                City = model.City,
                                Country = model.Country,
                                State = model.State,
                                ZipPostalCode = model.ZipPostalCode,
                                PartyId = model.PartyId,
                                IsPrimary = model.IsPrimary
                            };

                    if (string.IsNullOrEmpty(party.ExternalId))
                    {
                        party.ExternalId = Guid.NewGuid().ToString("B");

                        var response = this.AccountManager.AddParties(this.CurrentStorefront, customer, new List<Sitecore.Commerce.Entities.Party> { party });
                        result.SetErrors(response.ServiceProviderResult);
                        if (response.ServiceProviderResult.Success)
                        {
                            addresses = this.AllAddresses(result);
                        }

                        result.Initialize(addresses, null);
                    }
                    else
                    {
                        var response = this.AccountManager.UpdateParties(this.CurrentStorefront, customer, new List<Sitecore.Commerce.Entities.Party> { party });
                        result.SetErrors(response.ServiceProviderResult);
                        if (response.ServiceProviderResult.Success)
                        {
                            addresses = this.AllAddresses(result);
                        }

                        result.Initialize(addresses, null);
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("AddressModify", this, e);
                return Json(new BaseJsonResult("AddressModify", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the profile.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The result in Json format.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateProfile(ProfileModel model)
        {
            try
            {
                Assert.ArgumentNotNull(model, "UpdateProfileInputModel");
                ProfileBaseJsonResult result = new ProfileBaseJsonResult();

                this.ValidateModel(result);
                if (result.HasErrors)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (!Context.User.IsAuthenticated || Context.User.Profile.IsAdministrator)
                {
                    return Json(result);
                }

                var response = this.AccountManager.UpdateUser(this.CurrentStorefront, this.CurrentVisitorContext, model);
                result.SetErrors(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && !string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.PasswordRepeat))
                {
                    var changePasswordModel = new ChangePasswordInputModel { NewPassword = model.Password, ConfirmPassword = model.PasswordRepeat };
                    var passwordChangeResponse = this.AccountManager.UpdateUserPassword(this.CurrentStorefront, this.CurrentVisitorContext, changePasswordModel);
                    result.SetErrors(passwordChangeResponse.ServiceProviderResult);
                    if (passwordChangeResponse.ServiceProviderResult.Success)
                    {
                        result.Initialize(response.ServiceProviderResult);
                    }
                }

                return Json(result);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("UpdateProfile", this, e);
                return Json(new BaseJsonResult("UpdateProfile", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>The current authenticated user info</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public JsonResult GetCurrentUser()
        {
            try
            {
                if (!Context.User.IsAuthenticated || Context.User.Profile.IsAdministrator)
                {
                    var anonymousResult = new UserBaseJsonResult();
                    anonymousResult.Initialize(new CommerceUser());
                    return Json(anonymousResult, JsonRequestBehavior.AllowGet);
                }

                var response = this.AccountManager.GetUser(this.CurrentVisitorContext.UserName);
                var result = new UserBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("GetCurrentUser", this, e);
                return Json(new BaseJsonResult("GetCurrentUser", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The result in json format</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult ForgotPassword(ForgotPasswordInputModel model)
        {
            try
            {
                Assert.ArgumentNotNull(model, "model");
                ForgotPasswordBaseJsonResult result = new ForgotPasswordBaseJsonResult();

                this.ValidateModel(result);
                if (result.HasErrors)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var resetResponse = this.AccountManager.ResetUserPassword(this.CurrentStorefront, model);
                if (!resetResponse.ServiceProviderResult.Success)
                {
                    return Json(new ForgotPasswordBaseJsonResult(resetResponse.ServiceProviderResult));
                }

                result.Initialize(model.Email);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("ForgotPassword", this, e);
                return Json(new BaseJsonResult("ForgotPassword", e), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Concats the user name with the current domain if it missing
        /// </summary>
        /// <param name="userName">The user's user name</param>
        /// <returns>The updated user name</returns>
        public virtual string UpdateUserName(string userName)
        {
            var defaultDomain = CommerceServerSitecoreConfig.Current.DefaultCommerceUsersDomain;
            if (string.IsNullOrWhiteSpace(defaultDomain))
            {
                defaultDomain = CommerceConstants.ProfilesStrings.CommerceUsersDomainName;
            }

            return !userName.StartsWith(defaultDomain, StringComparison.OrdinalIgnoreCase) ? string.Concat(defaultDomain, @"\", userName) : userName;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }

        private Dictionary<string, string> GetAvailableCountries(AddressListItemJsonResult result)
        {
            var countries = new Dictionary<string, string>();
            var response = OrderManager.GetAvailableCountries();
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                countries = response.Result;
            }

            result.SetErrors(response.ServiceProviderResult);
            return countries;
        }

        private List<CSFConnectModels.CommerceParty> AllAddresses(AddressListItemJsonResult result)
        {
            var addresses = new List<CSFConnectModels.CommerceParty>();
            var response = this.AccountManager.GetCurrentCustomerParties(this.CurrentStorefront, this.CurrentVisitorContext);
            if (response.ServiceProviderResult.Success && response.Result != null)
            {
                addresses = response.Result.ToList();
            }

            result.SetErrors(response.ServiceProviderResult);
            return addresses;
        }

        #endregion
    }
}
