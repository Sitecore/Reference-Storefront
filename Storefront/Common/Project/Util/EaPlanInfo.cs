//-----------------------------------------------------------------------
// <copyright file="EaPlanInfo.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the EaPlanInfo class.</summary>
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

namespace Sitecore.Reference.Storefront
{
    /// <summary>
    /// A class used to conatin info about engagement plans that will be created during installation
    /// </summary>
    public class EaPlanInfo
    { 
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the EaPlan id
        /// </summary>
        public string EaPlanId { get; set; }

        /// <summary>
        /// Gets or sets the item id
        /// </summary>
        public string ItemId { get; set; }
    }
}