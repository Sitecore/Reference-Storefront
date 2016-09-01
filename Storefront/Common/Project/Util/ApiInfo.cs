//---------------------------------------------------------------------
// <copyright file="ApiInfo.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the ApiInfo class.</summary>
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

namespace Sitecore.Reference.Storefront.Util
{
    using System.Globalization;

    /// <summary>
    /// Class used to define the routes of our JS API calls.
    /// </summary>
    public class ApiInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        public ApiInfo(string name, string controller, string action)
        {
            this.Name = name;
            this.Controller = controller;
            this.Action = action;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string Url
        {
            get
            {
                return UrlBuilder.GetApiRoute(this.Controller.ToLower(CultureInfo.InvariantCulture) + "/" + this.Action.ToLower(CultureInfo.InvariantCulture));
            }
        }
    }
}
