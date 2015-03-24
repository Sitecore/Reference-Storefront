//-----------------------------------------------------------------------
// <copyright file="WindsorConfig.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the WindsorConfig class.</summary>
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

using Sitecore.Reference.Storefront;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WindsorConfig), "ConfigureContainer")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(WindsorConfig), "ReleaseContainer")]

namespace Sitecore.Reference.Storefront
{
    using System.Web.Http.Dispatcher;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Sitecore.Reference.Storefront.Infrastructure;

    /// <summary>
    /// Configures Castle Windsor IoC container.
    /// </summary>
    public static class WindsorConfig
    {
        /// <summary>
        /// The container.
        /// </summary>
        internal static readonly IWindsorContainer Container;

        /// <summary>
        /// Initializes static members of the <see cref="WindsorConfig"/> class. 
        /// </summary>
        static WindsorConfig()
        {
            Container = new WindsorContainer().Install(FromAssembly.This());
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        public static void ConfigureContainer()
        {
            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Container.Kernel));
            System.Web.Http.GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorHttpControllerActivator(Container));
        }

        /// <summary>
        /// Releases the container.
        /// </summary>
        public static void ReleaseContainer()
        {
            Container.Dispose();
        }
    }
}