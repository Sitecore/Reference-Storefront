//-----------------------------------------------------------------------
// <copyright file="HtmlExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
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

namespace Sitecore.Reference.Storefront.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web;
    using Sitecore;
    using Sitecore.Analytics;

    /// <summary>
    /// Extensions for working with HTML and HtmlHelpers.
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Returns the visitor identification snippet if needed
        /// </summary>
        /// <param name="sitecoreHelper">The sitecore helper.</param>
        /// <returns>
        /// HtmlString with the identification snippet
        /// </returns>
        public static HtmlString AnalyticsVisitorIdentification(this Sitecore.Mvc.Helpers.SitecoreHelper sitecoreHelper)
        {
            if (Context.Diagnostics.Tracing || Context.Diagnostics.Profiling)
            {
                return new HtmlString("<!-- Visitor identification is disabled because debugging is active. -->");
            }

            if (!Tracker.IsActive)
            {
                return MvcHtmlString.Empty;
            }

            return new HtmlString("<link href=\"/layouts/System/VisitorIdentification.aspx\" rel=\"stylesheet\" type=\"text/css\" />");
        }
    }
}