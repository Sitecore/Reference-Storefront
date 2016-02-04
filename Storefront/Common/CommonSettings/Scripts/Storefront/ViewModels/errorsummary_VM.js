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

var errorSummaryViewModel = null;

function ErrorLineDetail(errorMessage) {
    var self = this;

    self.errorMessage = errorMessage;
}

function ErrorSummaryViewModel(sectionId) {
    var self = this;

    self.sectionId = sectionId;
    self.errorList = ko.observableArray();
    self.shouldShowErrorList = ko.observable(false);

    self.AddToErrorList = function (data) {
        $(data.Errors).each(function () {
            self.errorList.push(new ErrorLineDetail(this));
        });

        self.shouldShowErrorList(self.errorList && self.errorList().length > 0 ? true : false);
    }

    self.ClearMessages = function () {
        self.errorList.removeAll();
        self.shouldShowErrorList(false);
    }
}

function initErrorSummary(sectionId) {
    errorSummaryViewModel = new ErrorSummaryViewModel(sectionId);
    ko.applyBindings(errorSummaryViewModel, document.getElementById(sectionId));
}