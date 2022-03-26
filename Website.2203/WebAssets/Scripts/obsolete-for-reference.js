google.charts.load('current', { 'packages': ['line', 'bar', 'gantt'] });


window.Charts = {
    capitalCashflow: {
        init: function (elem) {
            let data = new google.visualization.DataTable();
            data.addColumn('date', 'Month');
            data.addColumn('number', 'Cashflow');
            data.addColumn('number', 'Equity');
            data.addColumn('number', 'Mezzanine Debt');
            data.addColumn('number', 'Senior Debt');

            let options = {
                animation: {
                    duration: 1000,
                    easing: 'inAndOut',
                    startup: true
                },
                legend: {
                    position: 'bottom'
                }
            };

            let chart = new google.charts.Line(elem);

            elem._chart = chart;
            elem._data = data;
            elem._options = options;
        },

        drawChart: function (elem, rows) {
            let data = elem._data;
            let options = elem._options;
            let chart = elem._chart;

            function resizeChart() {
                chart.draw(data, google.charts.Line.convertOptions(options));
            }

            if (document.addEventListener) {
                window.addEventListener('resize', resizeChart);
            }
            else if (document.attachEvent) {
                window.attachEvent('onresize', resizeChart);
            }
            else {
                window.resize = resizeChart;
            }

            data.removeRows(0, data.getNumberOfRows());

            for (let i = 0; i < rows.length; i++) {
                data.addRows([
                    [new Date(rows[i].year, rows[i].month, rows[i].day), rows[i].cashBalance, rows[i].equity, rows[i].mezzanine, rows[i].senior]
                ]);
            }

            chart.draw(data, google.charts.Line.convertOptions(options));
        }
    },


    salesHistogram: {
        init: function (elem) {
            let options = {
                animation: {
                    duration: 1000,
                    easing: 'inAndOut',
                    startup: true
                },
                legend: {
                    position: 'bottom'
                }
            };

            let chart = new google.charts.Bar(elem);

            elem._chart = chart;
            elem._options = options;
        },

        drawChart: function (elem, rows) {
            var data = google.visualization.arrayToDataTable(rows);
            let options = elem._options;
            let chart = elem._chart;

            function resizeChart() {
                chart.draw(data, google.charts.Bar.convertOptions(options));
            }

            if (document.addEventListener) {
                window.addEventListener('resize', resizeChart);
            }
            else if (document.attachEvent) {
                window.attachEvent('onresize', resizeChart);
            }
            else {
                window.resize = resizeChart;
            }

            chart.draw(data, google.charts.Bar.convertOptions(options));
        }
    },


    gantt: {
        init: function (elem) {
            let data = new google.visualization.DataTable();
            data.addColumn('string', 'Task ID');
            data.addColumn('string', 'Task Name');
            data.addColumn('date', 'Start Date');
            data.addColumn('date', 'End Date');
            data.addColumn('number', 'Duration');
            data.addColumn('number', 'Percent Complete');
            data.addColumn('string', 'Dependencies');

            let options = {
                animation: {
                    duration: 1000,
                    easing: 'out',
                    startup: true
                },
                backgroundColor: {
                    fill: "#ffffff00"
                }
            };

            let chart = new google.visualization.Gantt(elem);

            elem._chart = chart;
            elem._data = data;
            elem._options = options;
        },

        drawChart: function (elem, rows) {
            let data = elem._data;
            let options = elem._options;
            let chart = elem._chart;

            function resizeChart() {
                chart.draw(data, options);
            }

            if (document.addEventListener) {
                window.addEventListener('resize', resizeChart);
            }
            else if (document.attachEvent) {
                window.attachEvent('onresize', resizeChart);
            }
            else {
                window.resize = resizeChart;
            }

            data.removeRows(0, data.getNumberOfRows());

            for (let i = 0; i < rows.length; i++) {
                data.addRows([
                    [rows[i].taskId, rows[i].taskName, new Date(rows[i].startYear, rows[i].startMonth, rows[i].startDay),
                    new Date(rows[i].endYear, rows[i].endMonth, rows[i].endDay), null, rows[i].percentComplete, rows[i].dependenciesCsv]
                ]);
            }

            chart.draw(data, options);
        }
    }
};


