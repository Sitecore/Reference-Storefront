//-----------------------------------------------------------------------
// <copyright file="CommerceOrderServiceProvider.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>The extended CS Integration Ordrer service provider.</summary>
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

namespace Sitecore.Reference.Storefront.Connect.Services.Orders
{
    using Sitecore.Reference.Storefront.Connect.Pipelines.Arguments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Represents the interaction with the wish list service.
    /// Service providers are wrapper objects that ease the interaction with the implementation pipelines. The providers only implement the calling pipelines.
    /// All the business logic is implemented in the pipeline processors.
    /// The CommerceOrderServiceProvider class implements the ServiceProvider class.
    /// </summary>
    public class CommerceOrderServiceProvider : Sitecore.Commerce.Connect.CommerceServer.Orders.CommerceOrderServiceProvider
    {
        /// <summary>
        /// Gets the available countries.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The service response</returns>
        [NotNull]
        public virtual GetAvailableCountriesResult GetAvailableCountries([NotNull]GetAvailableCountriesRequest request)
        {
            return this.RunPipeline<GetAvailableCountriesRequest, GetAvailableCountriesResult>(CommerceServerStorefrontConstants.PipelineNames.GetAvailableCountries, request);
        }

        /// <summary>
        /// Gets the available states.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The service response</returns>
        [NotNull]
        public virtual GetAvailableRegionsResult GetAvailableRegions([NotNull]GetAvailableRegionsRequest request)
        {
            return this.RunPipeline<GetAvailableRegionsRequest, GetAvailableRegionsResult>(CommerceServerStorefrontConstants.PipelineNames.GetAvailableRegions, request);
        }
    }
}