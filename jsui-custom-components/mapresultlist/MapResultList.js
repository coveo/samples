/*****************************************************************************
 * @license Copyright (c) 2005-2015, Coveo Solutions Inc.
 *****************************************************************************/

var Coveo;
(function (Coveo) {
    var Ui;
    (function (Ui) {
        var MapResultList = (function (_super) {
            __extends(MapResultList, _super);
            function MapResultList(element, options, bindings, elementClassId) {
                var _this = this;
                if (elementClassId === void 0) { elementClassId = MapResultList.ID; }
                _super.call(this, element, elementClassId, bindings);
                this.element = element;
                this.options = options;
                this.bindings = bindings;
                this.map = new google.maps.Map(this.element, {
                    center: { lat: 0, lng: 0 },
                    zoom: 1
                });
                this.geocoder = new google.maps.Geocoder();
                this.infowindow = new google.maps.InfoWindow({ content: "" });
                this.currentlyDisplayedMarkers = [];
                this.resultsDeferred = [];
                this.options = Ui.ComponentOptions.initComponentOptions(element, MapResultList, options);
                Coveo.Assert.exists(element);
                Coveo.Assert.exists(this.options);
                Coveo.Assert.exists(this.options.computeQueryResultAddress);
                this.showOrHideElementsDependingOnState(false, false);
                Coveo.$(this.root).on(Coveo.Events.QueryEvents.newQuery, Coveo.$.proxy(this.handleNewQuery, this));
                Coveo.$(this.root).on(Coveo.Events.ResultListEvents.newResultDisplayed, Coveo.$.proxy(this.handleProcessNewQueryResult, this));
                Coveo.$(this.root).on(Coveo.Events.ResultListEvents.newResultsDisplayed, Coveo.$.proxy(this.handleProcessNewQueryResults, this));
                Coveo.$(this.root).on(Coveo.Events.QueryEvents.duringQuery, Coveo.$.proxy(this.handleDuringQuery, this));
                Coveo.$(this.root).on(Coveo.Events.QueryEvents.queryError, function () {
                    _this.hideWaitingAnimation();
                    _this.removeAllMarkers();
                });
            }
            MapResultList.prototype.handleNewQuery = function () {
                if (!this.disabled) {
                    Coveo.$(this.element).show();
                }
            };
            MapResultList.prototype.removeAllMarkers = function () {
                for (var i = 0; i < this.currentlyDisplayedMarkers.length; i++) {
                    this.currentlyDisplayedMarkers[i].setMap(null);
                }
                this.currentlyDisplayedMarkers = [];
            };
            MapResultList.prototype._autoCreateComponentsInsideResult = function (element, result) {
                Coveo.Assert.exists(element);
                var initOptions = this.searchInterface.options;
                var initParameters = Coveo.$.extend({}, { options: initOptions }, this.getBindings(), { result: result });
                Ui.CoveoJQuery.automaticallyCreateComponentsInside(element, initParameters);
            };
            MapResultList.prototype.handleDuringQuery = function () {
                this.logger.trace('Removing the map markers');
                this.removeAllMarkers();
                this.resultsDeferred = [];
                this.showWaitingAnimation();
                this.showOrHideElementsDependingOnState(false, false);
            };
            MapResultList.prototype.handleProcessNewQueryResult = function (e, data) {
                if (!this.disabled) {
                    this.resultsDeferred.push(this.processQueryResult(data.result, data.item));
                }
            };
            MapResultList.prototype.processQueryResult = function (queryResult, resultDomElement) {
                var deferred = new Coveo.$.Deferred();
                var address = this.options.computeQueryResultAddress(queryResult);

                if (address !== "") {
                    var _this = this;

                    this.geocoder.geocode(
                        { address: address },
                        function(results, status) {
                            _this.processGeocodingResponse(address, results, status, queryResult, resultDomElement);
                            deferred.resolve();
                        }
                    );
                } else {
                    deferred.resolve();
                }

                return deferred;
            };
            MapResultList.prototype.processGeocodingResponse = function (address, results, status, queryResult, resultDomElement) {
                if (status == google.maps.GeocoderStatus.OK) {
                    this.addMarkerOnMap(results[0].geometry.location, queryResult, resultDomElement);
                } else {
                    console.log("Geocoding failed for address: ", address);
                }
            };
            MapResultList.prototype.addMarkerOnMap = function (location, queryResult, resultDomElement) {
                var _this = this;
                var marker = new google.maps.Marker({
                    position: location,
                    map: this.map,
                    title: queryResult.title
                });
                google.maps.event.addListener(marker, 'click', function () {
                    _this.infowindow.content = _this.getQueryResultInfoWindowContent(queryResult, resultDomElement);
                    _this.infowindow.open(_this.map, marker);
                });
                resultDomElement.marker = marker;
                Coveo.$(resultDomElement).on('mouseenter', this.toggleMarkerBounce);
                Coveo.$(resultDomElement).on('mouseleave', this.toggleMarkerBounce);
                this.currentlyDisplayedMarkers.push(marker);
            };
            MapResultList.prototype.getQueryResultInfoWindowContent = function (queryResult, resultDomElement) {
                return resultDomElement.outerHTML;
            };
            MapResultList.prototype.toggleMarkerBounce = function(e) {
                if (e.currentTarget.marker.getAnimation() != null) {
                    e.currentTarget.marker.setAnimation(null);
                } else {
                    e.currentTarget.marker.setAnimation(google.maps.Animation.BOUNCE);
                }
            };
            MapResultList.prototype.handleProcessNewQueryResults = function () {
                var _this = this;
                $.when.apply($, this.resultsDeferred).done(function () {
                    Coveo.$(_this.element).fastToggle(!_this.disabled);
                    if (!_this.disabled) {
                        _this.hideWaitingAnimation();
                        _this.centerMapOnMarkers();
                        _this.showOrHideElementsDependingOnState(true, _this.currentlyDisplayedMarkers.length != 0);
                    }
                });
            };
            MapResultList.prototype.centerMapOnMarkers = function () {
                this.map.fitBounds(this.currentlyDisplayedMarkers.reduce(function (bounds, marker) {
                        return bounds.extend(marker.getPosition());
                    },
                    new google.maps.LatLngBounds()
                ));
            };
            MapResultList.prototype.reset = function () {
                this.hideWaitingAnimation();
                this.removeAllMarkers();
            };
            MapResultList.prototype.showOrHideElementsDependingOnState = function (hasQuery, hasResults) {
                Coveo.$(this.element).find('.coveo-show-if-query').fastToggle(hasQuery);
                Coveo.$(this.element).find('.coveo-show-if-no-query').fastToggle(!hasQuery);
                Coveo.$(this.element).find('.coveo-show-if-results').fastToggle(hasQuery && hasResults);
                Coveo.$(this.element).find('.coveo-show-if-no-results').fastToggle(hasQuery && !hasResults);
            };
            MapResultList.prototype.showWaitingAnimation = function () {
                switch (this.options.waitAnimation.toLowerCase()) {
                    case 'fade':
                        Coveo.$(this.element).addClass('coveo-fade-out');
                        break;
                    case 'spinner':
                        if (Coveo.$(this.element).find('.coveo-wait-animation').length == 0) {
                            Coveo.$(this.element).append('<img src="../image/wait.gif" class="coveo-wait-animation"/>');
                        }
                        break;
                }
            };
            MapResultList.prototype.hideWaitingAnimation = function () {
                switch (this.options.waitAnimation.toLowerCase()) {
                    case 'fade':
                        Coveo.$(this.element).removeClass('coveo-fade-out');
                        break;
                    case 'spinner':
                        Coveo.$(this.element).children('img.coveo-wait-animation').detach();
                        break;
                }
            };
            MapResultList.ID = 'MapResultList';
            MapResultList.options = {
                waitAnimation: Ui.ComponentOptions.buildStringOption({ defaultValue: 'none' })
            };
            return MapResultList;
        })(Ui.Component);
        Ui.MapResultList = MapResultList;
        Ui.CoveoJQuery.registerAutoCreateComponent(MapResultList);
    })(Ui = Coveo.Ui || (Coveo.Ui = {}));
})(Coveo || (Coveo = {}));