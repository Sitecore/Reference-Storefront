//-----------------------------------------------------------------------
// <copyright file="StorefrontSessionStateAttribute.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the Storefront session state method attribute.</summary>
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

namespace Sitecore.Reference.Storefront.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.SessionState;

    /// <summary>
    /// Defines the StorefrontSessionStateAttribute class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class StorefrontSessionStateAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorefrontSessionStateAttribute"/> class.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        public StorefrontSessionStateAttribute(SessionStateBehavior behavior)
        {
            this.Behavior = behavior;
        }

        /// <summary>
        /// Gets the behavior.
        /// </summary>
        /// <value>
        /// The behavior.
        /// </value>
        public SessionStateBehavior Behavior { get; private set; }
    }
}
