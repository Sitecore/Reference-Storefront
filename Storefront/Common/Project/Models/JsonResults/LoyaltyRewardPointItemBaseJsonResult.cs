//-----------------------------------------------------------------------
// <copyright file="LoyaltyRewardPointItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LoyaltyRewardPointItemBaseJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities.LoyaltyPrograms;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using Sitecore.Commerce.Services;

    /// <summary>
    /// Json result for loyalty reward point operations.
    /// </summary>
    public class LoyaltyRewardPointItemBaseJsonResult : BaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyRewardPointItemBaseJsonResult"/> class.
        /// </summary>
        public LoyaltyRewardPointItemBaseJsonResult()
            : base()
        {
            this.Transactions = new List<LoyaltyTransactionItemBaseJsonResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyRewardPointItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public LoyaltyRewardPointItemBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
            this.Transactions = new List<LoyaltyTransactionItemBaseJsonResult>();
        }
        
        /// <summary>
        /// Gets or sets the reward point identifier.
        /// </summary>
        /// <value>
        /// The reward point identifier.
        /// </value>
        public string RewardPointId { get; set; }

        /// <summary>
        /// Gets or sets the active points.
        /// </summary>
        /// <value>
        /// The active points.
        /// </value>
        public string ActivePoints { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        /// <value>
        /// The currency code.
        /// </value>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the expired points.
        /// </summary>
        /// <value>
        /// The expired points.
        /// </value>
        public string ExpiredPoints { get; set; }

        /// <summary>
        /// Gets or sets the issued points.
        /// </summary>
        /// <value>
        /// The issued points.
        /// </value>
        public string IssuedPoints { get; set; }

        /// <summary>
        /// Gets or sets the type of the reward point.
        /// </summary>
        /// <value>
        /// The type of the reward point.
        /// </value>
        public string RewardPointType { get; set; }

        /// <summary>
        /// Gets or sets the used points.
        /// </summary>
        /// <value>
        /// The used points.
        /// </value>
        public string UsedPoints { get; set; }

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        /// <value>
        /// The transactions.
        /// </value>
        public List<LoyaltyTransactionItemBaseJsonResult> Transactions { get; protected set; }

        /// <summary>
        /// Initializes the specified reward point.
        /// </summary>
        /// <param name="rewardPoint">The reward point.</param>
        public virtual void Initialize(LoyaltyRewardPoint rewardPoint)
        {
            Assert.ArgumentNotNull(rewardPoint, "rewardPoint");

            this.ActivePoints = rewardPoint.ActivePoints.ToString(Sitecore.Context.Language.CultureInfo);
            this.CurrencyCode = rewardPoint.CurrencyCode;
            this.Description = rewardPoint.Description;
            this.ExpiredPoints = rewardPoint.ExpiredPoints.ToString(Sitecore.Context.Language.CultureInfo);
            this.IssuedPoints = rewardPoint.IssuedPoints.ToString(Sitecore.Context.Language.CultureInfo);
            this.RewardPointType = rewardPoint.RewardPointType.Name;
            this.UsedPoints = rewardPoint.UsedPoints.ToString(Sitecore.Context.Language.CultureInfo);

            var transactions = rewardPoint.GetPropertyValue("Transactions") as List<LoyaltyCardTransaction>;
            if (transactions == null || transactions.Count <= 0)
            {
                return;
            }

            foreach (var transaction in transactions)
            {
                var result = new LoyaltyTransactionItemBaseJsonResult();
                result.Initialize(transaction);
                this.Transactions.Add(result);
            }
        }
    }
}