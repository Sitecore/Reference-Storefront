//-----------------------------------------------------------------------
// <copyright file="VisitorContext.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Defines the VisitorContext class.</summary>
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

namespace Sitecore.Reference.Storefront.Managers
{
    using Sitecore.Analytics;
    using Sitecore.Commerce.Contacts;
    using Sitecore.Commerce.Entities.Customers;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Exceptions;
    using Sitecore.Reference.Storefront.Util;
    using System;
    using System.Linq;

    /// <summary>
    /// Context object to host any collected or calculated visitor information so it can be passed to service calls
    /// </summary>
    public class VisitorContext
    {
        private const string StaticVisitorId = "{74E29FDC-8523-4C4F-B422-23BBFF0A342A}";
        private string _userId = string.Empty;      
        private CommerceUser _commerceUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitorContext"/> class.
        /// </summary>
        /// <param name="contactFactory">The contact factory.</param>
        public VisitorContext([NotNull] ContactFactory contactFactory)
        {
            Assert.ArgumentNotNull(contactFactory, "contactFactory");

            this.ContactFactory = contactFactory;
        }

        /// <summary>
        /// Gets or sets the contact factory.
        /// </summary>
        /// <value>
        /// The contact factory.
        /// </value>
        public ContactFactory ContactFactory { get; protected set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId
        {
            get
            {
                // Use the VisitorId if we have not set a UserId
                if (string.IsNullOrEmpty(this._userId))
                {
                    return this.VisitorId;
                }

                return this._userId;
            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get
            {
                return this.ContactFactory.GetContact();
            }
        }

        /// <summary>
        /// Gets the commerce user.
        /// </summary>
        /// <value>
        /// The commerce user.
        /// </value>
        public CommerceUser CommerceUser
        {
            get
            {
                return this._commerceUser;
            }
        }

        /// <summary>
        /// Gets the visitor id.
        /// </summary>
        /// <value>The visitor id.</value>
        public string VisitorId
        {
            get
            {
                if (Tracker.Current != null && Tracker.Current.Contact != null && Tracker.Current.Contact.ContactId != Guid.Empty)
                {
                    return Tracker.Current.Contact.ContactId.ToString();
                }

                // Generate our own tracking id if needed for the experience editor.
                if (Sitecore.Context.PageMode.IsExperienceEditor)
                {
                    return GetExperienceEditorVisitorTrackingId();
                }

                throw new TrackingNotEnabledException(StorefrontManager.GetSystemMessage(StorefrontConstants.SystemMessages.TrackingNotEnabled));
            }
        }

        /// <summary>
        /// Gets the shopping cart identifier.
        /// Used for AX7 only
        /// </summary>
        /// <value>
        /// The shopping cart identifier.
        /// </value>
        public string ShoppingCartId
        {
            get 
            {
                if (Context.User.IsAuthenticated)
                {
                    // cartId will be ignored for authenticated user                    
                    return Guid.NewGuid().ToString();
                }

                return CartCookieHelper.GetAnonymousCartIdFromCookie();
            }          
        }

        /// <summary>
        /// Gets the current customer Id
        /// </summary>
        /// <returns>the id</returns>
        public string GetCustomerId()
        {
            return this.UserId;
        }       

        /// <summary>
        /// Resolve the CommerceUser from the Visitor
        /// </summary>
        /// <param name="user">The user.</param>
        public void SetCommerceUser(CommerceUser user)
        {
            if (Tracker.Current == null || Tracker.Current.Contact == null || Tracker.Current.Contact.ContactId == Guid.Empty)
            {
                // This only occurs if we are authenticated but there is no ExternalUser assigned.
                // This happens in preview mode we want to supply the default user to use in Preview mode
                // Tracker.Visitor.ExternalUser = "1";
                return;
            }

            Assert.IsNotNull(this.ContactFactory, "this.ContactFactory should not be null.");

            this._commerceUser = user;

            Assert.IsNotNull(this._commerceUser.Customers, "The user '{0}' does not contain a Customers collection.", user.UserName);

            this._userId = this._commerceUser.Customers.FirstOrDefault();
        }

        /// <summary>
        /// Gets a visitor tracking ID when in the Experience Editor.
        /// </summary>
        /// <returns>the id of this visitor</returns>
        private static string GetExperienceEditorVisitorTrackingId()
        {
            return StaticVisitorId;
        }        
    }
}