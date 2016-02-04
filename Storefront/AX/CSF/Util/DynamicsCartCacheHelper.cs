//-----------------------------------------------------------------------
// <copyright file="DynamicsCartCacheHelper.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Dynamics cart cache helper class.</summary>
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

namespace Sitecore.Reference.Storefront
{
    /// <summary>
    /// Defines the DynamicsCartCacheHelper class.
    /// </summary>
    public class DynamicsCartCacheHelper : CartCacheHelper
    {
        /// <summary>
        /// Gets the customer identifier.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The AX customer id string.</returns>
        protected override string GetCustomerId(string customerId)
        {
            // AX customerid isn't a GUID, it's the AX account number so we simply return the same string back.
            return customerId;
        }
    }
}