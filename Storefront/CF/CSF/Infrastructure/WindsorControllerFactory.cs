//-----------------------------------------------------------------------
// <copyright file="WindsorControllerFactory.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the WindsorControllerFactory class.</summary>
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

namespace Sitecore.Reference.Storefront.Infrastructure
{
    using Castle.MicroKernel;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// The windsor controller factory.
    /// </summary>
    public class WindsorControllerFactory : WindsorControllerFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactory" /> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public WindsorControllerFactory(IKernel kernel)
            : base(kernel)
        {
        }

        /// <summary>
        /// Determines whether the specified object is from current assembly.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if it is from the current assembly and false otherwise</returns>
        protected override bool IsFromCurrentAssembly(Type type)
        {
            if (type != null)
            {
                var currentAssembly = Assembly.GetExecutingAssembly().FullName;
                var controllerAssembly = type.Assembly.FullName;

                if (currentAssembly.Equals(controllerAssembly, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}