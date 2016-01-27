<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DisplayMap.aspx.cs" Inherits="DisplayMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Wat Is Dis</title>
    <script src="//code.jquery.com/jquery-1.11.1.min.js"></script>
    <script src="js/maps.js"></script>
    <script src="js/leaflet-0.7.5/leaflet.js"></script>
    <script src="js/leaflet-plugins/bing.js"></script>
    <script src="js/leaflet-plugins/google.js"></script>
    <link href="js/leaflet-0.7.5/leaflet.css" rel="stylesheet" type="text/css"/>
    <link href="//maxcdn.bootstrapcdn.com/bootswatch/3.3.5/cerulean/bootstrap.min.css" rel="stylesheet" />
    <link href="css/styles.css" rel="stylesheet" />
</head>
<div class="container-fluid">
    <div class="row">
        <div id="toplogo" class="col-sm-4"></div>
        <div id="message" class="col-sm-8"></div>
    </div>
    <div class="row">
        <div id="mapdiv" class="col-sm-12">
            <div class="map-canvas"></div>
        </div>
    </div>
    <div class="row">
        <div class="col-sm-3"></div>
        <div class="col-sm-3"></div>
        <div class="col-sm-3"></div>
    </div>
</div>
<body>
</body>
</html>
