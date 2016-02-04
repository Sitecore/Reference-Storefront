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

function LoadMapInAccordian(element)
{
    var mapCanvas = element.find(".map-canvas");
    init(mapCanvas);
    setMyLocation();
}
function LoadMap()
{
    init($(".map-canvas"));
    setMyLocation();
}
function setMyLocation(){
    var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);

    geoLocationProvider.getCurrentPosition({successCallback:displayCenter, showAccuracyCircle:false});
}

var map = null;
var Credentials = "AoDuh-y-4c57psY4ebQAurV-wFTFCfphVB_5TdyjFKv-eBiyU_bnUcMrAPT0BE1k";

function init(element) {

    if (element.length == 0){
        return;
    }

    map = new Microsoft.Maps.Map(element[0],
        {
            credentials: Credentials,
            mapTypeId: Microsoft.Maps.MapTypeId.road,
            zoom: 17
    });


    // TODO: Der skal laves noget der henter de forskellige butikker ind og smider dem ind på kortet. lige nu er det hardcoded ;)

    map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(55.673684,12.568147), {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map'></span>", width: null, height: null, draggable: false }));
    map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(55.378239,10.373608), {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map'></span>", width: null, height: null, draggable: false }));
    map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(55.674651,12.55808), {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map'></span>", width: null, height: null, draggable: false }));
    map.entities.push(new Microsoft.Maps.Pushpin(new Microsoft.Maps.Location(55.176198,10.490216), {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map'></span>", width: null, height: null, draggable: false }));
}

function displayCenter(args)
{
    var center = args.center;
    var pin = new Microsoft.Maps.Pushpin(center, {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map home'></span>", width: 27, height: 31, draggable: false, anchor:new Microsoft.Maps.Point(12,28) });

    map.entities.push(pin);
}

function ClickGeocode()
{
    map.getCredentials(MakeGeocodeRequest);
}

function MakeGeocodeRequest()
{
    // TODO: Når kunden skriver en ny adresse ind skal den første PIN fjernes
    var geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations?query=" + encodeURI(document.getElementById('autocomplete').value) + "&output=json&jsonp=GeocodeCallback&key=" + Credentials;
    CallRestService(geocodeRequest);
}

function GeocodeCallback(result)
{
    //alert("Found location: " + result.resourceSets[0].resources[0].name);

    if (result &&
        result.resourceSets &&
        result.resourceSets.length > 0 &&
        result.resourceSets[0].resources &&
        result.resourceSets[0].resources.length > 0)
    {
        // Set the map view using the returned bounding box
        var bbox = result.resourceSets[0].resources[0].bbox;
        var viewBoundaries = Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(bbox[0], bbox[1]), new Microsoft.Maps.Location(bbox[2], bbox[3]));
        map.setView({ bounds: viewBoundaries});

        // Add a pushpin at the found location
        var location = new Microsoft.Maps.Location(result.resourceSets[0].resources[0].point.coordinates[0], result.resourceSets[0].resources[0].point.coordinates[1]);
        var pushpin = new Microsoft.Maps.Pushpin(location, {htmlContent: "<span class='glyphicon glyphicon-map-marker custom-map home'></span>", width: null, height: null, draggable: false });
        map.entities.push(pushpin);
    }
}

function CallRestService(request)
{
    var script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("src", request);
    document.body.appendChild(script);
}




