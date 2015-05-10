//-----------------------------------------------------------------------
// <copyright file="CommerceServerStorefrontConstants.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Storefront constant definition.</summary>
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

namespace Sitecore.Reference.Storefront
{
    /// <summary>
    /// Defines the StorefrontConstants class.
    /// </summary>
    public static class CommerceServerStorefrontConstants
    {
        /// <summary>
        /// PipelineNames constants
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class PipelineNames
        {
            /// <summary>
            /// The get available regions pipeline name
            /// </summary>
            public const string GetAvailableRegions = "commerce.orders.getAvailableRegions";

            /// <summary>
            /// The get country region information pipeline name
            /// </summary>
            public const string GetAvailableCountries = "commerce.orders.getAvailableCountries";

            /// <summary>
            /// The translate entity to commerce profile pipeline name
            /// </summary>
            public const string TranslateEntityToCommerceAddressProfile = "translate.entityToCommerceAddressProfile";

            /// <summary>
            /// The translate commerce profile to entity pipeline name
            /// </summary>
            public const string TranslateCommerceAddressProfileToEntity = "translate.commerceAddressProfileToEntity";
        }

        /// <summary>
        /// Known storefront field names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownFieldNames
        {
            /// <summary>
            /// The commerce server payment methods field.
            /// </summary>
            public const string CommerceServerPaymentMethods = "CS Payment Methods";

            /// <summary>
            /// The commerce server shipping methods field.
            /// </summary>
            public const string CommerceServerShippingMethods = "CS Shipping Methods";

            /// <summary>
            /// The country location path field
            /// </summary>
            public const string CountryLocationPath = "Country location path";

            /// <summary>
            /// The country name field.
            /// </summary>
            public const string CountryName = "Name";

            /// <summary>
            /// The country code field.
            /// </summary>
            public const string CountryCode = "Country Code";

            /// <summary>
            /// The payment option value field.
            /// </summary>
            public const string PaymentOptionValue = "Payment Option Value";

            /// <summary>
            /// The region name field.
            /// </summary>
            public const string RegionName = "Name";

            /// <summary>
            /// The shipping option value field.
            /// </summary>
            public const string ShippingOptionValue = "Shipping Option Value";

            /// <summary>
            /// The shipping options location path field.
            /// </summary>
            public const string ShippingOptionsLocationPath = "Shipping Options location path";

            /// <summary>
            /// The supports wishlists field.
            /// </summary>
            public const string SupportsWishLists = "Supports Wishlists";

            /// <summary>
            /// The supports loyalty program field.
            /// </summary>
            public const string SupportsLoyaltyProgram = "Supports Loyalty Program ";

            /// <summary>
            /// The supports girst card payment field.
            /// </summary>
            public const string SupportsGirstCardPayment = "Supports Girft Card Payment";

            /// <summary>
            /// The value field.
            /// </summary>
            public const string Value = "Value";
        }

        /// <summary>
        /// Used to hold some of the default settings for the site
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class CartConstants
        {
            /// <summary>
            /// Name of the Billing address prefix
            /// </summary>
            public const string BillingAddressNamePrefix = "Billing_";

            /// <summary>
            /// Name of the shipping address prefix
            /// </summary>
            public const string ShippingAddressNamePrefix = "Shipping_";

            /// <summary>
            /// The email address name prefix
            /// </summary>
            public const string EmailAddressNamePrefix = ShippingAddressNamePrefix + "Email_";
        }

        /// <summary>
        /// Defines the Know Commerce Server weakly typed properties.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class KnowWeaklyTypesProperties
        {
            /// <summary>
            /// The email text property.
            /// </summary>
            public const string EmailText = "EmailText";

            /// <summary>
            /// The party type property.
            /// </summary>
            public const string PartyType = "PartyType";
        }
    }
}