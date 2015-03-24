//-----------------------------------------------------------------------
// <copyright file="LoyaltyTransactionItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LoyaltyTransactionItemBaseJsonResult class.</summary>
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
    using Sitecore.Diagnostics;
    using Sitecore.Commerce.Entities.LoyaltyPrograms;

    /// <summary>
    /// Json result for loyalty transaction operations.
    /// </summary>
    public class LoyaltyTransactionItemBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public string ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the entry time.
        /// </summary>
        /// <value>
        /// The entry time.
        /// </value>
        public string EntryTime { get; set; }

        /// <summary>
        /// Gets or sets the entry date.
        /// </summary>
        /// <value>
        /// The entry date.
        /// </value>
        public string EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the entry.
        /// </summary>
        /// <value>
        /// The type of the entry.
        /// </value>
        public string EntryType { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public string Points { get; set; }

        /// <summary>
        /// Gets or sets the store.
        /// </summary>
        /// <value>
        /// The store.
        /// </value>
        public string Store { get; set; }
        
        /// <summary>
        /// Initializes the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public virtual void Initialize(LoyaltyCardTransaction transaction)
        {
            Assert.ArgumentNotNull(transaction, "transaction");

            this.ExternalId = transaction.ExternalId;
            this.EntryTime = transaction.EntryDateTime.ToShortTimeString();
            this.EntryDate = transaction.EntryDateTime.ToShortDateString();
            this.EntryType = transaction.EntryType.Name;
            this.ExpirationDate = transaction.ExpirationDate.ToShortDateString();
            this.Points = transaction.RewardPointAmount.ToString(Sitecore.Context.Language.CultureInfo);
            this.Store = transaction.ShopName;
        }
    }
}