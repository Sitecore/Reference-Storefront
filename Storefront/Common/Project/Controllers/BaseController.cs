//-----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the BaseController class.</summary>
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
    using System.Web.Mvc;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Data.Items;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Reference.Storefront.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Diagnostics;
    using System.Linq;
    using Sitecore.Reference.Storefront.Models.JsonResults;

    /// <summary>
    /// Base class for Controllers
    /// </summary>
    public class BaseController : Mvc.Controllers.SitecoreController
    {
        private Catalog _currentCatalog;
        private ISiteContext _siteContext;
        private ICommerceSearchManager _currentSearchManager;
        private VisitorContext _currentVisitorContext;
        private ContactFactory _contactFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="contactFactory">The contact factory.</param>
        public BaseController(ContactFactory contactFactory)
        {
            Assert.ArgumentNotNull(contactFactory, "contactFactory");

            this._contactFactory = contactFactory;
        }

        /// <summary>
        /// Gets the contact factory.
        /// </summary>
        /// <value>
        /// The contact factory.
        /// </value>
        public ContactFactory ContactFactory 
        { 
            get 
            { 
                return this._contactFactory; 
            } 
        }

        /// <summary>
        /// Gets the current sitecontext
        /// </summary>
        /// <value>
        /// The current site context.
        /// </value>
        public ISiteContext CurrentSiteContext
        {
            get
            {
                return _siteContext ?? (_siteContext = CommerceTypeLoader.CreateInstance<ISiteContext>());
            }
        }

        /// <summary>
        /// Gets the Current Visitor Context
        /// </summary>
        public virtual VisitorContext CurrentVisitorContext
        {
            get
            {
                return _currentVisitorContext ?? (_currentVisitorContext = new VisitorContext(this.ContactFactory));
            }
        }

        /// <summary>
        /// Gets the Current Storefront being accessed
        /// </summary>
        public CommerceStorefront CurrentStorefront
        {
            get
            {
                return StorefrontManager.CurrentStorefront;
            }
        }

        /// <summary>
        /// Gets the current catalog being accessed
        /// </summary>
        public Catalog CurrentCatalog
        {
            get
            {
                if (_currentCatalog == null)
                {
                    _currentCatalog = CurrentStorefront.DefaultCatalog;
                    if (_currentCatalog == null)
                    {
                        //There was no catalog associated with the storefront or we are not using multi-storefront
                        //So we use the default catalog
                        _currentCatalog = new Catalog(Context.Database.GetItem(CommerceConstants.KnownItemIds.DefaultCatalog));
                    }
                }

                return _currentCatalog;
            }
        }

        /// <summary>
        /// Gets the currently loaded Search Manager
        /// Instantiates a new one if one is not loaded
        /// </summary>
        public ICommerceSearchManager CurrentSearchManager
        {
            get
            {
                return _currentSearchManager ?? (_currentSearchManager = CommerceTypeLoader.CreateInstance<ICommerceSearchManager>());
            }
        }

        /// <summary>
        /// Gets the current rendering under which the controller action is running
        /// </summary>
        protected Rendering CurrentRendering
        {
            get
            {
                return RenderingContext.Current.Rendering;
            }
        }

        /// <summary>
        /// Gets the view name associated to the current rendering
        /// </summary>
        protected string CurrentRenderingView
        {
            get
            {
                return this.GetRenderingView();
            }
        }

        /// <summary>
        /// Gets or sets the item under which the current controller action is running
        /// </summary>
        protected Item Item
        {
            get
            {
                return RenderingContext.Current.Rendering.Item;
            }

            set
            {
                RenderingContext.Current.Rendering.Item = value;
            }
        }

        /// <summary>
        /// Gets the NoDatasource View
        /// </summary>
        /// <returns>The view indicating no data source was found.</returns>
        public ActionResult GetNoDataSourceView()
        {
            if (!Context.PageMode.IsPageEditor)
            {
                return new EmptyResult();
            }

            var home = Context.Database.GetItem(Context.Site.RootPath + Context.Site.StartItem);
            var noDatasourceItemPath = home["No Datasource Item"];

            Item noDatasourceItem = null;
            if (!string.IsNullOrEmpty(noDatasourceItemPath))
            {
                noDatasourceItem = Context.Database.GetItem(noDatasourceItemPath);
            }

            NoDataSourceViewModel viewModel = null;
            if (noDatasourceItem != null)
            {
                viewModel = new NoDataSourceViewModel(noDatasourceItem);
            }

            if (viewModel == null)
            {
                return new EmptyResult();
            }

            return View(this.GetRenderingView("NoDatasource"), viewModel);
        }
        
        /// <summary>
        /// Validates the model.
        /// </summary>
        /// <param name="result">The result.</param>
        public virtual void ValidateModel(BaseJsonResult result)
        {
            if (ModelState.IsValid)
            {
                return;
            }

            var errors = (from modelValue in ModelState.Values.Where(modelValue => modelValue.Errors.Any()) from error in modelValue.Errors select error.ErrorMessage).ToList();
            result.SetErrors(errors);
        }

        /// <summary>
        /// Gets the rendering view.
        /// </summary>
        /// <param name="renderingViewName">Name of the rendering view.</param>
        /// <returns>The fully qualified path to the view.</returns>
        protected string GetRenderingView(string renderingViewName = null)
        {
            var shopName = StorefrontManager.CurrentStorefront.ShopName;
            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();

            if (String.IsNullOrWhiteSpace(renderingViewName))
            {
                renderingViewName = RenderingContext.Current.Rendering.RenderingItem.Name;
            }

            // TODO: mbe - Once tothers have checked in their code, rename the View/ShoppingCart folder to View/Cart.
            if (controllerName.Equals("Cart", StringComparison.OrdinalIgnoreCase))
            {
                controllerName = "ShoppingCart";
            }
            else if (controllerName.Equals("WishList", StringComparison.OrdinalIgnoreCase))
            {
                controllerName = "Account";
            }
            else if (controllerName.Equals("Loyalty", StringComparison.OrdinalIgnoreCase))
            {
                controllerName = "Account";
            }

            const string RenderingViewPathFormatString = "~/Views/{0}/{1}/{2}.cshtml";

            var renderingViewPath = string.Format(System.Globalization.CultureInfo.InvariantCulture, RenderingViewPathFormatString, shopName, controllerName, renderingViewName);

            var result = ViewEngines.Engines.FindView(ControllerContext, renderingViewPath, null);

            if (result.View != null)
            {
                return renderingViewPath;
            }

            return string.Format(System.Globalization.CultureInfo.InvariantCulture, RenderingViewPathFormatString, shopName, "Shared", renderingViewName);
        }

        /// <summary>
        /// Gets the absolute rendering view.
        /// </summary>
        /// <param name="subpath">The subpath.</param>
        /// <returns>A fully qualified path to the view containing the current storefront.</returns>
        protected string GetAbsoluteRenderingView(string subpath)
        {
            var shopName = StorefrontManager.CurrentStorefront.ShopName;

            if (!subpath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                subpath = subpath.Insert(0, "/");
            }

            const string RenderingViewPathFormatString = "~/Views/{0}{1}.cshtml";
            var renderingViewPath = string.Format(System.Globalization.CultureInfo.InvariantCulture, RenderingViewPathFormatString, shopName, subpath);

            return renderingViewPath;
        }
    }
}