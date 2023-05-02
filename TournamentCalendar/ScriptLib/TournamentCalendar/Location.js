class Location {
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
    */
    constructor(countryId, zipCodeId, cityId, streetId, btnMapModalId, mapPlaceholderId) {
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
        this.mapPlaceholderEle = document.getElementById(mapPlaceholderId);
        this.geocoder = new google.maps.Geocoder();
        setInterval(this.enableDisableModalButton.bind(this), 500);
    }

    get addressElements() {
        return [this.countryIdEle, this.zipCodeEle, this.cityEle, this.streetEle];
    }

    enableDisableModalButton()  {
        if (this.isAddressEntered) {
            this.btnMapModalEle.removeAttribute('disabled');
            this.btnMapModalEle.style.cursor = 'pointer';
        } else {
            this.btnMapModalEle.setAttribute('disabled', 'disabled');
            this.btnMapModalEle.style.cursor = 'not-allowed';
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
        // Default display, in case no location was found
        this.mapPlaceholderEle.innerHTML = '<div style="padding: 1rem; color: gray">Die Karte wird gezeigt,<br />wenn die Adresse gefunden wurde.</div>';

        if (this.isAddressEntered) {
            this.getLocation()
                .then(geoCoderResult => this.showLocationOnGoogleMaps(geoCoderResult[0], geoCoderResult[1]))
                .catch( /* swallow-up */);
        }
    }

    async getLocation() {
        // requires https://maps.googleapis.com/maps/api/js
        // Example for async/await: https://gabrieleromanato.name/javascript-how-to-use-the-google-maps-api-with-promises-and-async-await
        const address = this.getAddress(true);
        var response = await this.geocoder.geocode({ 'address': address })
        if (response.results[0]) {
            const latitude = response.results[0].geometry.location.lat();
            const longitude = response.results[0].geometry.location.lng();
            return [latitude, longitude];
        } else {
            return null;
        }
    }

    showLocationOnGoogleMaps(latitude, longitude) {
        // requires https://maps.googleapis.com/maps/api/js
        const coords = new google.maps.LatLng(latitude, longitude);

        const mapOptions = {
            zoom: 14,
            center: coords,
            mapTypeControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        //create the map, and place it in the HTML map div
        const map = new google.maps.Map(this.mapPlaceholderEle, mapOptions);

        //place the initial marker
        const marker = new google.maps.Marker({
            position: coords,
            map: map,
            title: 'Angegebener Standort'
        });
    }
}