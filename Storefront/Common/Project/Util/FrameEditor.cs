//-----------------------------------------------------------------------
// <copyright file="FrameEditor.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2015
// </copyright>
// <summary>Defines the EditorFrameHelper class.</summary>
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
    using System;
    using System.Web.Mvc;
    using System.Web.UI;

    using Sitecore.Web.UI.WebControls;
    
    /// <summary>
    /// Used to create integration of the editor frame into an MVC site
    /// </summary>
    public class FrameEditor : IDisposable
    {        
        private EditFrame _editFrameControl;
        private HtmlHelper _html;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEditor"/> class.
        /// </summary>
        /// <param name="html">The html helper managing the current request</param>
        /// <param name="dataSource">A path to the Sitecore item that the frame is for</param>
        /// <param name="buttons">A path to the edit frame buttons in the core database that need to be shown</param>
        public FrameEditor(HtmlHelper html, string dataSource = null, string buttons = null)
        {
            this._html = html;

            this._editFrameControl = new EditFrame
            {
                DataSource = dataSource ?? "/sitecore/content/home",
                Buttons = buttons ?? "/sitecore/content/Applications/WebEdit/Edit Frame Buttons/Default"
            };

            var output = new HtmlTextWriter(html.ViewContext.Writer);

            this._editFrameControl.RenderFirstPart(output);
        }

        /// <summary>
        /// Disposes of the current class appropriately
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._editFrameControl != null)
                {
                    this._editFrameControl.RenderLastPart(new HtmlTextWriter(this._html.ViewContext.Writer));
                    this._editFrameControl.Dispose();
                }
            }
        }
    }
}