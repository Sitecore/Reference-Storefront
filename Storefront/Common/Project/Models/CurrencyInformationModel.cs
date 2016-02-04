//-----------------------------------------------------------------------
// <copyright file="CurrencyInformationModel.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Model used to return currency information.</summary>
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

namespace Sitecore.Reference.Storefront.Models
{
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the CurrencyInformationModel class.
    /// </summary>
    public class CurrencyInformationModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyInformationModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CurrencyInformationModel(Item item)
        {
            this.Name = item.Name;
            this.Description = item[StorefrontConstants.KnownFieldNames.CurrencyDescription];
            this.Symbol = item[StorefrontConstants.KnownFieldNames.CurrencySymbol];
            this.SymbolPosition = MainUtil.GetInt(item[StorefrontConstants.KnownFieldNames.CurrencySymbolPosition], 3);
            this.CurrencyNumberFormatCulture = item[StorefrontConstants.KnownFieldNames.CurrencyNumberFormatCulture];
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>
        /// The symbol.
        /// </value>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the symbol position.
        /// </summary>
        /// <value>
        /// The symbol position.
        /// </value>
        public int SymbolPosition { get; set; }

        /// <summary>
        /// Gets or sets the currency number format culture.
        /// </summary>
        /// <value>
        /// The currency number format culture.
        /// </value>
        public string CurrencyNumberFormatCulture { get; set; }
    }
}
