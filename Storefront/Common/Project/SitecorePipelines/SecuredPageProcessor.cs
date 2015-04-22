//-----------------------------------------------------------------------
// <copyright file="SecuredPageProcessor.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the Sitecore httpBeginRequest processor responsible for redirecting 
// Secured Pages to HTTPS.</summary>
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

namespace Sitecore.Reference.Storefront.SitecorePipelines
{
    using Sitecore.Data.Managers;
    using Sitecore.Pipelines;
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
            if (Sitecore.Context.Item != null)
            {
                var template = TemplateManager.GetTemplate(Sitecore.Context.Item);
                if (template != null)
                {
                    if (template.DescendsFromOrEquals(StorefrontConstants.KnownTemplateItemIds.SecuredPage))
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
}
