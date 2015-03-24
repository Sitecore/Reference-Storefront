//-----------------------------------------------------------------------
// <copyright file="ManagerResponse.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Generic response returned by all manager classes.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Commerce.Services;

    /// <summary>
    /// Defined the ManagerResponse class.
    /// </summary>
    /// <typeparam name="TServiceProviderResult">The type of the service provider result.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ManagerResponse<TServiceProviderResult, TResult>
        where TServiceProviderResult : ServiceProviderResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerResponse{TServiceProviderResult, TResult}"/> class.
        /// </summary>
        /// <param name="serviceProviderResult">The service provider result instance.</param>
        /// <param name="result">The result.</param>
        public ManagerResponse(TServiceProviderResult serviceProviderResult, TResult result)
        {
            this.ServiceProviderResult = serviceProviderResult;
            this.Result = result;
        }

        /// <summary>
        /// Gets or sets the service provider result.
        /// </summary>
        /// <value>
        /// The service provider result.
        /// </value>
        public TServiceProviderResult ServiceProviderResult { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public TResult Result { get; set; }
    }
}