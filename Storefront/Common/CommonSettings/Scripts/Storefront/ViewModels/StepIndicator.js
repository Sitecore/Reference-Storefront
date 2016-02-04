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

function setupStepIndicator() {
    $(document).ready(function () {
        $("#checkoutNavigation1").click(function (e) {
            switchingCheckoutStep("shipping");
            $("#checkoutNavigation1").parent().addClass("active");
            $("#checkoutNavigation2").parent().removeClass("active");
            $("#checkoutNavigation3").parent().removeClass("active");
        });
        $("#checkoutNavigation2").click(function (e) {
            if (!$("#ToBillingButton").prop("disabled")) {
                switchingCheckoutStep("billing");
                $("#checkoutNavigation2").parent().addClass("active");
                $("#checkoutNavigation1").parent().removeClass("active");
                $("#checkoutNavigation3").parent().removeClass("active");
            } else {
                $("#checkoutNavigation1").parent().addClass("active");
                $("#checkoutNavigation2").parent().removeClass("active");
                $("#checkoutNavigation3").parent().removeClass("active");
            }
        });
        $("#checkoutNavigation3").click(function (e) {
            if (!$("#ToBillingButton").prop("disabled")) {
                switchingCheckoutStep("confirm");
                $("#checkoutNavigation3").parent().addClass("active");
                $("#checkoutNavigation2").parent().removeClass("active");
                $("#checkoutNavigation1").parent().removeClass("active");
            } else {
                $("#checkoutNavigation1").parent().addClass("active");
                $("#checkoutNavigation2").parent().removeClass("active");
                $("#checkoutNavigation3").parent().removeClass("active");
            }
        });
    });
}