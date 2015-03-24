//-----------------------------------------------------------------------
// <copyright file="CSBaseController.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the Commerce Server base controller.</summary>
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

namespace Sitecore.Reference.Storefront.Controllers
{
    using Sitecore.Commerce.Contacts;
    using Sitecore.Diagnostics;
    using Sitecore.Reference.Storefront.Managers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Defines the CSBaseController class.
    /// </summary>
    public class CSBaseController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSBaseController"/> class.
        /// </summary>
        /// <param name="accountManager">The account manager.</param>
        /// <param name="contactFactory">The contact factory.</param>
        public CSBaseController([NotNull] AccountManager accountManager, [NotNull] ContactFactory contactFactory)
            : base(contactFactory)
        {
            Assert.ArgumentNotNull(accountManager, "accountManager");

            this.AccountManager = accountManager;
        }

        /// <summary>
        /// Gets or sets the account manager.
        /// </summary>
        /// <value>
        /// The account manager.
        /// </value>
        public AccountManager AccountManager { get; set; }

        /// <summary>
        /// Gets the Current Visitor Context
        /// </summary>
        public override VisitorContext CurrentVisitorContext
        {
            get
            {
                // Setup the visitor context only once per HttpRequest.
                var siteContext = this.CurrentSiteContext;
                VisitorContext visitorContext = siteContext.Items["__visitorContext"] as VisitorContext;
                if (visitorContext == null)
                {
                    visitorContext = new VisitorContext(this.ContactFactory);
                    if (Context.User.IsAuthenticated && !Context.User.Profile.IsAdministrator)
                    {
                        visitorContext.SetCommerceUser(this.AccountManager.ResolveCommerceUser().Result);
                    }

                    siteContext.Items["__visitorContext"] = visitorContext;
                }

                return visitorContext;
            }
        }
    }
}