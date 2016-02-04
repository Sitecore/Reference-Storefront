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

function SetSavingProfileButton(cntx) {
    $(document).ready(function () {
        $("#saveProfileChangesButton").button('loading');
    });
}

function UpdateProfileSuccess(cntx) {
    ClearGlobalMessages();
    $("#saveProfileChangesButton").button('reset');

    if (cntx.Errors.length === 0) {
        $("#editProfileFail").show().fadeOut(4000);
        window.location.href = StorefrontUri("AccountManagement");
    }
    else {
        ShowGlobalMessages(cntx);
    }
}

function UpdateProfileFailure(cntx) {
    ClearGlobalMessages();
    ShowGlobalMessages(cntx);
    $("#saveProfileChangesButton").button('reset');
}