//-----------------------------------------------------------------------
// <copyright file="LineShippingOptionBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Line shipping options JSON result.</summary>
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

namespace Sitecore.Reference.Storefront.Models.JsonResults
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Entities.Shipping;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the LineShippingOptionBaseJsonResult class.
    /// </summary>
    public class LineShippingOptionBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineShippingOptionBaseJsonResult"/> class.
        /// </summary>
        public LineShippingOptionBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the line identifier.
        /// </summary>
        /// <value>
        /// The line identifier.
        /// </value>
        public string LineId { get; set; }

        /// <summary>
        /// Gets or sets the shipping options.
        /// </summary>
        /// <value>
        /// The shipping options.
        /// </value>
        public IEnumerable<ShippingOptionBaseJsonResult> ShippingOptions { get; set; }

        /// <summary>
        /// Initializes the specified line shipping option.
        /// </summary>
        /// <param name="lineShippingOption">The line shipping option.</param>
        public void Initialize(LineShippingOption lineShippingOption)
        {
            if (lineShippingOption == null)
            {
                return;
            }

            this.LineId = lineShippingOption.LineId;

            var shippingOptionList = new List<ShippingOptionBaseJsonResult>();

            if (lineShippingOption.ShippingOptions != null)
            {
                foreach (var shippingOption in lineShippingOption.ShippingOptions)
                {
                    var jsonResult = CommerceTypeLoader.CreateInstance<ShippingOptionBaseJsonResult>();

                    jsonResult.Initialize(shippingOption);
                    shippingOptionList.Add(jsonResult);
                }
            }

            this.ShippingOptions = shippingOptionList;
        }
    }
}
