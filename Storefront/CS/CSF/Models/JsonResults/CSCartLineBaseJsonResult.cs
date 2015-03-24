//-----------------------------------------------------------------------
// <copyright file="CSCartLineBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the CS CartBaseJsonResult custom class.</summary>
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the CSCartLineBaseJsonResult class.
    /// </summary>
    public class CSCartLineBaseJsonResult : CartLineBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSCartLineBaseJsonResult"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        public CSCartLineBaseJsonResult(CustomCommerceCartLine line)
            : base(line)
        {
            if (line.Adjustments.Count > 0)
            {
                foreach (var adjustment in line.Adjustments)
                {
                    this.DiscountOfferNames.Add(adjustment.Description);
                }
            }
        }
    }
}