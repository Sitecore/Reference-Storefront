//-----------------------------------------------------------------------
// <copyright file="CartExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CartExtensions class.</summary>
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

namespace Sitecore.Commerce.Services.Carts
{
    using Sitecore.Commerce.Connect.CommerceServer.Orders.Pipelines;

    /// <summary>
    /// Some utility methods for working with cart requests
    /// </summary>
    public static class CartExtensions
    {
        /// <summary>
        /// Adds a value to a cart request indicating that the Commerce Server pipelines (.pcf) should/shouldn't be run on the cart
        /// </summary>
        /// <param name="request">The request to append to</param>
        /// <param name="refresh">if set to <c>true</c> run the Commerce Server pipelines.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "the is by design")]
        public static void RefreshCart(this CartRequest request, bool refresh)
        {
            var info = CartRequestInformation.Get(request);

            if (info == null)
            {
                info = new CartRequestInformation(request, refresh);
            }
            else
            {
                info.Refresh = refresh;
            }
        }
    }
}