//-----------------------------------------------------------------------
// <copyright file="RetailConfiguration.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the RetailConfiguration class.</summary>
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
    using System.Configuration;

    /// <summary>
    /// Represents Retail Configuration used to setup an application.
    /// </summary>
    public class RetailConfiguration : ConfigurationSection
    {
        private const string PropIdentityProviders = "identityProviders";

        /// <summary>
        /// Gets the Identity Providers.
        /// </summary>
        [ConfigurationProperty(PropIdentityProviders)]
        [ConfigurationCollection(typeof(IdentityProviderClientCollection))]
        public IdentityProviderClientCollection IdentityProviders
        {
            get
            {
                return (IdentityProviderClientCollection)this[PropIdentityProviders];
            }
        }
    }
}