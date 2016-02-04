//-----------------------------------------------------------------------
// <copyright file="ChildCategoriesComputedField.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Sitecore index computed field to save product child categories in the
// sequence defined in Commerce Server.</summary>
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
    using CommerceServer.Core.Catalog;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Commerce.Connect.CommerceServer.Catalog;
    using Sitecore.Commerce.Connect.CommerceServer.Search.ComputedFields;
    using Sitecore.Data;
    using Sitecore.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Defines the VariantInfoComputedField class.
    /// </summary>
    public class ChildCategoriesComputedField : BaseCommerceComputedField
    {
        private Lazy<IEnumerable<ID>> _validTemplates = new Lazy<IEnumerable<ID>>(() =>
        {
            return new List<ID>() 
            { 
                CommerceConstants.KnownTemplateIds.CommerceCategoryTemplate
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
        /// <returns>The computed value.  In this case we return serialized JSON.</returns>
        public override object ComputeValue(ContentSearch.IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            var validatedItem = GetValidatedItem(indexable);

            if (validatedItem == null)
            {
                return string.Empty;
            }

            List<string> childCategoryItemList = new List<string>();

            Category category = this.GetCategoryReadOnly(validatedItem.ID, validatedItem.Language.Name);
            if (category != null)
            {
                if (category.ChildCategories != null && category.ChildCategories.Count > 0)
                {
                    foreach (var childCategory in category.ChildCategories)
                    {
                        childCategoryItemList.Add(childCategory.ExternalId.ToString());
                    }
                }
            }

            return childCategoryItemList;
        }

        /// <summary>
        /// Gets the variant field value.
        /// </summary>
        /// <typeparam name="T">Type of the property to return.</typeparam>
        /// <param name="variant">The variant.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The request value.</returns>
        protected virtual T GetVariantFieldValue<T>(Variant variant, string fieldName)
        {
            if (variant.DataRow.Table.Columns.Contains(fieldName))
            {
                var variantValue = variant[fieldName];
                if (variantValue != null)
                {
                    if (variantValue is T)
                    {
                        return (T)variantValue;
                    }

                    return (T)Convert.ChangeType(variantValue, typeof(T), CultureInfo.InvariantCulture);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets the child variants read only.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="language">The language.</param>
        /// <returns>Product variant collection.</returns>
        private Category GetCategoryReadOnly(ID itemId, string language)
        {
            Category category = null;

            var catalogRepository = CommerceTypeLoader.CreateInstance<ICatalogRepository>();
            var externalInfo = catalogRepository.GetExternalIdInformation(itemId.Guid);

            if (externalInfo != null && externalInfo.CommerceItemType == CommerceItemType.Category)
            {
                var culture = CommerceUtility.ConvertLanguageToCulture(language);
                category = catalogRepository.GetCategoryReadOnly(externalInfo.CatalogName, externalInfo.CategoryName, culture) as Category;
            }

            return category;
        }
    }
}
