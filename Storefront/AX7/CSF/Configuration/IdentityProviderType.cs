//-----------------------------------------------------------------------
// <copyright file="IdentityProviderType.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the IdentityProviderType class.</summary>
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

namespace Sitecore.Reference.Storefront.Configuration
{
    /// <summary>
    /// Defines supported Identity Providers.
    /// </summary>
    public enum IdentityProviderType
    {
        /// <summary>
        /// Indicates that Provider Type was not specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// OpenId Connect Provider.
        /// </summary>
        OpenIdConnect = 1,

        /// <summary>
        ///  ACS Provider.
        /// </summary>
        ACS = 2
    }
}