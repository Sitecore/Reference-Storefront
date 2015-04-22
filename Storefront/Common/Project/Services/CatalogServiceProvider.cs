//---------------------------------------------------------------------
// <copyright file="CatalogServiceProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CatalogServiceProvider class.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.Services
{
    using Sitecore.Commerce.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Represents the interaction with the catalog service.
    /// Service providers are wrapper objects that ease the interaction with the implementation pipelines. The providers only implement the calling pipelines. 
    /// All the business logic is implemented in the pipeline processors.
    /// The CatalogServiceProvider class implements the ServiceProvider class.
    /// </summary>
    public class CatalogServiceProvider : ServiceProvider
    {
        /// <summary>
        /// Registers an event specifying that the category page has been visited.
        /// </summary>
        /// <param name="request">The service request.</param>
        /// <returns>The service response.</returns>
        [NotNull]
        public virtual VisitedCategoryPageResult VisitedCategoryPage([NotNull]VisitedCategoryPageRequest request)
        {
            return this.RunPipeline<VisitedCategoryPageRequest, VisitedCategoryPageResult>(StorefrontConstants.PipelineNames.VisitedCategoryPage, request);
        }

        /// <summary>
        /// Registers an event specifying that the product details page has been visited.
        /// </summary>
        /// <param name="request">The service request.</param>
        /// <returns>The service response.</returns>
        [NotNull]
        public virtual VisitedProductDetailsPageResult VisitedProductDetailsPage([NotNull]VisitedProductDetailsPageRequest request)
        {
            return this.RunPipeline<VisitedProductDetailsPageRequest, VisitedProductDetailsPageResult>(StorefrontConstants.PipelineNames.VisitedProductDetailsPage, request);
        }

        /// <summary>
        /// Searches the initiated.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The service response.</returns>
        [NotNull]
        public virtual SearchInitiatedResult SearchInitiated([NotNull] SearchInitiatedRequest request)
        {
            return this.RunPipeline<SearchInitiatedRequest, SearchInitiatedResult>(StorefrontConstants.PipelineNames.SearchInitiated, request);
        }
    }
}