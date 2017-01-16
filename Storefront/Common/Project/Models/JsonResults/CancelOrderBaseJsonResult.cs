//-----------------------------------------------------------------------
// <copyright file="CancelOrderBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Emits the Json result for an order cancellation.</summary>
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
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Orders;

    /// <summary>
    /// Emits the Json result for an order cancellation.
    /// </summary>
    public class CancelOrderBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrderBaseJsonResult"/> class.
        /// </summary>
        public CancelOrderBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrderBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public CancelOrderBaseJsonResult(VisitorCancelOrderResult result)
            : base(result)
        {
            Assert.ArgumentNotNull(result, "result");
            if (result.CancellationStatus != null)
            {
                this.CancellationStatus = result.CancellationStatus.Name;
            }
        }

        /// <summary>
        /// Gets or sets the status of the order cancellation.
        /// </summary>
        public string CancellationStatus { get; set; }
    }
}
