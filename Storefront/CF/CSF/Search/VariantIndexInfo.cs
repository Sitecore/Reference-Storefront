//-----------------------------------------------------------------------
// <copyright file="VariantIndexInfo.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Product variant information stored in the "VariantInfo" index document property.</summary>
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

namespace Sitecore.Reference.Storefront.Search.ComputedFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the VariantIndexInfo class.
    /// </summary>
    public class VariantIndexInfo
    {
        /// <summary>
        /// Gets or sets the variant identifier.
        /// </summary>
        /// <value>
        /// The variant identifier.
        /// </value>
        public string VariantId { get; set; }

        /// <summary>
        /// Gets or sets the list price.
        /// </summary>
        /// <value>
        /// The list price.
        /// </value>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the base price.
        /// </summary>
        /// <value>
        /// The base price.
        /// </value>
        public decimal BasePrice { get; set; }
    }
}
