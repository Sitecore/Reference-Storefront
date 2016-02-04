//-----------------------------------------------------------------------
// <copyright file="DynamicsStorefrontConstants.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Storefront constant definition.</summary>
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
    /// Defines the StorefrontConstants class.
    /// </summary>
    public static class DynamicsStorefrontConstants
    {
        /// <summary>
        /// Known storefront field names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownFieldNames
        {
            /// <summary>
            /// The maximum number of loyalty programs to join field.
            /// </summary>
            public static readonly string MaxNumberOfLoyaltyProgramsToJoin = "Max Number of Loyalty Programs To Join";

            /// <summary>
            /// The AX channel id.
            /// </summary>
            public static readonly string ChannelId = "ChannelId";
        }

        /// <summary>
        /// Known storefront properties names.
        /// </summary>
        public static class KnownPropertiesNames
        {
            /// <summary>
            /// The AX channel id.
            /// </summary>
            public static readonly string ChannelId = "ChannelId";
        }
    }
}