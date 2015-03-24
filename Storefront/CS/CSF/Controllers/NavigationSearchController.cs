//-----------------------------------------------------------------------
// <copyright file="NavigationSearchController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the NavigationSearchController class.</summary>
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
    using Sitecore.Commerce.Connect.CommerceServer.Search.Models;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Reference.Storefront.Models.SitecoreItemModels;
    using Sitecore.Mvc.Presentation;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Used to handle all search naviagtion actions
    /// </summary>
    public class NavigationSearchController : BaseController
    {
        #region Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationSearchController"/> class.
        /// </summary>
        /// <param name="contactFactory">The contact factory.</param>
        public NavigationSearchController([NotNull] ContactFactory contactFactory)
            : base(contactFactory)
        {
        }

        /// <summary>
        /// Gets or sets the name of the index
        /// </summary>
        public string IndexName { get; set; }

        #endregion

        #region Controller actions

        /// <summary>
        /// Represents the index action
        /// </summary>
        /// <returns>An action result view</returns>
        [HttpGet]
        [AllowAnonymous]
        public override ActionResult Index()
        {
            var dataSourceQuery = RenderingContext.Current.Rendering.DataSource;

            var navigationCategories = new List<Category>();
            if (!string.IsNullOrWhiteSpace(dataSourceQuery))
            {
                var response = SearchNavigation.GetNavigationCategories(dataSourceQuery, new CommerceSearchOptions());
                var navigationResults = response.ResponseItems;
                navigationCategories.AddRange(navigationResults.Select(result => new Category(result)));
            }

            return View(navigationCategories);
        }

        #endregion
    }
}
