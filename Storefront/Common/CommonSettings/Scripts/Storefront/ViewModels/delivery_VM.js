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

function StoreViewModel(store) {
    var self = this;

    var populate = store != null;

    self.address = populate ? ko.validatedObservable(new AddressViewModel(store.Address)) : ko.validatedObservable(new AddressViewModel());
    self.name = populate ? ko.validatedObservable(store.Name).extend({ required: true }) : ko.validatedObservable().extend({ required: true });
    self.externalId = populate ? ko.observable(store.ExternalId) : ko.observable();
    self.distance = populate ? ko.observable(store.Distance) : ko.observable();
    self.locationCount = populate ? ko.observable(store.LocationCount) : ko.observable();
    self.latitude = populate ? ko.observable(store.Latitude) : ko.observable();
    self.longitude = populate ? ko.observable(store.Longitude) : ko.observable();
}