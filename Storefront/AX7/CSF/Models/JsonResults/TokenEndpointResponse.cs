//-----------------------------------------------------------------------
// <copyright file="TokenEndpointResponse.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the TokenEndpointResponse class.</summary>
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
    using System.Runtime.Serialization;

    /// <summary>
    /// Encapsulates properties returned by a request to the Token Endpoint.
    /// </summary>
    [DataContract]
    public class TokenEndpointResponse
    {
        /// <summary>
        /// Gets or sets the access_token.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the id_token.
        /// </summary>
        [DataMember(Name = "id_token")]
        public string IdToken { get; set; }

        /// <summary>
        /// Gets or sets the token's expiration.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the token's type.
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}