//jQuery to collapse the navbar on scroll
$(window).scroll(function() {
    if ($(".navbar").offset().top > 50) {
        $(".navbar-fixed-top.transform").addClass("top-nav-collapse");
    } else {
        $(".navbar-fixed-top.transform").removeClass("top-nav-collapse");
    }
});

//jQuery for page scrolling feature - requires jQuery Easing plugin
$(function() {
    $('.page-scroll a').bind('click', function(event) {
        var $anchor = $(this);
        var targetEl = $($anchor.data('target'));
        if (targetEl.length === 0)
            return;

        $('html, body').stop().animate({
            scrollTop: targetEl.offset().top
        }, 1500, 'easeInOutExpo');
        event.preventDefault();
    });
});

//var myOptions = {
//    zoom: 15,
//    center: new google.maps.LatLng(53.385873, -1.471471),
//    mapTypeId: google.maps.MapTypeId.ROADMAP,
//};

//var map = new google.maps.Map(document.getElementById('map'), myOptions);


function initializeMap(id) {
    "use strict";
    var image = '/Content/swetugg-2016/img/icon-map.png';

    var overlayTitle = 'Swetugg 2016';

    var locations = [
        //point number 1 
        // ['Swetugg 2016', 'Stockholm', '59.326142', '17.9875455'],
        ['Swetugg 2016', 'Stockholm', 59.2910932, 18.0836 ]
    ];

    id = (id === undefined) ? 'map' : id;

    var map = new google.maps.Map(document.getElementById(id), {
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        scrollwheel: false,
        zoomControl: true,
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.LARGE,
            position: google.maps.ControlPosition.LEFT_CENTER
        },
        streetViewControl: true,
        scaleControl: false,
        zoom: 11,
    });

    var myLatlng;
    var marker, i;
    var bounds = new google.maps.LatLngBounds();
    var infowindow = new google.maps.InfoWindow({ content: "loading..." });

    for (i = 0; i < locations.length; i++) {


        if (locations[i][2] !== undefined && locations[i][3] !== undefined) {
            var content = '<div class="infoWindow">' + locations[i][0] + '<br>' + locations[i][1] + '</div>';
            (function (content) {
                myLatlng = new google.maps.LatLng(locations[i][2], locations[i][3]);

                marker = new google.maps.Marker({
                    position: myLatlng,
                    icon: image,
                    title: overlayTitle,
                    map: map
                });

                google.maps.event.addListener(marker, 'click', (function () {
                    return function () {
                        infowindow.setContent(content);
                        infowindow.open(map, this);
                    };

                })(this, i));

                if (locations.length > 1) {
                    bounds.extend(myLatlng);
                    map.fitBounds(bounds);
                } else {
                    map.setCenter(myLatlng);
                }

            })(content);
        } else {

            var geocoder = new google.maps.Geocoder();
            var info = locations[i][0];
            var addr = locations[i][1];
            var latLng = locations[i][1];

            (function (info, addr) {

                geocoder.geocode({

                    'address': latLng

                }, function (results) {

                    myLatlng = results[0].geometry.location;
                    
                    marker = new google.maps.Marker({
                        position: myLatlng,
                        icon: image,
                        title: overlayTitle,
                        map: map
                    });
                    var $content = '<div class="infoWindow">' + info + '<br>' + addr + '</div>';
                    google.maps.event.addListener(marker, 'click', (function () {
                        return function () {
                            infowindow.setContent($content);
                            infowindow.open(map, this);
                        };
                    })(this, i));
                    
                    if (locations.length > 1) {
                        bounds.extend(myLatlng);
                        map.fitBounds(bounds);
                    } else {
                        map.setCenter(myLatlng);
                    }
                });
            })(info, addr);

        }
    }
}

