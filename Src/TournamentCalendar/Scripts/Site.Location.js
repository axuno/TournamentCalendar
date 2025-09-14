// All Site scripts go into the same namespace
var Site;  // NOSONAR
if (Site === undefined) {
    Site = {};
}

/* global google */

Site.Location = class {
    'use strict';

    /**
    * Represents a Location.
    * @constructor
    * @param {string} countryId - Id of the Country input element.
    * @param {string} zipCodeId - Id of the ZIP code input element.
    * @param {string} cityId - Id of the City input element.
    * @param {string} streetId - Id of the Street input element.
    * @param {string} btnMapModalId - Id of the button for showing the map as modal.
    * @param {string} enterAddrToggleId - Id of the checkbox to enable/disable the address section (see below).
    * @param {string} addressSectionId - Id of the section element which has input elements as children.
    * @param {string} mapPlaceholderId - Id of the element which will be filled with the map by google.maps
    * @param {string} mapsLib - The Google Maps library
    * @param {string} markerLib - The Extended Marker library
    * @param {string} geocoderLib - The Geocoder library
    */
    constructor(countryId, zipCodeId, cityId, streetId, btnMapModalId, mapId, mapPlaceholderId, mapsLib, markerLib, geocoderLib) {
        this.countryIdEle = document.getElementById(countryId);
        this.countryIdEle.addEventListener('change', this.update.bind(this), false);
        this.zipCodeEle = document.getElementById(zipCodeId);
        this.zipCodeEle.addEventListener('change', this.update.bind(this), false);
        this.zipCodeEle.addEventListener('change', function (e) { this.value = this.value.trim(); });
        this.cityEle = document.getElementById(cityId);
        this.cityEle.addEventListener('change', this.update.bind(this), false);
        this.cityEle.addEventListener('change', function (e) { this.value = this.value.trim(); });
        this.streetEle = document.getElementById(streetId);
        this.streetEle.addEventListener('change', this.update.bind(this), false);
        this.streetEle.addEventListener('change', function (e) { this.value = this.value.trim(); });
        this.btnMapModalEle = document.getElementById(btnMapModalId);
        this.mapId = mapId, this.mapPlaceholderEle = document.getElementById(mapPlaceholderId);
        this.MapsLib = mapsLib;
        this.MarkerLib = markerLib;
        this.GeocoderLib = geocoderLib;
        setInterval(this.enableDisableModalButton.bind(this), 500);
    }

    get addressElements() {
        return [this.countryIdEle, this.zipCodeEle, this.cityEle, this.streetEle];
    }

    enableDisableModalButton()  {
        if (this.isAddressEntered) {
            this.btnMapModalEle.removeAttribute('disabled');
            this.btnMapModalEle.style.cursor = 'pointer';
			this.btnMapModalEle.style.pointerEvents = 'auto';
        } else {
            this.btnMapModalEle.setAttribute('disabled', 'disabled');
            this.btnMapModalEle.style.cursor = 'not-allowed';
			this.btnMapModalEle.style.pointerEvents = 'all'; /* Needed to show the disabled cursor with BS5 - see https://github.com/twbs/bootstrap/issues/16088 */
        }
    }

    update() {
        this.enableDisableModalButton();
        if (this.isAddressEntered) {
            this.showHideLocation();
        }
    }

    get isAddressEntered() {
        return this.getAddress(false).length >= 3;
    }

    getAddress(withCountryId) {

        const address = (this.zipCodeEle.value.trim() + ' ' + this.cityEle.value.trim() + ' ' + this.streetEle.value.trim()).trim();
        if (withCountryId)
            return (this.countryIdEle.value.trim() + ' ' + address).trim();
        else
            return address;
    }

    arrayEquals(a, b) {
        return Array.isArray(a) &&
            Array.isArray(b) &&
            a.length === b.length &&
            a.every((val, index) => val === b[index]);
    }
    
    showHideLocation() {
        this.mapPlaceholderEle.innerHTML = '<div style="padding: 1rem; color: gray">Die Karte wird gezeigt,<br />wenn die Adresse gefunden wurde.</div>';

        if (this.isAddressEntered) {
            this.getLocation()
                .then(geoCoderResult => {
                    // Check if the promise returned a result.
                    if (geoCoderResult) {
                        this.showLocationOnGoogleMaps(geoCoderResult[0], geoCoderResult[1]);
                    } else {
                        // This block executes if getLocation() returned null due to an error.
                        this.mapPlaceholderEle.innerHTML = '<div style="padding: 1rem; color: red">Adresse nicht gefunden.</div>';
                    }
                });
        }
    }
    
    async getLocation() {
        // requires https://maps.googleapis.com/maps/api/js
        // Example for async/await: https://gabrieleromanato.name/javascript-how-to-use-the-google-maps-api-with-promises-and-async-await
        const address = this.getAddress(true);
        try {
            const response = await new this.GeocoderLib().geocode({ 'address': address });  //NOSONAR
            const results = response.results;

            if (Array.isArray(results) && results.length > 0) {
                const latitude = results[0].geometry.location.lat();
                const longitude = results[0].geometry.location.lng();
                return [latitude, longitude];
            } else {
                throw new Error('ZERO_RESULTS: No result was found for this GeocoderRequest.');
            }
        } catch (error) {
                if (error.message.includes('ZERO_RESULTS')) {
                    window.JL('Site.Location.getLocation').warn({'message': error.message, 'name': error.name});
                } else {
                    window.JL('Site.Location.getLocation').error({'message': error.message, 'name': error.name, 'stack': error.stack});
                }
            return null;
        }
    }

    showLocationOnGoogleMaps(latitude, longitude) {
        // requires https://maps.googleapis.com/maps/api/js
        const coords = new google.maps.LatLng(latitude, longitude);

        const mapOptions = {
            zoom: 14,
            center: coords,
            mapId: this.mapId,
            mapTypeControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        // create the map, and place it in the HTML map div
        const map = new this.MapsLib(this.mapPlaceholderEle, mapOptions);  //NOSONAR

        // place the initial marker
        const marker = new this.MarkerLib({   //NOSONAR
            position: coords,
            map: map,
            title: 'Angegebener Standort',
            content: new google.maps.marker.PinElement({
                background: '#d32f2f',
                borderColor: '#b71c1c',
                glyphColor: '#fff'
            }).element
        });
    }

    /**
     * Initializes the Google Maps with current coordinates
     * @param {any} coords - Optional: the LatLng coordinates to center the map initially. Default location is Kassel/Germany.
     */
    async initMap(coords) {
        const mapOptions = {
            zoom: 14,
            center: coords || new google.maps.LatLng(51.312801, 9.481544), // default: Kassel
            mapId: this.mapId,
            mapTypeControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        const map = new this.MapsLib(this.mapPlaceholderEle, mapOptions);  // NOSONAR
        // place the initial marker
        
        const marker = new this.MarkerLib({   //NOSONAR
            position: coords,
            map: map,
            title: 'Angegebener Standort',
            content: new google.maps.marker.PinElement({
                background: '#d32f2f',
                borderColor: '#b71c1c',
                glyphColor: '#fff'
            }).element
        });
    }
}
