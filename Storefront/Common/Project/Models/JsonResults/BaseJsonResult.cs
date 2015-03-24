//-----------------------------------------------------------------------
// <copyright file="BaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the BaseJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Services;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;

    /// <summary>
    /// Defines the BaseJsonResult class.
    /// </summary>
    public class BaseJsonResult : JsonResult
    {
        private readonly List<string> _errors = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult"/> class.
        /// </summary>
        public BaseJsonResult()
        {
            this.Success = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public BaseJsonResult(ServiceProviderResult result)
        {
            this.Success = true;

            if (result != null)
            {
                this.SetErrors(result);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJsonResult" /> class.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="exception">The exception.</param>
        public BaseJsonResult(string area, Exception exception)
        {
            this.Success = false;

            this.SetErrors(area, exception);
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public List<string> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return this._errors != null && this._errors.Any(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseJsonResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="result">The result.</param>
        public void SetErrors(ServiceProviderResult result)
        {
            this.Success = result.Success;
            if (result.SystemMessages.Count <= 0)
            {
                return;
            }

            var errors = result.SystemMessages;
            foreach (var error in errors)
            {
                var message = StorefrontManager.GetSystemMessage(error.Message);
                this.Errors.Add(string.IsNullOrEmpty(message) ? error.Message : message);
            }
        }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="exception">The exception.</param>
        public void SetErrors(string area, Exception exception)
        {
            this._errors.Add(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", area, exception.Message));
            this.Success = false;
        }

        /// <summary>
        /// Sets the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void SetErrors(List<string> errors)
        {
            if (!errors.Any())
            {
                return;
            }

            this.Success = false;
            this._errors.AddRange(errors);
        }
    }
}