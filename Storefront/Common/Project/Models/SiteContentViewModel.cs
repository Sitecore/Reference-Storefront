//-----------------------------------------------------------------------
// <copyright file="SiteContentViewModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the SiteContentViewModel class.</summary>
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

namespace Sitecore.Reference.Storefront.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Model that represents a summary of a site content item.
    /// </summary>
    public class SiteContentViewModel
    {
        /// <summary>
        /// The maximum summary title length.
        /// </summary>
        public const int MaxTitleLength = 100;

        /// <summary>
        /// Gets or sets the site content item.
        /// </summary>
        public Sitecore.Data.Items.Item Item { get; set; }

        /// <summary>
        /// Gets or sets the path to the site content page.
        /// </summary>
        public string ContentPath { get; set; }

        /// <summary>
        /// Gets or sets the site content summary title.
        /// </summary>
        public string SummaryTitle { get; set; }

        /// <summary>
        /// Gets or sets the site content summary text.
        /// </summary>
        public string SummaryText { get; set; }

        /// <summary>
        /// Creates a <see cref="SiteContentViewModel"/> based on a Sitecore item.
        /// </summary>
        /// <param name="item">The sitecore item.</param>
        /// <returns>A <see cref="SiteContentViewModel"/>.</returns>
        public static SiteContentViewModel Create(Sitecore.Data.Items.Item item)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(item, "item");
            var model = new SiteContentViewModel();
            model.Item = item;
            model.ContentPath = Sitecore.Links.LinkManager.GetItemUrl(item);
            model.SummaryTitle = Helpers.TrimTextToLength(item[StorefrontConstants.ItemFields.Title], MaxTitleLength);
            model.SummaryText = item[StorefrontConstants.ItemFields.SummaryText];

            return model;
        }
    }
}