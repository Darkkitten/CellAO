<?php
echo '404 error: http://' . $_SERVER['SERVER_NAME'] . $_SERVER['REQUEST_URI']; 
?>
<html>
<head>
<title>
The page Not Found
</title>
<style>
body {
background-color:#000000;
color:#FFFFEE;
}
hr{
color:blue;
noshadow:true;
}
a:visited{
color:tomato;
}
a:link{
color:cyan;
}
a:hover{
color:yellow;
}
.m1{
color:red;
font-weight:bold;
}
</style>
<script>
function mailWrite(){
lik = document.getElementById('mail')
lik.href="mailto:rick@unixpop.com?subject=Missing Document&body=The following URL is missing from your server\n [URL]"+document.location+"[/URL] \nThe previous page is <?=$_SERVER['HTTP_REFERER']?>"
url = document.getElementById('url')
url.innerHTML = document.location
}
</script>
</head>
<body onload="mailWrite()">
<hr />
<div height="300"; valign="middle" style="background-color:gold; margin-top:0px; padding:1px; vertical-align:middle; height:400px;">
<div style="background-color:#000000; postion:static; padding:0px; height:398px">
<p align="center" style="font-size:48pt;">404</p>
<h1 style="color:red;">The page is missing.</h1>
<h3 style="color:tomato;"><i>The page you want is not available on the server. Check your spelling or choose one option of the following.</i></h3>
<p align="center" style="font-size:12pt;">
URL requested is: <span id="URL"></span><br /> To inform the web master click <a href="#" id="mail">Here</a><br />
You are coming from: <?=($_SERVER['HTTP_REFERER'])?$_SERVER['HTTP_REFERER']:"<span class='m1'>Not Defined</span>"?><br />
To go back click <a href="javascript:history.back()">Here</a>
</p>
</div>
</div>
<hr />
<p align="center">All rights are reserved: Said Bakr OnLine<sup>&copy;</sup><?$p=getdate();echo $p['year'];?></p>
</body>
</html>