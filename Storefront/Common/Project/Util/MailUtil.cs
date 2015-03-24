//-----------------------------------------------------------------------
// <copyright file="MailUtil.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the MailUtil class.</summary>
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

namespace Sitecore.Reference.Storefront.Util
{
    using System;
    using System.IO;
    using System.Net.Mail;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.StringExtensions;
    using System.Text.RegularExpressions;
    using Sitecore.Reference.Storefront.Managers;
    using Sitecore.Reference.Storefront.Models;
    using System.Globalization;

    /// <summary>
    /// Defines the MailUtil class
    /// </summary>
    public class MailUtil
    {
        private string _mailFrom = string.Empty;
        private string _mailTo = string.Empty;
        private string _mailBody = string.Empty;
        private string _mailSubject = string.Empty;
        private string _mailAttachmentFileName = string.Empty;

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mailTemplate">The mail template.</param>
        /// <returns>True if the email was sent, false otherwise</returns>
        public virtual bool SendMail([NotNull] MailTemplate mailTemplate)
        {
            Assert.ArgumentNotNull(mailTemplate, "mailTemplate");
            Assert.ArgumentNotNull(mailTemplate.ToEmail, "mailTemplate.To");
            Assert.ArgumentNotNull(mailTemplate.FromEmail, "mailTemplate.From");

            return this.SendMail(mailTemplate.ToEmail, mailTemplate.FromEmail, mailTemplate.Subject, mailTemplate.Body, string.Empty);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="toEmail">To email.</param>
        /// <param name="fromEmail">From email.</param>
        /// <param name="subjectParameters">The subject parameters.</param>
        /// <param name="bodyParameters">The body parameters.</param>
        /// <returns>True if the email was sent, false otherwise</returns>
        public virtual bool SendMail([NotNull] string templateName, [NotNull] string toEmail, [NotNull] string fromEmail, [NotNull] object subjectParameters, [NotNull] object[] bodyParameters)
        {
            Assert.ArgumentNotNull(templateName, "templateName");
            Assert.ArgumentNotNull(toEmail, "toEmail");
            Assert.ArgumentNotNull(fromEmail, "fromEmail");
            Assert.ArgumentNotNull(subjectParameters, "subjectParameters");
            Assert.ArgumentNotNull(bodyParameters, "bodyParameters");

            Item mailTemplates = StorefrontManager.CurrentStorefront.GlobalItem.Children[StorefrontConstants.KnowItemNames.Mails];
            if (mailTemplates == null)
            {
                return false;
            }

            Item mailTemplate = mailTemplates.Children[templateName];
            if (mailTemplate == null)
            {
                Log.Error(Translate.Text(string.Format(CultureInfo.InvariantCulture, CommonTexts.CouldNotLoadTemplateMessageError, templateName)), this);
                return false;
            }

            var subjectField = mailTemplate.Fields[StorefrontConstants.KnownFieldNames.Subject];
            if (subjectField == null)
            {
                Log.Error(Translate.Text(string.Format(CultureInfo.InvariantCulture, CommonTexts.CouldNotFindEmailSubjectMessageError, templateName)), this);
                return false;
            }

            var bodyField = mailTemplate.Fields[StorefrontConstants.KnownFieldNames.Body];
            if (bodyField == null)
            {
                Log.Error(Translate.Text(string.Format(CultureInfo.InvariantCulture, CommonTexts.CouldNotFindEmailBodyMessageError, templateName)), this);
                return false;
            }

            var subject = string.Format(CultureInfo.InvariantCulture, subjectField.ToString(), subjectParameters);
            var body = string.Format(CultureInfo.InvariantCulture, bodyField.ToString(), bodyParameters);
           
            return this.SendMail(toEmail, fromEmail, subject, body, string.Empty);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="fromEmail">From email.</param>
        /// <param name="subject">The mail subject.</param>
        /// <param name="body">The mail body.</param>
        /// <param name="attachmentFileName">Name of the attachment file.</param>
        /// <returns>True if the email was sent, false otherwise</returns>
        public virtual bool SendMail([NotNull] string toEmail, [NotNull] string fromEmail, [NotNull] string subject, [NotNull] string body, [NotNull] string attachmentFileName)
        {
            Assert.ArgumentNotNull(toEmail, "toEmail");
            Assert.ArgumentNotNull(fromEmail, "fromEmail");
            Assert.ArgumentNotNull(subject, "subject");
            Assert.ArgumentNotNull(body, "body");
            Assert.ArgumentNotNull(attachmentFileName, "attachmentFileName");

            this._mailTo = toEmail;
            this._mailFrom = fromEmail;
            this._mailBody = body;
            this._mailAttachmentFileName = attachmentFileName;
            this._mailSubject = subject;

            return this.SendMail();
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <returns>True if the email was sent, false otherwise</returns>
        protected virtual bool SendMail()
        {
            MailMessage message = new MailMessage
                                    {
                                        From = new MailAddress(this._mailFrom),
                                        Body = this._mailBody,
                                        Subject = this._mailSubject,
                                        IsBodyHtml = true
                                    };
            message.To.Add(this._mailTo);

            if (this._mailAttachmentFileName != null && File.Exists(this._mailAttachmentFileName))
            {
                Attachment attachment = new Attachment(this._mailAttachmentFileName);
                message.Attachments.Add(attachment);
            }

            try
            {
                MainUtil.SendMail(message);
                Log.Info(Translate.Text(string.Format(CultureInfo.InvariantCulture, CommonTexts.MailSentToMessage, message.To, message.Subject)), "SendMailFromTemplate");
                return true;
            }
            catch (Exception e)
            {
                Log.Error(Translate.Text(string.Format(CultureInfo.InvariantCulture, CommonTexts.CouldNotSendMailMessageError, message.Subject, message.To)), e, "SendMailFromTemplate");
                return false;
            }
        }
    }
}
