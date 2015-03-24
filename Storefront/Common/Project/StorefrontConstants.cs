//-----------------------------------------------------------------------
// <copyright file="StorefrontConstants.cs" company="Sitecore Corporation">
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
    using Sitecore.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the StorefrontConstants class.
    /// </summary>
    public static class StorefrontConstants
    {
        /// <summary>
        /// Used to hold some of the default settings for the site
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// The default site name
            /// </summary>
            public static readonly string WebsiteName = "Storefront";

            /// <summary>
            /// The default number of items per page
            /// </summary>
            public static readonly int DefaultItemsPerPage = 12;
        }

        /// <summary>
        /// Contains Engagement Plan creation information.  The following Ids and names will be used to create the engagment plans for the site
        /// </summary>
        public static class EngagementPlans
        {
            /// <summary>
            /// The Abandoned Carts engagement plan Id to use when creating the Abandon carts eaplan.
            /// </summary>
            public static readonly string AbandonedCartsEaPlanId = "{7138ACC1-329C-4070-86DD-6A53D6F57AC5}";

            /// <summary>
            /// The Abandoned Carts name
            /// </summary>
            public static readonly string AbandonedCartsEaPlanName = "Abandoned Carts";

            /// <summary>
            /// The New Order Placed engagement plan template Id.
            /// </summary>
            public static readonly string NewOrderPlacedEaPlanId = "{7CA697EA-5CCA-4B59-85A3-D048B285E6B4}";

            /// <summary>
            /// The New Order Placed name
            /// </summary>
            public static readonly string NewOrderPlacedEaPlanName = "New Order Placed";

            /// <summary>
            /// The Products Back In Stock engagement plan template Id.
            /// </summary>
            public static readonly string ProductsBackInStockEaPlanId = "{36B4083E-F7F7-4E60-A747-75DDBEC6BB4B}";

            /// <summary>
            /// The Products Back In Stock name
            /// </summary>
            public static readonly string ProductsBackInStockEaPlanName = "Products Back In Stock";

            /// <summary>
            /// The WishList Created engagement plan template Id.
            /// </summary>
            public static readonly string WishListCreatedEaPlanId = "{C6BD2A27-3528-4107-8764-DB010EA400FF}";

            /// <summary>
            /// The WishList Created engagement plan template Id.
            /// </summary>
            public static readonly string WishListCreatedEaPlanName = "Wish List Created";
        }

        /// <summary>
        /// Known storefront field names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownFieldNames
        {
            /// <summary>
            /// The cancel field.
            /// </summary>
            public static readonly string Cancel = "Cancel";

            /// <summary>
            /// The create user field.
            /// </summary>
            public static readonly string CreateUser = "Create user";

            /// <summary>
            /// The customer message1 field.
            /// </summary>
            public static readonly string CustomerMessage1 = "Customer Message 1";

            /// <summary>
            /// The customer message2
            /// </summary>
            public static readonly string CustomerMessage2 = "Customer Message 2";

            /// <summary>
            /// The email
            /// </summary>
            public static readonly string Email = "Email";

            /// <summary>
            /// The email address placeholder field.
            /// </summary>
            public static readonly string EmailAddressPlaceholder = "Email Address Placeholder";

            /// <summary>
            /// The email missing message
            /// </summary>
            public static readonly string EmailMissingMessage = "Email Missing Message";

            /// <summary>
            /// The facebook button field.
            /// </summary>
            public static readonly string FacebookButton = "Facebook Button";

            /// <summary>
            /// The facebook text field.
            /// </summary>
            public static readonly string FacebookText = "Facebook Text";

            /// <summary>
            /// The first name
            /// </summary>
            public static readonly string FirstName = "First Name";

            /// <summary>
            /// The first name placeholder field.
            /// </summary>
            public static readonly string FirstNamePlaceholder = "First Name Placeholder";

            /// <summary>
            /// The fill form message
            /// </summary>
            public static readonly string FillFormMessage = "Fill Form Message";

            /// <summary>
            /// The guest checkout button
            /// </summary>
            public static readonly string GuestCheckoutButton = "Guest Checkout Button";

            /// <summary>
            /// The last name
            /// </summary>
            public static readonly string LastName = "Last Name";

            /// <summary>
            /// The last name placeholder field.
            /// </summary>
            public static readonly string LastNamePlaceholder = "Last Name Placeholder";

            /// <summary>
            /// The password
            /// </summary>
            public static readonly string Password = "Password";

            /// <summary>
            /// The passwords do not match message field.
            /// </summary>
            public static readonly string PasswordsDoNotMatchMessage = "Passwords Do Not Match Message";

            /// <summary>
            /// The password length message field.
            /// </summary>
            public static readonly string PasswordLengthMessage = "Password Length Message";

            /// <summary>
            /// The password missing message field.
            /// </summary>
            public static readonly string PasswordMissingMessage = "Password Missing Message";

            /// <summary>
            /// The password again field.
            /// </summary>
            public static readonly string PasswordAgain = "Password Again";

            /// <summary>
            /// The password placholder field.
            /// </summary>
            public static readonly string PasswordPlaceholder = "Password Placeholder";

            /// <summary>
            /// The registering
            /// </summary>
            public static readonly string Registering = "Registering";

            /// <summary>
            /// The sign in button field.
            /// </summary>
            public static readonly string SignInButton = "Sign In Button";

            /// <summary>
            /// The signing button
            /// </summary>
            public static readonly string SigningButton = "Signing Button";

            /// <summary>
            /// The show when autenticated field.
            /// </summary>
            public static readonly string ShowWhenAuthenticated = "Show when Authenticated";

            /// <summary>
            /// The show always field.
            /// </summary>
            public static readonly string ShowAlways = "Show Always";

            /// <summary>
            /// The body field.
            /// </summary>
            public static readonly string Body = "Body";

            /// <summary>
            /// The subject field.
            /// </summary>
            public static readonly string Subject = "Subject";

            /// <summary>
            /// The key field.
            /// </summary>
            public static readonly string Key = "Key";

            /// <summary>
            /// The value field.
            /// </summary>
            public static readonly string Value = "Value";

            /// <summary>
            /// The sender email address field.
            /// </summary>
            public static readonly string SenderEmailAddress = "Sender Email Address";
        }

        /// <summary>
        /// Known template item IDs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownTemplateItemIds
        {
            /// <summary>
            /// The home template id
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID Home = new ID("{FB9DBD60-CBA2-490D-9C72-997271D576A3}");

            /// <summary>
            /// The ID of the Standard Page template.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID StandardPage = new ID("{16E859D2-6542-407A-AC65-F34BCAD3EB3D}");
        }

        /// <summary>
        /// Known template names.
        /// </summary>
        public static class KnowItemNames
        {
            /// <summary>
            /// The mails template name
            /// </summary>
            public static readonly string Mails = "Mails";

            /// <summary>
            /// The lookups template name
            /// </summary>
            public static readonly string Lookups = "Lookups";

            /// <summary>
            /// The system messages template name
            /// </summary>
            public static readonly string SystemMessages = "System Messages";

            /// <summary>
            /// The inventory statuses template name
            /// </summary>
            public static readonly string InventoryStatuses = "Inventory Statuses";
        }

        /// <summary>
        /// Known item IDs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownItemIds
        {
            /// <summary>
            /// The Product Purchase outcome item Id
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID ProductPurchaseOutcome = new ID("{9016E456-95CB-42E9-AD58-997D6D77AE83}");

            /// <summary>
            /// The CommerceConnect engagement plan parent Id.
            /// </summary>
            public static readonly string CommerceConnectEaPlanParentId = "{03402BEE-21E9-458A-B3F4-D004CC4F21FA}";

            /// <summary>
            /// The Abandoned Carts engagement plan template Id.
            /// </summary>
            public static readonly string AbandonedCartsEaPlanBranchTemplateId = "{8C90E12F-4E2E-4E3D-9137-B2D5F5DD40C0}";

            /// <summary>
            /// The New Order Placed engagement plan template Id.
            /// </summary>
            public static readonly string NewOrderPlacedEaPlanBranchTemplateId = "{6F6A861F-78CF-4859-8AD8-7A2D5CCDBEB6}";

            /// <summary>
            /// The Products Back In Stock engagement plan template Id.
            /// </summary>
            public static readonly string ProductsBackInStockEaPlanBranchTemplateId = "{534EE43B-00B1-49D0-92A7-E78B9C127B00}";

            /// <summary>
            /// The WishList Created engagement plan template Id.
            /// </summary>
            public static readonly string WishListCreatedEaPlanBranchTemplateId = "{35E75C72-4985-4E09-88C3-0EAC6CD1E64F}";

            /// <summary>
            /// The deploy command Id.
            /// </summary>
            public static readonly string DeployCommandId = "{4044A9C4-B583-4B57-B5FF-2791CB0351DF}";
        }

        /// <summary>
        /// Known site context custom properties.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownSiteContextProperties
        {
            /// <summary>
            /// The shop name
            /// </summary>
            public static readonly string ShopName = "shopName";
        }

        /// <summary>
        /// Contains constants that represent page event data names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class PageEventDataNames
        {
            /// <summary>
            /// The name of the ProductId page event data.
            /// </summary>
            public static readonly string ProductId = "ProductId";

            /// <summary>
            /// The name of the ParentCategoryName page event data.
            /// </summary>
            public static readonly string ParentCategoryName = "ParentCategoryName";

            /// <summary>
            /// The name of the CatalogName page event data.
            /// </summary>
            public static readonly string CatalogName = "CatalogName";

            /// <summary>
            /// The name of the CategoryName page event data.
            /// </summary>
            public static readonly string CategoryName = "CategoryName";
        }

        /// <summary>
        /// Contains constants that represent Sitecore pipeline names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class PipelineNames
        {
            /// <summary>
            /// The name of the VisitedProductDetailsPage pipeline.
            /// </summary>
            public const string VisitedProductDetailsPage = Prefix + "visitedProductDetailsPage";

            /// <summary>
            /// The name of the VisitedCategoryPage pipeline.
            /// </summary>
            public const string VisitedCategoryPage = Prefix + "visitedCategoryPage";

            /// <summary>
            /// The prefix to be added in front of all pipelines names.
            /// </summary>
            private const string Prefix = "commerce.storefront.";
        }

        /// <summary>
        /// Used to store strings using in query strings
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class QueryStrings
        {
            /// <summary>
            /// Used for paging
            /// </summary>
            public const string Paging = "pg";

            /// <summary>
            /// Used for site content paging
            /// </summary>
            public const string SiteContentPaging = "scpg";

            /// <summary>
            /// Used for the sorting field
            /// </summary>
            public const string Sort = "s";

            /// <summary>
            /// Used for the sorting field direction
            /// </summary>
            public const string SortDirection = "sd";

            /// <summary>
            /// Used for facets
            /// </summary>
            public const string Facets = "f";

            /// <summary>
            /// Used for separating facets
            /// </summary>
            public const char FacetsSeparator = '|';

            /// <summary>
            /// Used for the search keyword
            /// </summary>
            public const string SearchKeyword = "q";

            /// <summary>
            /// Used for page size
            /// </summary>
            public const string PageSize = "ps";

            /// <summary>
            /// Used for site content page size.
            /// </summary>
            public const string SiteContentPageSize = "scps";
        }

        /// <summary>
        /// Contains the names of index fields.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class KnownIndexFields
        {
            /// <summary>
            /// The name of the sitecontentitem index field.
            /// </summary>
            public static readonly string SiteContentItem = "sitecontentitem";

            /// <summary>
            /// The name of the instocklocations index field.
            /// </summary>
            public static readonly string InStockLocations = "instocklocations";

            /// <summary>
            /// The name of the outofstocklocations index field.
            /// </summary>
            public static readonly string OutOfStockLocations = "outofstocklocations";

            /// <summary>
            /// The name of the orderablelocations index field.
            /// </summary>
            public static readonly string OrderableLocations = "orderablelocations";

            /// <summary>
            /// The name of the preorderable index field.
            /// </summary>
            public static readonly string PreOrderable = "preorderable";
        }

        /// <summary>
        /// Contains the names of Sitecore item fields.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class ItemFields
        {
            /// <summary>
            /// The name of the DisplayInSearchResults field.
            /// </summary>
            public static readonly string DisplayInSearchResults = "DisplayInSearchResults";

            /// <summary>
            /// The name of the Title field.
            /// </summary>
            public static readonly string Title = "Title";

            /// <summary>
            /// The name of the SummaryText field.
            /// </summary>
            public static readonly string SummaryText = "SummaryText";
        }

        /// <summary>
        /// Contains style class names that are used in source code.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class StyleClasses
        {
            /// <summary>
            /// The class to change product search results page size.
            /// </summary>
            public static readonly string ChangePageSize = "changePageSize";

            /// <summary>
            /// The class to change site content search results page size.
            /// </summary>
            public static readonly string ChangeSiteContentPageSize = "changeSiteContentPageSize";
        }

        /// <summary>
        /// Used to hold some of the default settings for the site
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class CartConstants
        {
            /// <summary>
            /// Name of the Billing address
            /// </summary>
            public static readonly string BillingAddressName = "Billing";

            /// <summary>
            /// Name of the shipping address
            /// </summary>
            public static readonly string ShippingAddressName = "Shipping";
        }
    }
}