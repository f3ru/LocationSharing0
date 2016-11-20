<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <html>
    <head>
        <title>Geolocation</title>
        <meta name="viewport" content="initial-scale=1.0, user-scalable=no">
        <meta charset="utf-8">
        <style>
            /* Always set the map height explicitly to define the size of the div
       * element that contains the map. */
            #map {
                height: 100%;
            }
            /* Optional: Makes the sample page fill the window. */
            html, body {
                height: 100%;
                margin: 0;
                padding: 0;
            }
        </style>
    </head>
    <body>
        <div id="loginDiv">
            <input id="UserName" type="text" />
            <button type="button" id="CreateUser">Create User</button>
        </div>
        <div id="mapDiv" style="display:none">
            <div id="map" style="height: 400px"></div>
<%--            <button type="button" id="SendLocationToServer">Send My Location To Server</button>--%>
            <br />
            <input id="GroupName" type="text" />
            <button type="button" id="CreateGroup">Create Group</button>
            <br />
            <br />
            <button type="button" id="GetGroupsList">Get Groups List</button>
            <br />
            <div class="row GroupsList"></div>
        </div>
        <script>
            var pos;
            var map;
            var groupPeople = [];
            var allMarkers = [];
            var markerSize = { x: 22, y: 40 };
            var firstTime = true;
            function initMap() {
                map = new google.maps.Map(document.getElementById('map'));
                google.maps.Marker.prototype.setLabel = function (label) {
                    this.label = new MarkerLabel({
                        map: this.map,
                        marker: this,
                        text: label
                    });
                    this.label.bindTo('position', this, 'position');
                };
                var MarkerLabel = function (options) {
                    this.setValues(options);
                    this.span = document.createElement('span');
                    this.span.className = 'map-marker-label';
                };
                MarkerLabel.prototype = $.extend(new google.maps.OverlayView(), {
                    onAdd: function () {
                        this.getPanes().overlayImage.appendChild(this.span);
                        var self = this;
                        this.listeners = [
                        google.maps.event.addListener(this, 'position_changed', function () { self.draw(); })];
                    },
                    draw: function () {
                        var text = String(this.get('text'));
                        var position = this.getProjection().fromLatLngToDivPixel(this.get('position'));
                        this.span.innerHTML = text;
                        this.span.style.left = (position.x - (markerSize.x / 2)) - (text.length * 3) + 10 + 'px';
                        this.span.style.top = (position.y - markerSize.y + 40) + 'px';
                    }
                });
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function (position) {
                        pos = {
                            lat: position.coords.latitude,
                            lng: position.coords.longitude
                        };
                        var marker = new google.maps.Marker({
                            position: pos,
                            map: map,
//                            label:"ME",
                            title:"ME"
                        });
                        allMarkers.push(marker);
                    });
                }
            }
            //$('#SendLocationToServer').click(function () {
            //    var Data = JSON.stringify({ Latitude: pos.lat, Longitude: pos.lng });
            //    $.ajax({
            //        url: "Default.aspx/sendLocationToServer",
            //        data: Data,
            //        type: "POST",
            //        dataType: "json",
            //        contentType: "application/json; charset=utf-8",
            //        success: function (mydata) {
            //        }
            //    });
            //});
            $('#CreateGroup').click(function () {
                var Data = JSON.stringify({ Name: $('#GroupName').val() });
                $.ajax({
                    url: "Default.aspx/CreateGroup",
                    data: Data,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (mydata) {
                    }
                });
            });
            $('#CreateUser').click(function () {
                var Data = JSON.stringify({ Name: $('#UserName').val(), Latitude: pos.lat, Longitude: pos.lng });
                $.ajax({
                    url: "Default.aspx/LoginUser",
                    data: Data,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (mydata) {
                        $('#mapDiv').show();
                        $('#loginDiv').hide();
                        google.maps.event.trigger(map, 'resize');
                        map.setCenter(pos);
                        map.setZoom(13);
                        var maxZoomService = new google.maps.MaxZoomService();
                        maxZoomService.getMaxZoomAtLatLng(pos, function (response) {
                            if (response.status == 'OK') {
                                map.setZoom(response.zoom - 2);
                            }
                        });
                    }
                });
            });
            $('#GetGroupsList').click(function () {
                $('.GroupsList').empty();
                GetGroupList();
            });
            function AfterJoiningGroup()
            {
                setInterval(function () {
                    $.ajax({
                        url: "Default.aspx/GetGroupPeople",
                        type: "POST",
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (mydata) {
                            groupPeople = mydata.d;
                            placeMarker();
                        }
                    });
                    navigator.geolocation.getCurrentPosition(function (position) {
                        pos = {
                            lat: position.coords.latitude,
                            lng: position.coords.longitude
                        };
                        var marker = new google.maps.Marker({
                            position: pos,
                            map: map,
                            //label: "ME",
                            title: "ME"
                        });
                        allMarkers.push(marker);
                    });
                }, 10000);
            }
            function JoinGroup(group_name) {
                var Data = JSON.stringify({ Name: group_name});
                $.ajax({
                    url: "Default.aspx/JoinGroup",
                    data: Data,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (mydata) {
                        //$.ajax({
                        //    url: "Default.aspx/GetGroupPeople",
                        //    type: "POST",
                        //    dataType: "json",
                        //    contentType: "application/json; charset=utf-8",
                        //    success: function (mydata) {
                        //        groupPeople = mydata.d;
                        //        placeMarker();
                        //    }
                        //});
                        AfterJoiningGroup();
                        $('.GroupsList').empty();
                        $('.GroupsList').append("<div>" + group_name + " joined</div>");
                    }
                });
            }
            function placeMarker()
            {
                $.each(allMarkers, function (i, o) {
                    o.setMap(null);
                });
                $.each(groupPeople, function (i, o) {
                    var myLatlng = new google.maps.LatLng(o.Lat, o.Lon);
                    marker = new google.maps.Marker({
                        position: myLatlng,
                        title:o.UserName,
                        //label: o.UserName,
                        map: map
                    });
                    allMarkers.push(marker);
                });
                if (firstTime) {
                    var bounds = new google.maps.LatLngBounds();
                    for (i = 0; i < allMarkers.length; i++) {
                        bounds.extend(allMarkers[i].getPosition());
                    }
                    map.setCenter(bounds.getCenter());
                    map.fitBounds(bounds);
                    firstTime = false;
                }
            }
            function GetGroupList()
            {
                $.ajax({
                    url: "Default.aspx/GetGroupList",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (mydata) {
                        $.each(mydata.d, function (i, o) {
                            $('.GroupsList').append('<a class="groupList" style="cursor:pointer;">' + o + '</a><br/>');
                        });
                        $('.groupList').click(function () {
                            var a = $(this).text();
                            JoinGroup(a);
                        });
                    }
                });
            }
        </script>
        <script async defer
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyAT9rn3SlKVKzsCEUm7Z6Q3ldSIxn4d2tc&callback=initMap">
        </script>
    </body>
    </html>
</asp:Content>