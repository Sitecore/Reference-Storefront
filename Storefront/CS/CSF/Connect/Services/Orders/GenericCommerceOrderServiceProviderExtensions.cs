//-----------------------------------------------------------------------
// <copyright file="GenericCommerceOrderServiceProviderExtensions.cs" company="Sitecore Corporation">
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
    /// Extends the <see cref="CommerceOrderServiceProvider"/> class with some generic methods.
    /// It allows you to specify the result type of a custom service call.
    /// </summary>
    public static class GenericCommerceOrderServiceProviderExtensions
    {
        /// <summary>
        /// Gets available countries. It calls the GetAvailableCountries pipeline.
        /// </summary>
        /// <typeparam name="TGetAvailableCountriesRequest">The extended type of the  <see cref="GetAvailableCountriesRequest" />class.</typeparam>
        /// <typeparam name="TGetAvailableCountriesResult">The extended type of the <see cref="GetAvailableCountriesResult" />class.</typeparam>
        /// <param name="orderProvider">A <see cref="CommerceOrderServiceProvider"/> object.</param>
        /// <param name="request">The <see cref="GetAvailableCountriesRequest" /> Contains the search criteria.</param>
        /// <returns>The extended <see cref="GetAvailableCountriesResult" />.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), NotNull]
        public static TGetAvailableCountriesResult GetAvailableCountries<TGetAvailableCountriesRequest, TGetAvailableCountriesResult>([NotNull] this CommerceOrderServiceProvider orderProvider, [NotNull] TGetAvailableCountriesRequest request)
            where TGetAvailableCountriesRequest : GetAvailableCountriesRequest
            where TGetAvailableCountriesResult : GetAvailableCountriesResult, new()
        {
            return orderProvider.RunPipeline<GetAvailableCountriesRequest, TGetAvailableCountriesResult>(CommerceServerStorefrontConstants.PipelineNames.GetAvailableCountries, request);
        }

        /// <summary>
        /// Gets available states. It calls the GetAvailableRegions pipeline.
        /// </summary>
        /// <typeparam name="TGetAvailableRegionsRequest">The extended type of the  <see cref="GetAvailableRegionsRequest" />class.</typeparam>
        /// <typeparam name="TGetAvailableRegionsResult">The extended type of the <see cref="GetAvailableRegionsResult" />class.</typeparam>
        /// <param name="orderProvider">A <see cref="CommerceOrderServiceProvider" /> object.</param>
        /// <param name="request">The <see cref="GetAvailableRegionsRequest" /> Contains the search criteria.</param>
        /// <returns>
        /// The extended <see cref="GetAvailableRegionsResult" />.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), NotNull]
        public static TGetAvailableRegionsResult GetAvailableRegions<TGetAvailableRegionsRequest, TGetAvailableRegionsResult>([NotNull] this CommerceOrderServiceProvider orderProvider, [NotNull] TGetAvailableRegionsRequest request)
            where TGetAvailableRegionsRequest : GetAvailableRegionsRequest
            where TGetAvailableRegionsResult : GetAvailableRegionsResult, new()
        {
            return orderProvider.RunPipeline<GetAvailableRegionsRequest, TGetAvailableRegionsResult>(CommerceServerStorefrontConstants.PipelineNames.GetAvailableRegions, request);
        }
    }
}