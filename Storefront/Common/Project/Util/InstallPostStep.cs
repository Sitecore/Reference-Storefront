//-----------------------------------------------------------------------
// <copyright file="InstallPostStep.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the InstallPostStep class.</summary>
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
    using System.Collections.Specialized;
    using Sitecore.Install.Framework;
    using System.Collections.Generic;
    using System.Globalization;
    using Sitecore.Analytics;
    using Sitecore.Commerce.Connect.CommerceServer;
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Globalization;
    using Sitecore.Data.Items;

    /// <summary>
    /// A class used for post step work after installing packages.  
    /// </summary>
    public class InstallPostStep : IPostStep
    {
        /// <summary>
        /// The default localized content folder which will be found in the website\Temp folder
        /// </summary>
        private const string DefaultLocalizationFolder = "Storefront";

        /// <summary>
        /// Format string for creating the enagagement plan name
        /// </summary>
        private const string EaPlanFormatString = "{0} {1}";

        /// <summary>
        /// Engagement plans to create
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Not hungarian")]
        private readonly List<EaPlanInfo> _eaPlanInfos = new List<EaPlanInfo>
        {
            new EaPlanInfo
            {
                Name = StorefrontConstants.EngagementPlans.AbandonedCartsEaPlanName, 
                ItemId = StorefrontConstants.EngagementPlans.AbandonedCartsEaPlanId, 
                EaPlanId = StorefrontConstants.KnownItemIds.AbandonedCartsEaPlanBranchTemplateId
            },
            new EaPlanInfo
            {
                Name = StorefrontConstants.EngagementPlans.NewOrderPlacedEaPlanName, 
                ItemId = StorefrontConstants.EngagementPlans.NewOrderPlacedEaPlanId, 
                EaPlanId = StorefrontConstants.KnownItemIds.NewOrderPlacedEaPlanBranchTemplateId
            },
            new EaPlanInfo
            {
                Name = StorefrontConstants.EngagementPlans.ProductsBackInStockEaPlanName, 
                ItemId = StorefrontConstants.EngagementPlans.ProductsBackInStockEaPlanId, 
                EaPlanId = StorefrontConstants.KnownItemIds.ProductsBackInStockEaPlanBranchTemplateId
            }
        };

        /// <summary>
        /// Runs the specified output.
        /// </summary>
        /// <param name="output">the output</param>
        /// <param name="metadata">the metadata</param>
        public void Run(ITaskOutput output, NameValueCollection metadata)
        {
            var postStep = new Sitecore.Commerce.Connect.CommerceServer.InstallPostStep(DefaultLocalizationFolder);

            postStep.OutputMessage(Translate.Text(CommonTexts.CreateDeployEngagementPlans));
            this.CreateEaPlans();
            this.DeployEaPlans();
            postStep.OutputMessage(Translate.Text(CommonTexts.CreateDeployEngagementPlansComplete));

            postStep.Run(output, metadata);
        }

        /// <summary>
        /// Creates the Engagement Plans
        /// </summary>
        protected virtual void CreateEaPlans()
        {
            var database = Context.ContentDatabase ?? Database.GetDatabase("master");

            foreach (var plan in this._eaPlanInfos)
            {
                var item = database.GetItem(ID.Parse(plan.EaPlanId));

                if (item != null)
                {
                    plan.Name = string.Format(CultureInfo.InvariantCulture, EaPlanFormatString, StorefrontConstants.Settings.WebsiteName, item.DisplayName);
                    var result = ItemManager.AddFromTemplate(plan.Name, ID.Parse(plan.EaPlanId), database.GetItem(StorefrontConstants.KnownItemIds.CommerceConnectEaPlanParentId), ID.Parse(plan.ItemId));
                    continue;
                }

                CommerceLog.Current.Error(string.Format(CultureInfo.InvariantCulture, "Error creating engagement plan '{0}'.", plan.Name), this);
            }
        }

        /// <summary>
        /// Deploys the engagement plans
        /// </summary>
        protected virtual void DeployEaPlans()
        {
            foreach (var planInfo in this._eaPlanInfos)
            {
                var engagementPlanItem = Tracker.DefinitionItems.EngagementPlans[planInfo.Name];

                if (engagementPlanItem.IsDeployed)
                {
                    continue;
                }

                var result = ((Item)engagementPlanItem).State.GetWorkflow().Execute(StorefrontConstants.KnownItemIds.DeployCommandId, engagementPlanItem, string.Empty, false).Succeeded;

                if (!result)
                {
                    CommerceLog.Current.Error(string.Format(CultureInfo.InvariantCulture, "Error deploying engagement plan '{0}'.", planInfo.Name), this);
                }
            }
        }
    }
}