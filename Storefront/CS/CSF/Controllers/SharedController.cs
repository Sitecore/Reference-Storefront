//-----------------------------------------------------------------------
// <copyright file="SharedController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SharedController class.</summary>
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
    using Sitecore.Commerce.Contacts;
    using Sitecore.Mvc.Presentation;
    using System.Web.Mvc;

    /// <summary>
    /// Used to handle shared account actions
    /// </summary>
    public class SharedController : BaseController
    {
        private readonly RenderingModel _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedController"/> class.
        /// </summary>
        /// <param name="contactFactory">The contact factory.</param>
        public SharedController(ContactFactory contactFactory)
            : base(contactFactory) 
        {
            _model = new RenderingModel();
        }

        /// <summary>
        /// Gets the current rendering model.
        /// </summary>
        /// <value>
        /// The current rendering model.
        /// </value>
        internal RenderingModel CurrentRenderingModel
        {
            get
            {
                _model.Initialize(this.CurrentRendering);
                return _model;
            }
        }

        /// <summary>
        /// Gets Layout.
        /// </summary>
        /// <returns>Storefront layout</returns>
        public ActionResult Layout()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Navigation extra structure.
        /// </summary>
        /// <returns>NavigationExtraStructure view</returns>
        public ActionResult NavigationExtraStructure()
        {
            return View(this.GetRenderingView("Structures/NavigationExtraStructure"));
        }

        /// <summary>
        /// Gets the Navigation structure.
        /// </summary>
        /// <returns>NavigationStructure view</returns>
        public ActionResult NavigationStructure()
        {
            return View(this.GetRenderingView("Structures/NavigationStructure"));
        }

        /// <summary>
        /// Gets the Single column.
        /// </summary>
        /// <returns>SingleColumn view</returns>
        public ActionResult SingleColumn()
        {
            return View(this.GetRenderingView("Structures/SingleColumn"));
        }

        /// <summary>
        /// Gets the Three column.
        /// </summary>
        /// <returns>ThreeColumn view</returns>
        public ActionResult ThreeColumn()
        {
            return View(this.GetRenderingView("Structures/ThreeColumn"), this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Top structure.
        /// </summary>
        /// <returns>TopStructure view</returns>
        public ActionResult TopStructure()
        {
            return View(this.GetRenderingView("Structures/TopStructure"));
        }

        /// <summary>
        /// Gets the Two column center right.
        /// </summary>
        /// <returns>TwoColumnCenterRight view</returns>
        public ActionResult TwoColumnCenterRight()
        {
            return View(this.GetRenderingView("Structures/TwoColumnCenterRight"), this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Two column left center.
        /// </summary>
        /// <returns>TwoColumnLeftCenter view</returns>
        public ActionResult TwoColumnLeftCenter()
        {
            return View(this.GetRenderingView("Structures/TwoColumnLeftCenter"), this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Breadcrumbs.
        /// </summary>
        /// <returns>Breadcrumbs view</returns>
        public ActionResult Breadcrumbs()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Error view.
        /// </summary>
        /// <returns>Error view</returns>
        public ActionResult Error()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Error image.
        /// </summary>
        /// <returns>ErrorImage view</returns>
        public ActionResult ErrorImage()
        {
            return View(this.CurrentRenderingView, this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Errors summary.
        /// </summary>
        /// <returns>ErrorsSummary view</returns>
        public ActionResult ErrorsSummary()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Footer navigation.
        /// </summary>
        /// <returns>FooterNavigation view</returns>
        public ActionResult FooterNavigation()
        {
            return View(this.CurrentRenderingView, this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets th Language selector.
        /// </summary>
        /// <returns>LanguageSelector view</returns>
        public ActionResult LanguageSelector()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Logo.
        /// </summary>
        /// <returns>Logo view</returns>
        public ActionResult Logo()
        {
            return View(this.CurrentRenderingView, this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Share and print view.
        /// </summary>
        /// <returns>ShareAndPrint view</returns>
        public ActionResult ShareAndPrint()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Prints the only.
        /// </summary>
        /// <returns>The PrintOnly view.</returns>
        public ActionResult PrintOnly()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Social connector.
        /// </summary>
        /// <returns>SocialConnector view</returns>
        public ActionResult SocialConnector()
        {
            return View(this.CurrentRenderingView);
        }

        /// <summary>
        /// Gets the Title text.
        /// </summary>
        /// <returns>TitleText view</returns>
        public ActionResult TitleText()
        {
            return View(this.CurrentRenderingView, this.CurrentRenderingModel);
        }

        /// <summary>
        /// Gets the Top bar links.
        /// </summary>
        /// <returns>TopBarLinks view</returns>
        public ActionResult TopBarLinks()
        {
            return View(this.CurrentRenderingView, this.CurrentRenderingModel);
        }
    }
}