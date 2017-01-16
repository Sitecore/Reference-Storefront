//-----------------------------------------------------------------------
// <copyright file="StorefrontConstants.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Storefront constant definition.</summary>
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
        /// ItemTypes enumerator.
        /// </summary>
        public enum ItemTypes
        {
            /// <summary>
            /// Unknown ItemType or null.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Category ItemType.
            /// </summary>
            Category,

            /// <summary>
            /// NamedSearch Itemtype.
            /// </summary>
            NamedSearch,

            /// <summary>
            /// Product ItemType.
            /// </summary>
            Product,

            /// <summary>
            /// Secured page ItemType.
            /// </summary>
            SecuredPage,

            /// <summary>
            /// The SelectedProducts ItemType.
            /// </summary>
            SelectedProducts,

            /// <summary>
            /// Standard page ItemType.
            /// </summary>
            StandardPage,

            /// <summary>
            /// Variant ItemType.
            /// </summary>
            Variant
        }

        /// <summary>
        /// Storefront views.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class Views
        {
            /// <summary>
            /// The empty view.
            /// </summary>
            public static readonly string Empty = "/Shared/Empty";

            /// <summary>
            /// The addresses
            /// </summary>
            public static readonly string Addresses = "Addresses";

            /// <summary>
            /// The edit profile
            /// </summary>
            public static readonly string EditProfile = "EditProfile";

            /// <summary>
            /// The register
            /// </summary>
            public static readonly string Register = "Register";

            /// <summary>
            /// The user pending activation
            /// </summary>
            public static readonly string UserPendingActivation = "UserPendingActivation";
        }

        /// <summary>
        /// Defines the Action names constants
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownActionNames
        {
            /// <summary>
            /// The account link up pending action name
            /// </summary>
            public static readonly string AccountLinkupPendingActionName = "AccountLinkupPending";

            /// <summary>
            /// The register action name
            /// </summary>
            public static readonly string RegisterActionName = "Register";

            /// <summary>
            /// The login action name
            /// </summary>
            public static readonly string LoginActionName = "Login";
        }

        /// <summary>
        /// Defines the System Message constants.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class SystemMessages
        {
            /// <summary>
            /// Authentication provider error message.
            /// </summary>
            public static readonly string AuthenticationProviderError = "AuthenticationProviderError";

            /// <summary>
            /// Cart not found error message.
            /// </summary>
            public static readonly string CartNotFoundError = "CartNotFoundError";

            /// <summary>
            /// Could not create user message.
            /// </summary>
            public static readonly string CouldNotCreateUser = "CouldNotCreatedUser";
            
            /// <summary>
            /// Could not find email body message error message.
            /// </summary>
            public static readonly string CouldNotFindEmailBodyMessageError = "CouldNotFindEmailBodyMessageError";
            
            /// <summary>
            /// Could not find email subject message error message.
            /// </summary>
            public static readonly string CouldNotFindEmailSubjectMessageError = "CouldNotFindEmailSubjectMessageError";
            
            /// <summary>
            /// Could not load template message error message.
            /// </summary>
            public static readonly string CouldNotLoadTemplateMessageError = "CouldNotLoadTemplateMessageError";
            
            /// <summary>
            /// Could not send mail message error message.
            /// </summary>
            public static readonly string CouldNotSendMailMessageError = "CouldNotSendMailMessageError";
            
            /// <summary>
            /// Could not sent email error message.
            /// </summary>
            public static readonly string CouldNotSentEmailError = "CouldNotSentEmailError";
            
            /// <summary>
            /// Invalid email error message.
            /// </summary>
            public static readonly string InvalidEmailError = "InvalidEmailError";
            
            /// <summary>
            /// Invalid password error message.
            /// </summary>
            public static readonly string InvalidPasswordError = "InvalidPasswordError";
            
            /// <summary>
            /// Mail sent to message message.
            /// </summary>
            public static readonly string MailSentToMessage = "MailSentToMessage";
            
            /// <summary>
            /// Maximum addresse limit reached message.
            /// </summary>
            public static readonly string MaxAddressLimitReached = "MaxAddresseLimitReached";
            
            /// <summary>
            /// Maximum loyalty programs to join reached message.
            /// </summary>
            public static readonly string MaxLoyaltyProgramsToJoinReached = "MaxLoyaltyProgramsToJoinReached";
            
            /// <summary>
            /// Maximum wish list line limit reached message.
            /// </summary>
            public static readonly string MaxWishListLineLimitReached = "MaxWishListLineLimitReached";
            
            /// <summary>
            /// Maximum wish list limit reached message.
            /// </summary>
            public static readonly string MaxWishListLimitReached = "MaxWishListLimitReached";
            
            /// <summary>
            /// Password could not be reset message.
            /// </summary>
            public static readonly string PasswordCouldNotBeReset = "PasswordCouldNotBeReset";
            
            /// <summary>
            /// Password retrieval answer invalid message.
            /// </summary>
            public static readonly string PasswordRetrievalAnswerInvalid = "PasswordRetrievalAnswerInvalid";
            
            /// <summary>
            /// Password retrieval question invalid message.
            /// </summary>
            public static readonly string PasswordRetrievalQuestionInvalid = "PasswordRetrievalQuestionInvalid";
            
            /// <summary>
            /// Submit order has empty cart message.
            /// </summary>
            public static readonly string SubmitOrderHasEmptyCart = "SubmitOrderHasEmptyCart";
            
            /// <summary>
            /// Tracking not enabled message.
            /// </summary>
            public static readonly string TrackingNotEnabled = "TrackingNotEnabled";
            
            /// <summary>
            /// Unknown membership provider error message.
            /// </summary>
            public static readonly string UnknownMembershipProviderError = "UnknownMembershipProviderError";
            
            /// <summary>
            /// Update user profile error message.
            /// </summary>
            public static readonly string UpdateUserProfileError = "UpdateUserProfileError";
            
            /// <summary>
            /// User already exists message.
            /// </summary>
            public static readonly string UserAlreadyExists = "UserAlreadyExists";
            
            /// <summary>
            /// User name for email exists message.
            /// </summary>
            public static readonly string UserNameForEmailExists = "UserNameForEmailExists";
            
            /// <summary>
            /// User name invalid message.
            /// </summary>
            public static readonly string UserNameInvalid = "UserNameInvalid";
            
            /// <summary>
            /// User not found error message.
            /// </summary>
            public static readonly string UserNotFoundError = "UserNotFoundError";
            
            /// <summary>
            /// User rejected error message.
            /// </summary>
            public static readonly string UserRejectedError = "UserRejectedError";

            /// <summary>
            /// The default currency not set exception message.
            /// </summary>
            public static readonly string DefaultCurrencyNotSetException = "DefaultCurrencyNotSetException";

            /// <summary>
            /// The invalid currency error message.
            /// </summary>
            public static readonly string InvalidCurrencyError = "InvalidCurrencyError";

            /// <summary>
            /// The login failed error message.
            /// </summary>
            public static readonly string LoginFailed = "LoginFailed";

            /// <summary>
            /// The authorization code missing error message.
            /// </summary>
            public static readonly string AuthorizationCodeMissing = "AuthorizationCodeMissing";

            /// <summary>
            /// The cancel pending request message.
            /// </summary>
            public static readonly string CancelPendingRequest = "CancelPendingRequest";

            /// <summary>
            /// The account not found error message.
            /// </summary>
            public static readonly string AccountNotFound = "AccountNotFound";

            /// <summary>
            /// The activation code sent message.
            /// </summary>
            public static readonly string ActivationCodeSent = "ActivationCodeSent";

            /// <summary>
            /// The wrong activation code error message.
            /// </summary>
            public static readonly string WrongActivationCode = "WrongActivationCode";

            /// <summary>
            /// The linkup succeeded message.
            /// </summary>
            public static readonly string LinkupSucceeded = "LinkupSucceeded";

            /// <summary>
            /// The card authorization failed message.
            /// </summary>
            public static readonly string CardAuthorizationFailed = "CardAuthorizationFailed";
        }
        
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

            /// <summary>
            /// The default currency to be applied.  This is temporary until the multi-currency support is integrated in all facets of the sytem.
            /// </summary>
            public static readonly string DefaultCurrencyCode = "USD";
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
            /// The method id field.
            /// </summary>
            public static readonly string MethodId = "Method ID";

            /// <summary>
            /// The storefront configuration field.
            /// </summary>
            public static readonly string StorefrontConfiguration = "Storefront Configuration";

            /// <summary>
            /// The cancel field.
            /// </summary>
            public static readonly string Cancel = "Cancel";

            /// <summary>
            /// The create user field.
            /// </summary>
            public static readonly string CreateUser = "Create user";

            /// <summary>
            /// The activate user
            /// </summary>
            public static readonly string ActivateUser = "Activate user";

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
            /// The first name missing message
            /// </summary>
            public static readonly string FirstNameMissingMessage = "First Name Missing Message";

            /// <summary>
            /// The last name missing message
            /// </summary>
            public static readonly string LastNameMissingMessage = "Last Name Missing Message";

            /// <summary>
            /// The activation code missing message
            /// </summary>
            public static readonly string ActivationCodeMissingMessage = "Activation Code Missing Message";

            /// <summary>
            /// The resend activation code message
            /// </summary>
            public static readonly string ResendActivationCodeMessage = "Resend Activation Code Message";

            /// <summary>
            /// The activation code
            /// </summary>
            public static readonly string ActivationCode = "Activation Code";

            /// <summary>
            /// The facebook button field.
            /// </summary>
            public static readonly string FacebookButton = "Facebook Button";

            /// <summary>
            /// The facebook text field.
            /// </summary>
            public static readonly string FacebookText = "Facebook Text";

            /// <summary>
            /// The activation text
            /// </summary>
            public static readonly string ActivationText = "Activation Text";

            /// <summary>
            /// The activate text
            /// </summary>
            public static readonly string ActivateText = "Activate Text";

            /// <summary>
            /// The link account text
            /// </summary>
            public static readonly string LinkAccount = "Link Account";

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
            /// The generate secure link field.
            /// </summary>
            public static readonly string GenerateSecureLink = "Generate Secure Link";

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
            /// The activating
            /// </summary>
            public static readonly string Activating = "Activating";

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

            /// <summary>
            /// The maximum number of addresses field.
            /// </summary>
            public static readonly string MaxNumberOfAddresses = "Max Number of Addresses";

            /// <summary>
            /// The maximum number of wishlists field.
            /// </summary>
            public static readonly string MaxNumberOfWishLists = "Max Number of WishLists";

            /// <summary>
            /// The maxnumber of wish list items field.
            /// </summary>
            public static readonly string MaxNumberOfWishListItems = "Max Number of WishList Items";

            /// <summary>
            /// The "Use index file for product status in lists" field.
            /// </summary>
            public static readonly string UseIndexFileForProductStatusInLists = "Use Index File For Product Status In Lists";

            /// <summary>
            /// The form authentication
            /// </summary>
            public static readonly string FormsAuthentication = "Forms Authentication";

            /// <summary>
            /// The operating unit number
            /// </summary>
            public static readonly string OperatingUnitNumber = "OperatingUnitNumber";

            /// <summary>
            /// The map key field.
            /// </summary>
            public static readonly string MapKey = "Map Key";

            /// <summary>
            /// The named searches field.
            /// </summary>
            public static readonly string NamedSearches = "Named Searches";

            /// <summary>
            /// The title field.
            /// </summary>
            public static readonly string Title = "Title";

            /// <summary>
            /// The product list field.
            /// </summary>
            public static readonly string ProductList = "Product List";

            /// <summary>
            /// The currency description field.
            /// </summary>
            public static readonly string CurrencyDescription = "Currency Description";

            /// <summary>
            /// The currency symbol field.
            /// </summary>
            public static readonly string CurrencySymbol = "Currency Symbol";

            /// <summary>
            /// The currency symbol field.
            /// </summary>
            public static readonly string CurrencySymbolPosition = "Currency Symbol Position";

            /// <summary>
            /// The currency number format culture field.
            /// </summary>
            public static readonly string CurrencyNumberFormatCulture = "Currency Number Format Culture";

            /// <summary>
            /// The supported currencies field.
            /// </summary>
            public static readonly string SupportedCurrencies = "Supported Currencies";

            /// <summary>
            /// The default currency field.
            /// </summary>
            public static readonly string DefaultCurrency = "Default Currency";

            /// <summary>
            /// The new contoso account
            /// </summary>
            public static readonly string NewContosoAccount = "New Contoso Account";

            /// <summary>
            /// The link contoso account
            /// </summary>
            public static readonly string LinkContosoAccount = "Link Contoso Account";

            /// <summary>
            /// The email address of existing customer
            /// </summary>
            public static readonly string EmailAddressOfExistingCustomer = "Email Address Of Existing Customer";

            /// <summary>
            /// The email of existing customer
            /// </summary>
            public static readonly string EmailOfExistingCustomer = "Email Of Existing Customer";

            /// <summary>
            /// The enter email for account association
            /// </summary>
            public static readonly string EnterEmailForAccountAssociation = "EnterEmailForAccountAssociation";

            /// <summary>
            /// The continue shopping text
            /// </summary>
            public static readonly string ContinueShoppingText = "Continue Shopping Text";

            /// <summary>
            /// The sign out text
            /// </summary>
            public static readonly string SignOutText = "Sign Out Text";

            /// <summary>
            /// The disclaimer text
            /// </summary>
            public static readonly string DisclaimerText = "Disclaimer Text";
        }

        /// <summary>
        /// Known template names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class KnownTemplateNames
        {
            /// <summary>
            /// The commerce named search template name.
            /// </summary>
            public static readonly string CommerceNamedSearch = "Commerce Named Search";

            /// <summary>
            /// The named search template name.
            /// </summary>
            public static readonly string NamedSearch = "Named Search";
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
            /// The ID of the Named Search template.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID NamedSearch = new ID("{F3C0CD6C-9FA9-442D-BD3A-5A25E292F2F7}");

            /// <summary>
            /// The ID of the Standard Page template.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID StandardPage = new ID("{16E859D2-6542-407A-AC65-F34BCAD3EB3D}");

            /// <summary>
            /// The ID of the Secured Page template.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID SecuredPage = new ID("{02CCCF95-7BE5-4549-81F9-AC97A22D6816}");

            /// <summary>
            /// The ID of the Selected Products template.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
            public static readonly ID SelectedProducts = new ID("{A45D0030-79F2-4DBF-9A74-226A33C58249}");
        }

        /// <summary>
        /// Known template names.
        /// </summary>
        public static class KnowItemNames
        {
            public static readonly string CurrencyConfiguration = "Currency Configuration";
            public static readonly string CurrencyDisplayAdjustments = "Currencies Display Adjustments";
            public static readonly string FulfillmentConfiguration = "Fulfillment Configuration";
            public static readonly string PaymentConfiguration = "Payment Configuration";

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

            /// <summary>
            /// The relationships template name.
            /// </summary>
            public static readonly string Relationships = "Relationships";

            /// <summary>
            /// The order statuses template name.
            /// </summary>
            public static readonly string OrderStatuses = "Order Statuses";

            /// <summary>
            /// The currencies lookup folder.
            /// </summary>
            public static readonly string Currencies = "Currencies";

            /// <summary>
            /// The payments folder.
            /// </summary>
            public static readonly string Payments = "Payments";

            /// <summary>
            /// The shipping folder.
            /// </summary>
            public static readonly string Shipping = "Shipping";
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
            /// The shop name page event data.
            /// </summary>
            public static readonly string ShopName = "ShopName";

            /// <summary>
            /// The currency page event data.
            /// </summary>
            public static readonly string Currency = "Currency";
        }

        /// <summary>
        /// Contains constants that represent Sitecore pipeline names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required for access purposes")]
        public static class PipelineNames
        {
            /// <summary>
            /// The name of the SearchInitiated pipeline.
            /// </summary>
            public const string SearchInitiated = Prefix + "searchInitiated";

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
            /// User for the order confirmation id.
            /// </summary>
            public const string ConfirmationId = "confirmationId";

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

            /// <summary>
            /// The child categories sequence index field.
            /// </summary>
            public static readonly string ChildCategoriesSequence = "childcategoriessequence";
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