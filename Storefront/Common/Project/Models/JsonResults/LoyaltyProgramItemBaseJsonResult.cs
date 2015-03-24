//-----------------------------------------------------------------------
// <copyright file="LoyaltyProgramItemBaseJsonResult.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the LoyaltyProgramItemBaseJsonResult class.</summary>
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
    using System.Collections.Generic;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Entities.LoyaltyPrograms;
    using System;
    using System.Linq;

    /// <summary>
    /// Json result for loyalty program operations.
    /// </summary>
    public class LoyaltyProgramItemBaseJsonResult : BaseJsonResult
    {
        private readonly List<LoyaltyTierItemBaseJsonResult> _tiers = new List<LoyaltyTierItemBaseJsonResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyProgramItemBaseJsonResult"/> class.
        /// </summary>
        public LoyaltyProgramItemBaseJsonResult()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyProgramItemBaseJsonResult"/> class.
        /// </summary>
        /// <param name="result">The service provider result.</param>
        public LoyaltyProgramItemBaseJsonResult(ServiceProviderResult result)
            : base(result)
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the program identifier.
        /// </summary>
        /// <value>
        /// The program identifier.
        /// </value>
        public string ProgramId { get; set; }

        /// <summary>
        /// Gets the loyalty tiers.
        /// </summary>
        /// <value>
        /// The loyalty tiers.
        /// </value>
        public List<LoyaltyTierItemBaseJsonResult> LoyaltyTiers
        {
            get
            {
                return this._tiers;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyProgramItemBaseJsonResult" /> class.
        /// </summary>
        /// <param name="program">The program.</param>
        public virtual void Initialize(LoyaltyProgramStatus program)
        {
            Assert.ArgumentNotNull(program, "program");

            this.Name = program.Name;
            this.Description = program.Description;
            this.ProgramId = program.ExternalId;

            foreach (var tier in program.LoyaltyTiers)
            {
                var cardTier = program.LoyaltyCardTiers.FirstOrDefault(ct => ct.TierId.Equals(tier.TierId, StringComparison.OrdinalIgnoreCase));
                var result = new LoyaltyTierItemBaseJsonResult();
                result.Initialize(tier, cardTier);
                this._tiers.Add(result);
            }
        }
    }
}