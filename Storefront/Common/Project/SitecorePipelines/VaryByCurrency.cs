//---------------------------------------------------------------------
// <copyright file="VaryByCurrency.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Handles the "Vary by Currency" cacheable option.</summary>
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
    using Sitecore.Mvc.Pipelines.Response.RenderRendering;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines the VaryByCurrency class.
    /// </summary>
    public class VaryByCurrency : RenderRenderingProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public override void Process(RenderRenderingArgs args)
        {
            if (args.Rendered
              || HttpContext.Current == null
              || (!args.Cacheable)
              || args.Rendering.RenderingItem == null)
            {
                return;
            }

            Item rendering = args.PageContext.Database.GetItem(args.Rendering.RenderingItem.ID);

            if (rendering == null || rendering["VaryByCurrency"] != "1")
            {
                return;
            }

            // When no cache key is present, we generate a full cache key; Otherwise we append to the existing ones.
            if (string.IsNullOrWhiteSpace(args.CacheKey))
            {
                args.CacheKey = string.Format(CultureInfo.InvariantCulture, "_#varyByCurrency_{0}_{1}_{2}_{3}_{4}", Context.Site.Name, Sitecore.Context.Language.Name, HttpContext.Current.Request.Url.AbsoluteUri, args.Rendering.RenderingItem.ID.ToString(), StorefrontManager.GetCustomerCurrency());
            }
            else
            {
                args.CacheKey = string.Format(CultureInfo.InvariantCulture, "_#varybyCurrency_{0}_{1}_{2}", args.CacheKey, args.Rendering.RenderingItem.ID.ToString(), StorefrontManager.GetCustomerCurrency());
            }
        }
    }
}
