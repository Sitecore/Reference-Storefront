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

function ActivationSuccess(data) {
    if (data && data.Success) {
        var url = new Uri(StorefrontUri("Login"))
              .addQueryParam("existingAccount", data.Data.ActivatedEmail)
              .addQueryParam("externalIdProvider", data.Data.IdProvider)
              .toString();
        window.location.href = url;
    }

    ClearGlobalMessages();
    ShowGlobalMessages(data);
    $("#activationButton").button('reset');
}

function ActivationFailure(data) {
    ShowGlobalMessages(data);
    $("#activationButton").button('reset');
}

function SetActivatingButton(cntx) {
    $(document).ready(function () {
        if ($("#EmailAddressOfExistingCustomer").val().length == 0 || $("#ActivationCode").val().length == 0) {
            e.preventDefault();
        }
        $("#activationButton").button('loading');
    });
}