//-----------------------------------------------------------------------
// <copyright file="AddressItemJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the AddressItemJsonResult class.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services;
    using CSFConnectModels = Sitecore.Reference.Storefront.Connect.Models;

    /// <summary>
    /// Json result for party operations.
    /// </summary>
    public class AddressItemJsonResult : AddressItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressItemJsonResult"/> class.
        /// </summary>
        public AddressItemJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressItemJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public AddressItemJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }       

        /// <summary>
        /// Initializes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public override void Initialize(Party address)
        {
            base.Initialize(address);            
            this.Name = ((CSFConnectModels.CommerceParty)address).Name;            
            this.IsPrimary = ((CSFConnectModels.CommerceParty)address).IsPrimary;            
        }
    }
}