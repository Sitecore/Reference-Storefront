//-----------------------------------------------------------------------
// <copyright file="WindsorHttpControllerActivator.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the WindsorHttpControllerActivator class.</summary>
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
  using System.Net.Http;
  using System.Web.Http.Controllers;
  using System.Web.Http.Dispatcher;
  using Castle.Windsor;

  /// <summary>
  /// Defines the WindsorHttpControllerActivator type.
  /// </summary>
  public class WindsorHttpControllerActivator : IHttpControllerActivator
  {
    /// <summary>
    /// The container.
    /// </summary>
    private readonly IWindsorContainer container;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindsorHttpControllerActivator"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public WindsorHttpControllerActivator(IWindsorContainer container)
    {
      this.container = container;
    }

    /// <summary>
    /// Creates an <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.
    /// </summary>
    /// <param name="request">The message request.</param>
    /// <param name="controllerDescriptor">The HTTP controller descriptor.</param>
    /// <param name="controllerType">The type of the controller.</param>
    /// <returns>
    /// An <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.
    /// </returns>
    public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
    {
      var controller = (IHttpController)this.container.Resolve(controllerType);

      request.RegisterForDispose(new Release(() => this.container.Release(controller)));

      return controller;
    }

    /// <summary>
    /// Defines the WindsorHttpControllerActivator.Release type.
    /// </summary>
    private class Release : IDisposable
    {
      /// <summary>
      /// The release.
      /// </summary>
      private readonly Action release;

      /// <summary>
      /// Initializes a new instance of the <see cref="Release"/> class.
      /// </summary>
      /// <param name="release">The release.</param>
      public Release(Action release)
      {
        this.release = release;
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
        this.release();
      }
    }
  }
}