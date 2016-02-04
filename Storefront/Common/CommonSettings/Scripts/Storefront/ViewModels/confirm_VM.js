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

var confirmViemModel = null;

function ConfirmViewModel(data) {

    var items = ko.observableArray();

    $.map(data, function (index, value) {

        var currentId = value.externalCartlineId;
        if (items.indexOf(currentId)) {
            items.push([currentId,value]);
        }
       
    })

   
}

function composedItem(image, displayName, color, delivery, lineprice, quantity, linetotal, externalCartlineId) {

    this.image = image;
    this.displayName = displayName;
    this.color = color;
    this.delivery = delivery;
    this.price = lineprice;
    this.quantity = quantity;
    this.total = linetotal;
    this.id = externalCartlineId;

}