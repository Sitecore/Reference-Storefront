//---------------------------------------------------------------------
// <copyright file="TriggerVisitedProductDetailsPagePageEvent.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the TriggerVisitedProductDetailsPagePageEvent class.</summary>
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Pipelines.Common;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Services;
    using Sitecore.Sites;

    /// <summary>
    /// Defines the processor that triggers the page event to track visits to the product details page.
    /// </summary>
    public class TriggerVisitedProductDetailsPagePageEvent : TriggerPageEvent
    {
        /// <summary>
        /// Executes the business logic of the TriggerPageEevent processor.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(ServicePipelineArgs args)
        {
            if (Sitecore.Context.Site.DisplayMode != DisplayMode.Normal)
            {
                return;
            }

            base.Process(args);
        }

        /// <summary>
        /// Gets the page event data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The page event data.</returns>
        protected override Dictionary<string, object> GetPageEventData(ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var data = base.GetPageEventData(args) ?? new Dictionary<string, object>();
            var request = args.Request as VisitedProductDetailsPageRequest;
            if (request != null)
            {
                data.Add(StorefrontConstants.PageEventDataNames.ProductId, request.ProductId);
                data.Add(StorefrontConstants.PageEventDataNames.ParentCategoryName, request.ParentCategoryName ?? string.Empty);
                data.Add(StorefrontConstants.PageEventDataNames.CatalogName, request.CatalogName ?? string.Empty);
            }

            return data;
        }
    }
}