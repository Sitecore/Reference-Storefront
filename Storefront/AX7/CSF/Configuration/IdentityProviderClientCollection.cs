//-----------------------------------------------------------------------
// <copyright file="IdentityProviderClientCollection.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the IdentityProviderClientCollection class.</summary>
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
    /// Represents a collection of Identity Provider Clients.
    /// </summary>
    public class IdentityProviderClientCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates new element of the collection.
        /// </summary>
        /// <returns>Newly created element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new IdentityProviderClientConfigurationElement();
        }

        /// <summary>
        /// Gets element's key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IdentityProviderClientConfigurationElement)element).Name;
        }
    }
}