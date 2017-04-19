//-----------------------------------------------------------------------
// <copyright file="FederatedPaymentModelBinder.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>The FederatedPayment custom ModelBinder.</summary>
//-----------------------------------------------------------------------
// Copyright 2017 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------------------------------------------------------------------------------------------

namespace Sitecore.Reference.Storefront.Binders
{
    using System;
    using System.Web.Mvc;
    using Models.InputModels;

    /// <summary>
    /// The Binder for the FederatedPaymentInputModelItem
    /// </summary>
    /// <seealso cref="System.Web.Mvc.IModelBinder" />
    internal class FederatedPaymentModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>
        /// The bound value.
        /// </returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var actualModel = new FederatedPaymentInputModelItem();

            foreach (var prop in actualModel.GetType().GetProperties())
            {
                var modelName = string.Concat(bindingContext.ModelName, '.', prop.Name);
                ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(modelName);

                if (valueResult == null)
                {
                    continue;
                }

                ModelState modelState = new ModelState { Value = valueResult };
                
                if (prop.PropertyType.Equals(typeof(decimal)))
                {
                    try
                    {
                        prop.SetValue(actualModel, Convert.ToDecimal(valueResult.AttemptedValue, Sitecore.Context.Language.CultureInfo), null);
                    }
                    catch (FormatException e)
                    {
                        modelState.Errors.Add(e);
                    }
                }
                else
                {
                    prop.SetValue(actualModel, valueResult.RawValue, null);
                }

                bindingContext.ModelState.Add(modelName, modelState);
            }         

            return actualModel;
        }
    }
}
