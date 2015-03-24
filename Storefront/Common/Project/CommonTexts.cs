//-----------------------------------------------------------------------
// <copyright file="CommonTexts.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines all of the localizable strings.</summary>
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
    using Sitecore.Globalization;

    /// <summary>
    /// Represents the texts in Sitecore.Reference.Storefront
    /// </summary>
    [LocalizationTexts(ModuleName = "Sitecore.Reference.Storefront")]
    public static class CommonTexts
    {
        /// <summary>
        /// "Creating and deploying enagagement plans." text
        /// </summary>
        public const string CreateDeployEngagementPlans = "Creating and deploying engagement plans.";

        /// <summary>
        /// "Done creating and deploying enagagement plans." text
        /// </summary>
        public const string CreateDeployEngagementPlansComplete = "Done creating and deploying engagement plans.";

        /// <summary>
        /// "Could not load mailtemplate:  from {0}" text
        /// </summary>
        public const string CouldNotLoadTemplateMessageError = "Could not load mailtemplate {0}";

        /// <summary>
        /// "Could not find subject for email {0}" text
        /// </summary>
        public const string CouldNotFindEmailSubjectMessageError = "Could not find subject for email {0}";

        /// <summary>
        /// "Could not find body for email {0}" text
        /// </summary>
        public const string CouldNotFindEmailBodyMessageError = "Could not find body for email {0}";

        /// <summary>
        /// "Could not find field  {0}" text
        /// </summary>
        public const string CouldNotFindFieldMessageError = "Could not find field  {0}";

        /// <summary>
        /// "'{0}' is not a valid email address. Check '{2}' field in mail template '{1}'" text
        /// </summary>
        public const string InvalidEmailAddressMessageError = "'{0}' is not a valid email address";

        /// <summary>
        /// Mail sent to {0} with subject: {1}" text
        /// </summary>
        public const string MailSentToMessage = "Mail sent to {0} with subject: {1}";

        /// <summary>
        /// "Could not send Mail: '{0}' To:{1}" text
        /// </summary>
        public const string CouldNotSendMailMessageError = "Could not send Mail: '{0}' To:{1}";
    }
}