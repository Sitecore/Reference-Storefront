//-----------------------------------------------------------------------
// <copyright file="AvailableStatesBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AvailableStatesBaseJsonResult class.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The Json result of a request to retrieve the available states.
    /// </summary>
    public class AvailableStatesBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableStatesBaseJsonResult"/> class.
        /// </summary>
        public AvailableStatesBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableStatesBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public AvailableStatesBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the available states.
        /// </summary>
        public Dictionary<string, string> States { get; set; }

        /// <summary>
        /// Initializes the specified states.
        /// </summary>
        /// <param name="states">The states.</param>
        public virtual void Initialize(Dictionary<string, string> states)
        {
            Assert.ArgumentNotNull(states, "states");

            this.States = states;
        }
    }
}