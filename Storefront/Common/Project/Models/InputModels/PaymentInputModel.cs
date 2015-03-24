//-----------------------------------------------------------------------
// <copyright file="PaymentInputModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>InputModel model for setting payment information.</summary>
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

namespace Sitecore.Reference.Storefront.Models.InputModels
{
    /// <summary>
    /// Defines the PaymentInputModel class.
    /// </summary>
    public class PaymentInputModel : BaseInputModel
    {
        /// <summary>
        /// Gets or sets the credit card payment.
        /// </summary>
        /// <value>
        /// The credit card payment.
        /// </value>
        public CreditCardPaymentInputModelItem CreditCardPayment { get; set; }

        /// <summary>
        /// Gets or sets the gift card payment.
        /// </summary>
        /// <value>
        /// The gift card payment.
        /// </value>
        public GiftCardPaymentInputModelItem GiftCardPayment { get; set; }

        /// <summary>
        /// Gets or sets the loyalty card payment.
        /// </summary>
        /// <value>
        /// The loyalty card payment.
        /// </value>
        public LoyaltyCardPaymentInputModelItem LoyaltyCardPayment { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        /// <value>
        /// The billing address.
        /// </value>
        public PartyInputModelItem BillingAddress { get; set; }
    }
}
