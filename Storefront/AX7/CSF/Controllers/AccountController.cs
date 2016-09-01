//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the AccountController class.</summary>
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
    using Microsoft.IdentityModel.Protocols;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Configuration;
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Customers;
    using Sitecore.Commerce.Entities.Orders;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Exceptions;
    using Sitecore.Links;
    using Sitecore.Reference.Storefront.Configuration;
    using Sitecore.Reference.Storefront.ExtensionMethods;
    using Sitecore.Reference.Storefront.Infrastructure;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Reference.Storefront.Models.InputModels;
    using Sitecore.Reference.Storefront.Models.JsonResults;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;
    
    /// <summary>
    /// Used to handle all account actions
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class AccountController : AXBaseController
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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
        /// <param name="existingAccount">The existing account.</param>
        /// <param name="externalIdProvider">The external identifier provider.</param>
        /// <returns>
        /// The view to display to the user
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "url not required in webpage")]
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Login(string returnUrl, string existingAccount, string externalIdProvider)
        {
            ViewBag.ReturnUrl = returnUrl;
           
            var model = new LoginModel();
            model.IsActivationFlow = !string.IsNullOrEmpty(existingAccount);

            if (model.IsActivationFlow)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture, 
                    StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.LinkupSucceeded), 
                    Context.Site.Name,
                    externalIdProvider,
                    existingAccount);
                model.Message = message;
            }

            List<IdentityProviderModel> providers = new List<IdentityProviderModel>();
            IDictionary<string, IdentityProviderClientConfigurationElement> identityProviderDictionary = GetIdentityProvidersFromConfig();
            foreach (IdentityProviderClientConfigurationElement provider in identityProviderDictionary.Values)
            {
                MediaItem providerImage = Sitecore.Context.Database.GetItem(provider.ImageUrl.OriginalString);                
                providers.Add(new IdentityProviderModel() { Name = provider.Name, Image = providerImage });
            }
            
            model.Providers.AddRange(providers);           
            return View(this.CurrentRenderingView, model);
        }

        /// <summary>
        /// Action invoked on being redirected from open identity provider.
        /// </summary>
        /// <returns>View after being redirected from open identity provider.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when email claim does not exist.</exception>
        public ActionResult OAuthV2Redirect()
        {
            IdentityProviderClientConfigurationElement currentProvider = OpenIdConnectUtilities.GetCurrentProviderSettings();

            // Check whether provider returned an error which could be a case if a user rejected a consent.           
            string errorCode = this.HttpContext.Request.Params["error"];            
            if (errorCode != null)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.LoginFailed), 
                    currentProvider.Name, 
                    errorCode);
                CommerceLog.Current.Warning(message, this);
                this.Response.Redirect("~", false);
                this.HttpContext.ApplicationInstance.CompleteRequest();
                return null;
            }

            string authorizationCode = OpenIdConnectUtilities.ValidateRequestAndGetAuthorizationCode();

            if (authorizationCode == null)
            {
                string message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.AuthorizationCodeMissing);
                SecurityException securityException = new SecurityException(message);
                CommerceLog.Current.Error(message, this, securityException);
                throw securityException;
            }

            string bodyParameters = string.Format(
                CultureInfo.InvariantCulture,
                "grant_type=authorization_code&code={0}&redirect_uri={1}&client_id={2}&client_secret={3}",
                authorizationCode,
                currentProvider.RedirectUrl,
                currentProvider.ClientId,
                currentProvider.ClientSecret);

            OpenIdConnectConfiguration providerDiscoveryDocument = OpenIdConnectUtilities.GetDiscoveryDocument(currentProvider.Issuer);

            string returnValuesJson = OpenIdConnectUtilities.HttpPost(new Uri(providerDiscoveryDocument.TokenEndpoint), bodyParameters);

            TokenEndpointResponse tokenResponse = OpenIdConnectUtilities.DeserilizeJson<TokenEndpointResponse>(returnValuesJson);

            JwtSecurityToken token = OpenIdConnectUtilities.GetIdToken(tokenResponse.IdToken);

            Claim emailClaim = token.Claims.SingleOrDefault(c => string.Equals(c.Type, OpenIdConnectUtilities.Email, StringComparison.OrdinalIgnoreCase));

            string email = null;

            // IdentityServer does not return email claim.
            if (emailClaim != null)
            {
                email = emailClaim.Value;
            }

            return this.GetRedirectionBasedOnAssociatedCustomer(tokenResponse.IdToken, currentProvider.ProviderType, email);            
        }

        /// <summary>
        /// Action invoked on being redirected from ACS identity provider.
        /// </summary>
        /// <returns>View after being redirected from ACS identity provider.</returns>
        /// <exception cref="System.NotSupportedException">Thrown when email claim does not exist.</exception>        
        public ActionResult AcsRedirect()
        {
            string documentContents;
            using (Stream receiveStream = this.HttpContext.Request.InputStream)
            {
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                documentContents = readStream.ReadToEnd();
            }

            string acsToken = OpenIdConnectUtilities.GetAcsToken(documentContents);

            JwtSecurityToken token = new JwtSecurityToken(acsToken);
            var emailClaim = token.Claims.FirstOrDefault(t => t.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            string email = null;

            // Not all providers provide the claim, for instance, Windows Live ID does not.
            if (emailClaim != null)
            {
                email = emailClaim.Value;
            }

            return this.GetRedirectionBasedOnAssociatedCustomer(acsToken, IdentityProviderType.ACS, email);
        }

        /// <summary>
        /// Handles a user trying to log off
        /// </summary>
        /// <returns>The view to display to the user after logging off</returns>
        [HttpGet]
        [Authorize]
        public ActionResult LogOff()
        {
            if (Context.User.IsAuthenticated)
            {
                var ctx = Request.GetOwinContext();
                ctx.Authentication.SignOut(OpenIdConnectUtilities.ApplicationCookieAuthenticationType);                      

                // Clean up openId nonce cookie. This is just a workaround. Ideally, we should be calling 'ctx.Authentication.SignOut(providerClient.Name)'              
                foreach (string cookieName in ControllerContext.HttpContext.Request.Cookies.AllKeys)
                {
                    if (cookieName.StartsWith("OpenIdConnect.nonce.", StringComparison.OrdinalIgnoreCase))
                    {
                        OpenIdConnectUtilities.RemoveCookie(cookieName);
                        break;
                    }
                }               
            }

            this.AccountManager.Logout();
            return Redirect("/federatedSignout");
        }

        /// <summary>
        /// Federateds the signout.
        /// </summary>
        /// <returns>Current rendering view</returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult FederatedSignOut()
        {
            IdentityProviderClientConfigurationElement providerClient = OpenIdConnectUtilities.GetCurrentProviderSettings();
            Uri externalLogOffUri = providerClient.LogOffUrl;

            OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieCurrentProvider);
            OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieCurrentProviderType);
            OpenIdConnectUtilities.CleanUpOnSignOutOrAuthFailure(this.HttpContext);

            var model = new FederatedSignOutModel() { LogOffUri = externalLogOffUri };          
            return View(this.CurrentRenderingView, model);
        }

        /// <summary>
        /// Registers the specified user.
        /// </summary>
        /// <param name="commerceUser">The commerce user.</param>
        /// <returns>
        /// Redirects to the Home page after registration
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RegisterExistingUser(CommerceCustomer commerceUser)
        {
            try
            {
                Assert.ArgumentNotNull(commerceUser, "commerceUser");               
                RegisterBaseJsonResult result = new RegisterBaseJsonResult();

                var userResponse = this.AccountManager.GetUser(commerceUser.Name);
                if (userResponse.Result == null)
                {
                    // create the user in Sitecore
                    var inputModel = new RegisterUserInputModel { UserName = commerceUser.Name, Password = System.Web.Security.Membership.GeneratePassword(8, 4) };
                    inputModel.FirstName = commerceUser.Properties["FirstName"] as string ?? string.Empty;
                    inputModel.LastName = commerceUser.Properties["LastName"] as string ?? string.Empty;
                    var response = this.AccountManager.RegisterUser(this.CurrentStorefront, inputModel);
                    if (!response.ServiceProviderResult.Success || response.Result == null)
                    {                   
                        result.SetErrors(response.ServiceProviderResult);
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                var isLoggedIn = this.AccountManager.Login(CurrentStorefront, CurrentVisitorContext, commerceUser.Name, false);
                if (isLoggedIn)
                {
                    return RedirectToLocal(StorefrontManager.StorefrontHome);
                }
                else
                {
                    result.SetErrors(new List<string> { StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CouldNotCreateUser) });
                }

                return Json(result);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Redirect("/login");
            }
            catch (Exception e)
            {
                CommerceLog.Current.Error("Register", this, e);
                return Json(new BaseJsonResult("Register", e), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Addressees this instance.
        /// </summary>
        /// <returns>The view to display address book</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Addresses()
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            return View(this.GetRenderingView(StorefrontConstants.Views.Addresses));
        }

        /// <summary>
        /// Displays the Profile Edit Page.
        /// </summary>
        /// <returns>
        /// Profile Edit Page
        /// </returns>
        [HttpGet]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult EditProfile()
        {
            var model = new ProfileModel();

            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            var commerceUser = this.AccountManager.GetUser(Context.User.Name).Result;

            if (commerceUser == null)
            {
                return View(this.GetRenderingView(StorefrontConstants.Views.EditProfile), model);
            }

            model.FirstName = commerceUser.FirstName;
            model.Email = commerceUser.Email;
            model.EmailRepeat = commerceUser.Email;
            model.LastName = commerceUser.LastName;
            model.TelephoneNumber = commerceUser.GetPropertyValue("Phone") as string;

            return View(this.GetRenderingView(StorefrontConstants.Views.EditProfile), model);
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <returns>The view to display.</returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ForgotPassword()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Changees the password.
        /// </summary>
        /// <returns>Change password view</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ChangePassword()
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Forgots the password confirmation.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>The forgot password confirmation view</returns>
        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult ForgotPasswordConfirmation(string userName)
        {
            ViewBag.UserName = userName;

            return View(this.CurrentRenderingView);
        }        

        /// <summary>
        /// Handles a user trying to login
        /// </summary>
        /// <param name="provider">The name of the open Id provider</param>
        /// <returns>The view to display to the user</returns>     
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ActionName("Login")]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult LoginPost(string provider)
        {
            IdentityProviderClientConfigurationElement providerConfig = OpenIdConnectUtilities.GetIdentityProviderFromConfiguration(provider);
            switch (providerConfig.ProviderType)
            {
                case IdentityProviderType.OpenIdConnect:
                    ControllerContext.HttpContext.GetOwinContext().Authentication.Challenge(providerConfig.Name);
                    return new HttpUnauthorizedResult();

                case IdentityProviderType.ACS:
                    // Storing cookie with current provider (used in Logoff).
                    OpenIdConnectUtilities.SetCookie(this.HttpContext, OpenIdConnectUtilities.CookieCurrentProvider, providerConfig.Name);
                    OpenIdConnectUtilities.SetCookie(this.HttpContext, OpenIdConnectUtilities.CookieCurrentProviderType, providerConfig.ProviderType.ToString());

                    string url = string.Format(CultureInfo.InvariantCulture, "{0}v2/wsfederation?wa=wsignin1.0&wtrealm={1}", providerConfig.Issuer, providerConfig.RedirectUrl);
                    Response.Redirect(url, true);
                    break;

                default:
                    SecurityException securityException = new SecurityException(string.Format(CultureInfo.InvariantCulture, "The identity provider type {0} is not supported", providerConfig.ProviderType));
                    CommerceLog.Current.Error("LoginPost", this, securityException);
                    throw securityException;
            }

            return null;
        }

        /// <summary>
        /// Starts the sign up process.
        /// </summary>
        /// <param name="email">The email address from external identifier.</param>
        /// <param name="isActivationPending">if set to <c>true</c> [is activation pending].</param>
        /// <returns>
        /// The view for entering sign up information.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string email, bool? isActivationPending)
        {           
            RegisterModel registerViewModel = new RegisterModel()
            {  
                UserName = email               
            };

            if (isActivationPending == true)
            {
                registerViewModel.Errors.Add(StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CancelPendingRequest));
            }                  
            
            return View(this.CurrentRenderingView, registerViewModel);
        }

        /// <summary>
        /// Handles a user trying to register. Action invoked to finnish user sign up.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// The view to display to the user after creating new account or linking to existing account.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Register(RegisterUserInputModel inputModel)
        {
            RegisterBaseJsonResult result = new RegisterBaseJsonResult();
            try
            {
                Assert.ArgumentNotNull(inputModel, "RegisterInputModel");

                if (string.Equals(inputModel.SignupSelection, "NewAccount", StringComparison.OrdinalIgnoreCase))
                {
                    inputModel.Password = System.Web.Security.Membership.GeneratePassword(8, 4);
                    var response = this.AccountManager.RegisterUser(this.CurrentStorefront, inputModel);
                    if (response.ServiceProviderResult.Success && response.Result != null)
                    {
                        var isLoggedIn = this.AccountManager.Login(CurrentStorefront, CurrentVisitorContext, response.Result.UserName, false);
                        if (!isLoggedIn)
                        {
                            result.Success = false;
                            result.SetErrors(new List<string> { StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.CouldNotCreateUser) });
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.SetErrors(response.ServiceProviderResult);
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string emailOfExistingCustomer = inputModel.LinkupEmail;

                    var response = this.AccountManager.InitiateLinkToExistingCustomer(emailOfExistingCustomer);
                    if (response.ServiceProviderResult.Success && response.Result != null)
                    {
                        ////Clean up auth cookies completely. We need to be signed out.
                        OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieCurrentProvider);
                        OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieCurrentProviderType);
                        OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.OpenIdCookie);

                        result.UserName = response.Result.Name;
                        result.IsSignupFlow = true;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result.Success = false;
                        result.SetErrors(response.ServiceProviderResult);
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (AggregateException ex)
            {
                result.Success = false;
                result.SetErrors(StorefrontConstants.KnownActionNames.RegisterActionName, ex.InnerExceptions[0]);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Redirect("/login");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.SetErrors(StorefrontConstants.KnownActionNames.RegisterActionName, ex);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Action invoked to bring up screen to enter activation code to finalize account link-up.
        /// </summary>
        /// <param name="isSignupFlow">if set to <c>true</c> [is signup flow].</param>
        /// <param name="email">The email.</param>     
        /// <returns>
        /// The action.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security.Web.Configuration", "CA3147:MarkVerbHandlersWithValidateAntiforgeryToken", MessageId = "#ValidateAntiForgeryTokenAttributeDefaultMissing", Justification = "Support for anti-forgery token will be added once the controls are redesigned to follow MVC pattern.")]
        public ActionResult AccountLinkupPending(string isSignupFlow, string email)
        {
            CustomerLinkupPendingViewModel viewModel = new CustomerLinkupPendingViewModel();
            viewModel.Messages = new List<string>();
            if (!string.IsNullOrEmpty(isSignupFlow) && isSignupFlow.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                string maskedEmailAddress = Helpers.GetMaskedEmailAddress(email);
                string message = string.Format(CultureInfo.CurrentCulture, StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.ActivationCodeSent), maskedEmailAddress);
                viewModel.Messages.Add(message);
                viewModel.EmailAddressOfExistingCustomer = email;
            }          

            return this.View(this.GetRenderingView(StorefrontConstants.Views.UserPendingActivation), viewModel);
        }

        /// <summary>
        /// Action invoked to complete link up of an existing customer with an external identity.
        /// </summary>
        /// <param name="inputModel">The input model.</param>
        /// <returns>
        /// View for entering activation code to finalize account link up.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult FinalizeAccountLinkup(CustomerLinkupPendingInputModel inputModel)
        {
            BaseJsonResult result = new BaseJsonResult();
            
            this.ValidateModel(result);
            if (result.HasErrors)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var response = this.AccountManager.FinalizeLinkToExistingCustomer(inputModel.EmailAddressOfExistingCustomer, inputModel.ActivationCode);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    var providerName = GetAuthenticationProviderName(response.Result.Properties["ExternalIdentityProvider"] as string);
                    result.Data = new { ActivatedEmail = response.Result.Name, IdProvider = providerName };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.WrongActivationCode);
                    result.Success = false;
                    result.SetErrors(new List<string>() { message });
                    result.SetErrors(response.ServiceProviderResult);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Redirect("/login");
            }
            catch (Exception ex)
            {
                var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.WrongActivationCode);
                result.Success = false;
                result.SetErrors(new List<string>() { message, ex.Message });
                return Json(result, JsonRequestBehavior.AllowGet);                   
            }           
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyOrders()
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            try
            {
                var orders = this.OrderManager.GetOrders(this.CurrentVisitorContext.UserId, Context.Site.Name).Result;
                return View(this.CurrentRenderingView, orders.ToList());
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Redirect("/login");
            }
        }

        /// <summary>
        /// Orders the detail.
        /// </summary>
        /// <param name="id">The order confirmation Id.</param>
        /// <returns>
        /// The view to display order details
        /// </returns>
        [HttpGet]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyOrder(string id)
        {
            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            try
            {
                var response = this.OrderManager.GetOrderDetails(CurrentStorefront, CurrentVisitorContext, id);
                ViewBag.ItemShipping = response.Result.Lines.Count > 1 && (response.Result.Shipping == null || response.Result.Shipping.Count == 0);
                return View(this.CurrentRenderingView, response.Result);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Redirect("/login");
            }
        }

        /// <summary>
        /// Recent Orders PlugIn for Account Management Home Page
        /// </summary>
        /// <returns>The view to display recent orders</returns>
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult RecentOrders()
        {
            try
            {
                var recentOrders = new List<OrderHeader>();              
                var response = this.OrderManager.GetOrders(this.CurrentVisitorContext.UserId, Context.Site.Name);
                var result = new OrdersBaseJsonResult(response.ServiceProviderResult);
               
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    var orders = response.Result.ToList();
                    recentOrders = orders.Where(order => (order as CommerceOrderHeader).LastModified > DateTime.Today.AddDays(-30)).Take(5).ToList();
                }              

                result.Initialize(recentOrders);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [ValidateAntiForgeryToken]
        public ActionResult AccountHomeProfile()
        {
            var model = new ProfileModel();

            if (!Context.User.IsAuthenticated)
            {
                return Redirect("/login");
            }

            if (Context.User.IsAuthenticated && !Context.User.Profile.IsAdministrator)
            {
                var commerceUser = this.AccountManager.GetUser(Context.User.Name).Result;
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [StorefrontSessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
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
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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

                var addresses = new List<CommerceParty>();
                var response = this.AccountManager.RemovePartiesFromCurrentUser(this.CurrentStorefront, this.CurrentVisitorContext, model.ExternalId);
                var result = new AddressListItemJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success)
                {
                    addresses = this.AllAddresses(result);
                }

                result.Initialize(addresses, null);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        /// <returns>The view to display the updated address</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [HttpPost]
        [Authorize]
        [ValidateJsonAntiForgeryToken]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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

                var addresses = new List<CommerceParty>();
                var userResponse = this.AccountManager.GetUser(Context.User.Name);
                var result = new AddressListItemJsonResult(userResponse.ServiceProviderResult);
                if (userResponse.ServiceProviderResult.Success && userResponse.Result != null)
                {
                    var commerceUser = userResponse.Result;
                    var customer = new CommerceCustomer { ExternalId = commerceUser.ExternalId };
                    var party = new CommerceParty
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
                        // Verify we have not reached the maximum number of addresses supported.
                        int numberOfAddresses = this.AllAddresses(result).Count;
                        if (numberOfAddresses >= StorefrontManager.CurrentStorefront.MaxNumberOfAddresses)
                        {
                            var message = StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.MaxAddressLimitReached);
                            result.Errors.Add(string.Format(CultureInfo.InvariantCulture, message, numberOfAddresses));
                            result.Success = false;
                        }
                        else
                        {
                            var response = this.AccountManager.AddParties(this.CurrentStorefront, customer, new List<CommerceParty> { party });
                            result.SetErrors(response.ServiceProviderResult);
                            if (response.ServiceProviderResult.Success)
                            {
                                addresses = this.AllAddresses(result);
                            }

                            result.Initialize(addresses, null);
                        }
                    }
                    else
                    {
                        var response = this.AccountManager.UpdateParties(this.CurrentStorefront, customer, new List<CommerceParty> { party });
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
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        [StorefrontSessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
        public JsonResult GetCurrentUser()
        {
            try
            {
                if (!Sitecore.Context.User.IsAuthenticated || Context.User.Profile.IsAdministrator)
                {
                    var anonymousResult = new UserBaseJsonResult();
                    anonymousResult.Initialize(new CommerceUser());
                    return Json(anonymousResult, JsonRequestBehavior.AllowGet);
                }

                var response = this.AccountManager.GetUser(Context.User.Name);
                var result = new UserBaseJsonResult(response.ServiceProviderResult);
                if (response.ServiceProviderResult.Success && response.Result != null)
                {
                    result.Initialize(response.Result);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
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
            catch (Sitecore.Commerce.OpenIDConnectionClosedUnexpectedlyException)
            {
                this.CleanNotAuthorizedSession();
                return Json(new BaseJsonResult("Login"), JsonRequestBehavior.AllowGet);
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
            string defaultDomain;
            if (Context.Site != null && Context.Site.Domain != null && !string.IsNullOrEmpty(Context.Site.Domain.Name))
            {
                defaultDomain = Context.Site.Domain.Name;
            }
            else
            {
                defaultDomain = CommerceServerSitecoreConfig.Current.DefaultCommerceUsersDomain;
                if (string.IsNullOrWhiteSpace(defaultDomain))
                {
                    defaultDomain = CommerceConstants.ProfilesStrings.CommerceUsersDomainName;
                }
            }
            
            return !userName.StartsWith(defaultDomain, StringComparison.OrdinalIgnoreCase) ? string.Concat(defaultDomain, @"\", userName) : userName;
        }

        private static string GetAuthenticationProviderName(string authenticationProviderUrl)
        {
            IDictionary<string, IdentityProviderClientConfigurationElement> identityProviderDictionary = GetIdentityProvidersFromConfig();
            IdentityProviderClientConfigurationElement providerConfig = identityProviderDictionary.Values.Where(v => string.Equals(v.Issuer.OriginalString, authenticationProviderUrl, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (providerConfig != null)
            {
                return providerConfig.Name;
            }
            else
            {
                CommerceLog.Current.Error(string.Format(CultureInfo.CurrentCulture, "Online Store unsupported identity provider type encountered {0}", authenticationProviderUrl), typeof(AccountController));
                return authenticationProviderUrl;
            }
        }

        private static IDictionary<string, IdentityProviderClientConfigurationElement> GetIdentityProvidersFromConfig()
        {
            IDictionary<string, IdentityProviderClientConfigurationElement> identityProvierLookUp = new Dictionary<string, IdentityProviderClientConfigurationElement>();
        
            RetailConfiguration retailConfiguration = (RetailConfiguration)OpenIdConnectUtilities.DynamicsConnectorConfiguration.GetSection(OpenIdConnectUtilities.ConfigurationSectionName);
            foreach (IdentityProviderClientConfigurationElement provider in retailConfiguration.IdentityProviders)
            {
                identityProvierLookUp.Add(provider.Name, provider);
            }

            return identityProvierLookUp;
        }

        private ActionResult GetRedirectionBasedOnAssociatedCustomer(string authToken, IdentityProviderType identityProviderType, string email)
        {
            OpenIdConnectUtilities.SetTokenCookie(authToken); 
           
            var customerResult = AccountManager.GetCustomer().ServiceProviderResult;           
            CommerceCustomer customer = customerResult.CommerceCustomer;
            if (customerResult.Success && customer != null)
            {
                if (identityProviderType == IdentityProviderType.OpenIdConnect)
                {
                    OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieState);
                    OpenIdConnectUtilities.RemoveCookie(OpenIdConnectUtilities.CookieNonce);
                }

                return this.RegisterExistingUser(customer);
            }
            else 
            {
                string url = string.Format(CultureInfo.InvariantCulture, "/Register?isActivationPending={0}&email={1}", customerResult.Properties["IsRequestToLinkToExistingCustomerPending"], email);
                return Redirect(url);
            }            
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

        private List<CommerceParty> AllAddresses(AddressListItemJsonResult result)
        {
            var addresses = new List<CommerceParty>();
            var response = this.AccountManager.GetCurrentUserParties(this.CurrentStorefront, this.CurrentVisitorContext);
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
