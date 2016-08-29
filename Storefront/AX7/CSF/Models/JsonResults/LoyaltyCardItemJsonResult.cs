//-----------------------------------------------------------------------
// <copyright file="LoyaltyCardItemJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the LoyaltyCardItemJsonResult class.</summary>
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
    using Sitecore.Commerce.Entities.LoyaltyPrograms;
    using Sitecore.Commerce.Services;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;

    /// <summary>
    /// Json result for loyalty cards operations.
    /// </summary>
    public class LoyaltyCardItemJsonResult : LoyaltyCardItemBaseJsonResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyCardItemJsonResult"/> class.
        /// </summary>
        public LoyaltyCardItemJsonResult()
            : base()
        {
            this.Programs = new List<LoyaltyProgramItemBaseJsonResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyCardItemJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public LoyaltyCardItemJsonResult(ServiceProviderResult result)
            : base(result)
        {
            this.Programs = new List<LoyaltyProgramItemBaseJsonResult>();
        }

        /// <summary>
        /// Gets or sets the programs.
        /// </summary>
        /// <value>
        /// The programs.
        /// </value>
        public List<LoyaltyProgramItemBaseJsonResult> Programs { get; protected set; }

        /// <summary>
        /// Initializes the specified loyalty card.
        /// </summary>
        /// <param name="loyaltyCard">The loyalty card.</param>
        public override void Initialize(LoyaltyCard loyaltyCard)
        {
            Assert.ArgumentNotNull(loyaltyCard, "loyaltyCard");

            this.CardNumber = loyaltyCard.CardNumber;

            foreach (var point in loyaltyCard.RewardPoints)
            {
                var result = new LoyaltyRewardPointItemJsonResult();
                result.Initialize(point);
                this.RewardPoints.Add(result);
            }

            foreach (var program in ((Sitecore.Commerce.Connect.DynamicsRetail.Entities.LoyaltyPrograms.LoyaltyCard)loyaltyCard).LoyaltyPrograms)
            {
                var result = new LoyaltyProgramItemBaseJsonResult();
                result.Initialize(program);
                this.Programs.Add(result);
            }
        }
    }
}