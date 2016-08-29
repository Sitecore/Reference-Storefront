//-----------------------------------------------------------------------
// <copyright file="SecuredPageProcessor.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the Sitecore httpBeginRequest processor responsible for redirecting 
// Secured Pages to HTTPS.</summary>
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Sitecore.Data.Managers;
    using Sitecore.Pipelines;
    using Sitecore.Reference.Storefront.Managers;
    using System.Web;

    /// <summary>
    /// Defines the SecuredPageProcessor class.
    /// </summary>
    public class SecuredPageProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public virtual void Process(PipelineArgs args)
        {
            if (Sitecore.Context.Item != null && StorefrontManager.EnforceHttps)
            {
                // We must test specifically for Category and Product types because of the catalog item resolver who
                // changes the Sitecore.Context.Item and therefore it is not the page that is represented.
                var itemType = Sitecore.Context.Item.ItemType();
                if (itemType == StorefrontConstants.ItemTypes.SecuredPage ||
                    itemType == StorefrontConstants.ItemTypes.Product ||
                    itemType == StorefrontConstants.ItemTypes.Category)
                {
                    if (!HttpContext.Current.Request.IsSecureConnection)
                    {
                        string url = HttpContext.Current.Request.Url.ToString().Replace("http://", "https://");
                        HttpContext.Current.Response.Redirect(url, true);
                    }
                }
            }
        }
    }
}
