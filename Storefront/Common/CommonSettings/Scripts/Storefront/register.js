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

$(document).ready(function () {
    $(".form-group input[name='SignupSelection']").click(function () {

        if ($('input:radio[name=SignupSelection]:checked').val() == "NewAccount") {
            $(".new-user").attr("disabled", false);
            $("#LinkupEmail").attr("disabled", true);            
        } else {
            $(".new-user").attr("disabled", true);
            $("#LinkupEmail").attr("disabled", false);           
        }
    });

    $(".form-group input[name='SignupSelection']:first").attr("checked", true).trigger("click");
});

function RegisterSuccess(data) {
    if (data && data.Success) {
        if (data.IsSignupFlow) {
             var url = new Uri(StorefrontUri("UserPendingActivation"))
                .addQueryParam("isSignupFlow", data.IsSignupFlow)
                .addQueryParam("email", data.UserName)
                .toString();
             window.location.href = url;
        }
        else {
            window.location.href = StorefrontUri("AccountManagement");
        }        
    }

    ClearGlobalMessages();
    ShowGlobalMessages(data);
    $("#registerButton").button('reset');
}

function RegisterFailure(data) {
    ShowGlobalMessages(data);
    $("#registerButton").button('reset');
}

function SetLoadingButton(cntx) {
    $(document).ready(function () {
        $("#registerButton").click(function (e) {
            if ($('input:radio[name=SignupSelection]:checked').val() == "NewAccount") {
                if ($("#UserName").val().length == 0) {
                    e.preventDefault();
                }
            }
            else if ($('input:radio[name=SignupSelection]:checked').val() == "LinkAccount") {
                if ($("#LinkupEmail").val().length == 0) {
                    e.preventDefault();
                }
            } else {
                e.preventDefault();
                throw "Sign up option not supported.";
            }
        })

        $("#registerButton").button('loading');
    });
}