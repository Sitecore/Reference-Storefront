//-----------------------------------------------------------------------
// <copyright file="CardPaymentAcceptUrlJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the CardPaymentAcceptUrlJsonResult class.</summary>
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
    using System;
    using System.Globalization;
    using Diagnostics;
    using Sitecore.Commerce.Services.Payments;

    /// <summary>
    /// Defines the CardPaymentAcceptUrlJsonResult class.
    /// </summary>
    public class CardPaymentAcceptUrlJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardPaymentAcceptUrlJsonResult"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public CardPaymentAcceptUrlJsonResult(GetPaymentServiceUrlResult result)
            : base(result)
        {
            Assert.IsNotNull(result, "result");

            if (result.Success)
            {
                var serviceUrl = new Uri(result.Url);
                this.ServiceUrl = result.Url;
                this.MessageOrigin = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", serviceUrl.Scheme, serviceUrl.Authority);
            }
            else
            {
                this.SetErrors(result);
            }          
        }

        /// <summary>
        /// Gets or sets the payment service URL.
        /// </summary>
        /// <value>
        /// The payment service URL.
        /// </value>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the message origin.
        /// </summary>
        /// <value>
        /// The message origin.
        /// </value>
        public string MessageOrigin { get; set; }
    }
}
