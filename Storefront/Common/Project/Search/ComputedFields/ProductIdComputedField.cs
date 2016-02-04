//-----------------------------------------------------------------------
// <copyright file="ProductIdComputedField.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Sitecore index computed field to lower case the product id.</summary>
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

namespace Sitecore.Reference.Storefront.Search.ComputedFields
{
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Search.ComputedFields;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the ProductIdComputedField class.
    /// </summary>
    public class ProductIdComputedField : BaseCommerceComputedField
    {
        private Lazy<IEnumerable<ID>> _validTemplates = new Lazy<IEnumerable<ID>>(() =>
        {
            return new List<ID>() 
            { 
                CommerceConstants.KnownTemplateIds.CommerceProductTemplate
            };
        });

        /// <summary>
        /// Gets the list of valid templates for this computed value
        /// </summary>
        protected override IEnumerable<ID> ValidTemplates
        {
            get
            {
                return this._validTemplates.Value;
            }
        }

        /// <summary>
        /// Computes the value.
        /// </summary>
        /// <param name="indexable">The indexable.</param>
        /// <returns>The lower cased product id only for product template items.</returns>
        public override object ComputeValue(ContentSearch.IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var validatedItem = GetValidatedItem(indexable);

            if (validatedItem == null || string.IsNullOrWhiteSpace(validatedItem.Name))
            {
                return string.Empty;
            }

            return validatedItem.Name.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
