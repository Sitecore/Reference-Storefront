//---------------------------------------------------------------------
// <copyright file="TriggerSearchPageEvent.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the processor that triggers the search page event.</summary>
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
    using Sitecore.Commerce.Pipelines.Common;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Services;
    using Sitecore.Sites;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Defines the TriggerSearchPageEvent class.
    /// </summary>
    public class TriggerSearchPageEvent : TriggerPageEvent
    {
        /// <summary>
        /// Executes the business logic of the TriggerPageEevent processor.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(Commerce.Pipelines.ServicePipelineArgs args)
        {
            if (Sitecore.Context.Site.DisplayMode != DisplayMode.Normal)
            {
                return;
            }

            base.Process(args);
        }

        /// <summary>
        /// Gets the page event text.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The page event text.
        /// </returns>
        protected override string GetPageEventText(Commerce.Pipelines.ServicePipelineArgs args)
        {
            var request = args.Request as SearchInitiatedRequest;
            if (request != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} ({1} results)", request.SearchTerm, request.NumberOfHits);
            }
            else
            {
                return base.GetPageEventText(args);
            }
        }

        /// <summary>
        /// Gets the page event data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The page event data.
        /// </returns>
        protected override Dictionary<string, object> GetPageEventData(Commerce.Pipelines.ServicePipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var data = base.GetPageEventData(args) ?? new Dictionary<string, object>();
            var request = args.Request as SearchInitiatedRequest;
            if (request != null)
            {
                data.Add(StorefrontConstants.PageEventDataNames.ShopName, request.ShopName);
                data.Add(StorefrontConstants.PageEventDataNames.SearchTerm, request.SearchTerm ?? string.Empty);
                data.Add(StorefrontConstants.PageEventDataNames.NumberOfHits, request.NumberOfHits);
            }

            return data;
        }
    }
}
