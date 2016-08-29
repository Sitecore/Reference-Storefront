//-----------------------------------------------------------------------
// <copyright file="IdentityProviderClientConfigurationElement.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the IdentityProviderClient class.</summary>
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
    using System;
    using System.Configuration;

    /// <summary>
    /// Defines IdentityProvider configuration element.
    /// </summary>
    public class IdentityProviderClientConfigurationElement : ConfigurationElement
    {
        private const string PropName = "name";
        private const string PropIssuer = "issuer";
        private const string PropClientId = "clientId";
        private const string PropClientSecret = "clientSecret";
        private const string PropRedirectUrl = "redirectUrl";
        private const string PropLogOffUrl = "logOffUrl";
        private const string PropImageUrl = "imageUrl";
        private const string PropProviderType = "providerType";
        private const string PropDomainHint = "domainHint";

        /// <summary>
        /// Gets the identity Provider's Name.
        /// </summary>
        [ConfigurationProperty(PropName)]
        public string Name
        {
            get
            {
                return (string)this[PropName];
            }
        }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        [ConfigurationProperty(PropIssuer)]
        public Uri Issuer
        {
            get
            {
                return (Uri)this[PropIssuer];
            }
        }

        /// <summary>
        /// Gets the Client ID registered with the Identity Provider.
        /// </summary>
        [ConfigurationProperty(PropClientId)]
        public string ClientId
        {
            get
            {
                return (string)this[PropClientId];
            }
        }

        /// <summary>
        /// Gets the Client Secret issued by the Identity Provider.
        /// </summary>
        [ConfigurationProperty(PropClientSecret)]
        public string ClientSecret
        {
            get
            {
                return (string)this[PropClientSecret];
            }
        }

        /// <summary>
        /// Gets the Redirect URL registered with the Identity Provider.
        /// </summary>
        [ConfigurationProperty(PropRedirectUrl)]
        public Uri RedirectUrl
        {
            get
            {
                return (Uri)this[PropRedirectUrl];
            }
        }

        /// <summary>
        /// Gets the identity Provider Type.
        /// </summary>
        [ConfigurationProperty(PropProviderType)]
        public IdentityProviderType ProviderType
        {
            get
            {
                return (IdentityProviderType)this[PropProviderType];
            }
        }

        /// <summary>
        /// Gets the Domain Hint.
        /// </summary>
        /// <remarks>Identifies social identity provider.</remarks>
        [ConfigurationProperty(PropDomainHint)]
        public string DomainHint
        {
            get
            {
                return (string)this[PropDomainHint];
            }
        }

        /// <summary>
        /// Gets the External provider's log off URL.
        /// </summary>
        [ConfigurationProperty(PropLogOffUrl)]
        public Uri LogOffUrl
        {
            get
            {
                return (Uri)this[PropLogOffUrl];
            }
        }

        /// <summary>
        /// Gets the provider image url.
        /// </summary>
        [ConfigurationProperty(PropImageUrl)]
        public Uri ImageUrl
        {
            get
            {
                return (Uri)this[PropImageUrl];
            }
        }
    }
}