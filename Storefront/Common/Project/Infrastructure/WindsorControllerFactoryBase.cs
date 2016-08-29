//-----------------------------------------------------------------------
// <copyright file="WindsorControllerFactoryBase.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the WindsorControllerFactory class.</summary>
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
    using Castle.MicroKernel;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Linq;
    using Sitecore.Reference.Storefront.Managers;

    /// <summary>
    /// The windsor controller factory.
    /// </summary>
    public abstract class WindsorControllerFactoryBase : DefaultControllerFactory
    {
        /// <summary>
        /// The kernel.
        /// </summary>
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorControllerFactoryBase" /> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        protected WindsorControllerFactoryBase(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Releases the specified controller.
        /// </summary>
        /// <param name="controller">The controller to release.</param>
        public override void ReleaseController(IController controller)
        {
            if (this.IsFromCurrentAssembly(controller.GetType()))
            {
                this.kernel.ReleaseComponent(controller);
            }
            else
            {
                base.ReleaseController(controller);
            }
        }

        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// The controller instance.
        /// </returns>
        /// <exception cref="System.Web.HttpException">Error 40.4</exception>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (this.IsFromCurrentAssembly(controllerType))
            {
                if (controllerType == null)
                {
                    throw new HttpException(404, string.Format(CultureInfo.InvariantCulture, "The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
                }

                return (IController)this.kernel.Resolve(controllerType);
            }

            var controller = base.GetControllerInstance(requestContext, controllerType);

            return controller;
        }

        /// <summary>
        /// Determines whether the specified object is from current assembly.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if it is from the current assembly and false otherwise</returns>
        protected abstract bool IsFromCurrentAssembly(Type type);

        /// <summary>
        /// Returns the controller's session behavior.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>
        /// The controller's session behavior.
        /// </returns>
        protected override System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, Type controllerType)
        {
            if (!StorefrontManager.ReadOnlySessionStateBehaviorEnabled)
            {
                return System.Web.SessionState.SessionStateBehavior.Required;
            }

            var actionName = requestContext.RouteData.Values["action"].ToString();
            MethodInfo actionMethodInfo;

            try
            {
                actionMethodInfo = controllerType.GetMethod(actionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }
            catch (AmbiguousMatchException /* matchExc */)
            {
                var httpRequestTypeAttr =
                    requestContext.HttpContext.Request.RequestType.Equals("POST")
                        ? typeof(HttpPostAttribute)
                        : typeof(HttpGetAttribute);

                actionMethodInfo =
                    controllerType.GetMethods().FirstOrDefault(
                        mi =>
                        mi.Name.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) && mi.GetCustomAttributes(httpRequestTypeAttr, false).Length > 0);
            }

            if (actionMethodInfo != null)
            {
                var actionSessionStateAttr = actionMethodInfo.GetCustomAttributes(typeof(StorefrontSessionStateAttribute), false)
                                 .OfType<StorefrontSessionStateAttribute>()
                                 .FirstOrDefault();

                if (actionSessionStateAttr != null)
                {
                    System.Diagnostics.Debug.WriteLine("{0}: {1}", actionName, actionSessionStateAttr.Behavior.ToString());
                    return actionSessionStateAttr.Behavior;
                }
            }

            var defaultBehavior = base.GetControllerSessionBehavior(requestContext, controllerType);
            System.Diagnostics.Debug.WriteLine("{0}: {1}", actionName, defaultBehavior.ToString());

            return defaultBehavior;
        }
    }
}