window.OpenLayers = {
    soldPrices: {
        init: function (elem, dotNetObject, baseAddress, searchRadius, soldPrices) {
            // Icons
            let baseIconStyle = new ol.style.Style({
                image: new ol.style.Circle({
                    fill: new ol.style.Fill({
                        color: 'rgba(55, 200, 150, 0.6)'
                    }),
                    stroke: new ol.style.Stroke({
                        width: 1,
                        color: 'rgba(55, 200, 150, 0.8)'
                    }),
                    radius: 10
                })
            });

            let targetIconStyle = new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5, 1],
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'fraction',
                    scale: 1.6,
                    src: '_content/Vectis.AppCore/icons/drop-pin-blue.svg'
                })
            });

            let searchStyle = new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255, 100, 50, 0.2)'
                }),
                stroke: new ol.style.Stroke({
                    width: 2,
                    color: 'rgba(255, 100, 50, 0.8)'
                })
            });


            // Map Layer
            let mapLayer = new ol.layer.Tile({
                source: new ol.source.OSM()
            });


            // Base location layer
            let basePoint = ol.proj.transform([baseAddress.longitude, baseAddress.latitude], 'EPSG:4326', 'EPSG:3857');

            let baseLocation = new ol.Feature({
                geometry: new ol.geom.Point(basePoint),
                postcode: baseAddress.postcode
            });

            let baseLocationVector = new ol.source.Vector({
                features: [baseLocation]
            });

            let baseLocationLayer = new ol.layer.Vector({
                source: baseLocationVector,
                style: baseIconStyle
            });


            // Search area layer as a circle
            let searchSource = new ol.source.Vector({
                projection: 'EPSG:4326'
            });

            let searchLayer = new ol.layer.Vector({
                source: searchSource,
                style: searchStyle
            });


            // Sold locations layer
            let soldLocationsVector = new ol.source.Vector();

            for (let i = 0; i < soldPrices.length; i++) {
                let loc = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.transform([soldPrices[i].key.longitude, soldPrices[i].key.latitude], 'EPSG:4326', 'EPSG:3857')),
                    postcode: soldPrices[i].key.postcode
                });

                soldLocationsVector.addFeature(loc);
            }

            let soldLocationsLayer = new ol.layer.Vector({
                source: soldLocationsVector
            });


            // Mouse wheel requires CTRL keypress - referenced source uses ALT key
            // ref https://github.com/openlayers/openlayers/issues/7666
            let mouseInteraction = new ol.interaction.MouseWheelZoom();

            let oldFn = mouseInteraction.handleEvent;
            mouseInteraction.handleEvent = function (e) {
                let type = e.type;
                if (type !== "wheel" && type !== "wheel") {
                    return true;
                }

                if (!e.originalEvent.ctrlKey) {
                    return true
                }

                oldFn.call(this, e);
            }


            // The Map!
            let map = new ol.Map({
                interactions: ol.interaction.defaults({ mouseWheelZoom: false }).extend([mouseInteraction]),
                target: elem.id,
                layers: [mapLayer, searchLayer, soldLocationsLayer, baseLocationLayer],
                controls: ol.control.defaults({
                    attributionOptions: {
                        collapsible: false
                    }
                }),
                view: new ol.View({
                    center: ol.proj.fromLonLat([baseAddress.longitude, baseAddress.latitude]),
                    zoom: 13
                })
            });


            // Add the circle
            let projection = map.getView().getProjection();
            let center = ol.proj.fromLonLat([baseAddress.longitude, baseAddress.latitude]);
            let pointResolution = ol.proj.getPointResolution(projection, 1, center)
            let myRadius = searchRadius / pointResolution;
            let circle = new ol.geom.Circle(center, myRadius);
            let circleVector = new ol.Feature(circle);
            searchSource.addFeature(circleVector);


            // Feature selection
            let select = new ol.interaction.Select({
                condition: ol.events.condition.click,
                layers: [soldLocationsLayer],
                style: targetIconStyle
            });

            map.addInteraction(select);

            select.on('select', function (e) {
                if (e.selected.length == 0) {
                    dotNetObject.invokeMethodAsync('SetClickedPostcode', "");
                }
                else {
                    let selectedPostcode = e.selected[0].get('postcode');
                    dotNetObject.invokeMethodAsync('SetClickedPostcode', selectedPostcode);
                }
            });


            // Store the locations layers for later updates
            elem._baseAddress = baseAddress;
            elem._searchRadius = searchRadius;
            elem._searchSource = searchSource;
            elem._soldLocationsLayer = soldLocationsLayer;
            elem._select = select;
            elem._projection = projection;
        },


        updateLocations: function (elem, searchRadius, soldPrices, selectedPostcode) {
            // Update search circle
            if (searchRadius != elem._searchRadius) {
                let center = ol.proj.fromLonLat([elem._baseAddress.longitude, elem._baseAddress.latitude]);
                let pointResolution = ol.proj.getPointResolution(elem._projection, 1, center)
                let myRadius = searchRadius / pointResolution;
                let circle = new ol.geom.Circle(center, myRadius);
                let circleVector = new ol.Feature(circle);

                elem._searchRadius = searchRadius;
                elem._searchSource.clear();
                elem._searchSource.addFeature(circleVector);
            }

            // Locations and Vectors
            elem._soldLocationsVector = new ol.source.Vector();

            for (let i = 0; i < soldPrices.length; i++) {
                let loc = new ol.Feature({
                    geometry: new ol.geom.Point(ol.proj.transform([soldPrices[i].key.longitude, soldPrices[i].key.latitude], 'EPSG:4326', 'EPSG:3857')),
                    postcode: soldPrices[i].key.postcode
                });

                elem._soldLocationsVector.addFeature(loc);

                if (soldPrices[i].key.postcode == selectedPostcode) {
                    elem._select.getFeatures().push(loc);
                }
            }


            // Update sources for the layers
            elem._soldLocationsLayer.setSource(elem._soldLocationsVector);
        }
    }
};


window.GeneralInterop = {
    DisplayObject: (dateTime, obj) => {
        if (dateTime !== "") {
            console.log(" ");
            console.log(dateTime);
        }

        console.log(obj);
        return true;
    },

    GetElementText: (id) => {
        let x = document.getElementById(id).innerText;
        return x;
    },

    ThemeSetter: {
        setTheme: function (sheetName) {
            document.getElementById("app-theme").setAttribute("href", "_content/Vectis.AppCore/css/" + sheetName + "-theme.min.css");
        }
    },

    TitleSetter: {
        SetTitle: function (title) {
            document.title = title;
        }
    },
}