//---------------------------------------------------------------------
// <copyright file="ApplyVaryByUrlPath.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Handles the "Vary by Url Path" cacheable option.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Pipelines.Response.RenderRendering;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Defines the ApplyVaryByUrlPath class,
    /// </summary>
    public class ApplyVaryByUrlPath : RenderRenderingProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(RenderRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.Rendered
              || HttpContext.Current == null
              || (!args.Cacheable)
              || string.IsNullOrWhiteSpace(args.CacheKey)
              || args.Rendering.RenderingItem == null)
            {
                return;
            }

            Item rendering = args.PageContext.Database.GetItem(args.Rendering.RenderingItem.ID);

            if (rendering == null || rendering["VaryByUrlPath"] != "1")
            {
                return;
            }

            args.CacheKey = string.Format(CultureInfo.InvariantCulture, "_#varyByUrlPath_{0}_{1}_{2}_{3}", Context.Site.Name, Sitecore.Context.Language.Name, HttpContext.Current.Request.Url.AbsoluteUri, args.Rendering.RenderingItem.ID.ToString());
        }
    }
}
