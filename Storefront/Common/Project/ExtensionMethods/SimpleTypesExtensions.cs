//---------------------------------------------------------------------
// <copyright file="SimpleTypesExtensions.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Extensions for dealing with Commerce Server entities.</summary>
//---------------------------------------------------------------------
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

namespace Sitecore.Reference.Storefront.Extensions
{
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using System;
    using System.Globalization;

    /// <summary>
    /// Some utility extension methods for simple types
    /// </summary>
    public static class SimpleTypesExtensions
    {
        /// <summary>
        /// Turns a decimal value into a currency string
        /// </summary>
        /// <param name="currency">The decimal object to act on</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns>
        /// A decimal formatted as a string
        /// </returns>
        public static string ToCurrency(this decimal? currency, string currencyCode)
        {
            if (currency.HasValue)
            {
                return currency.Value.ToCurrency(currencyCode);
            }
            else
            {
                return 0M.ToCurrency(currencyCode);
            }
        }

        /// <summary>
        /// Turns a decimal value into a currency string
        /// </summary>
        /// <param name="currency">The decimal object to act on</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns>
        /// A decimal formatted as a string
        /// </returns>
        public static string ToCurrency(this decimal currency, string currencyCode)
        {
            NumberFormatInfo info;
            CurrencyInformationModel currencyInfo = StorefrontManager.GetCurrencyInformation(currencyCode);
            if (currencyInfo != null)
            {
                info = (NumberFormatInfo)CultureInfo.GetCultureInfo(currencyInfo.CurrencyNumberFormatCulture).NumberFormat.Clone();
                info.CurrencySymbol = currencyInfo != null ? currencyInfo.Symbol : currencyCode;
                info.CurrencyPositivePattern = currencyInfo.SymbolPosition;
            }
            else
            {
                info = (NumberFormatInfo)CultureInfo.GetCultureInfo(Sitecore.Context.Language.Name).NumberFormat.Clone();
            }

            return currency.ToString("C", info);
        }

        /// <summary>
        /// To the displayed date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The formatted date based on the selected culture.</returns>
        public static string ToDisplayedDate(this DateTime date)
        {
            return date.ToString("d", Context.Language.CultureInfo);
        }
    }
}