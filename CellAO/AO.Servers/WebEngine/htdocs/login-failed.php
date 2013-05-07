<?php
	require_once('configs/config.php');
	$theme = THEME;
	$themetabs = $theme.'tabs.php';
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <title>Login Failed</title>
<link href="<?php echo $theme; ?>css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
<div id="container">
    <!-- The topmost bar -->
    <div id="topbar">
	Error in Login details.
    </div>
    <!-- End of top bar -->

    <!-- This holds the main header -->
    <div id="headerwrapper">

        <!-- This is the site title -->
        <h1>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Login Failed!</h1>
        <div>
            <!-- This is the site slogan -->
            <h6>
                Please check your username and password.
            </h6>
            <br />
        </div>
	</div>
	    <div id="tabholder">
        <div id="tabs8">
            <ul>
                <!-- CSS Tabs -->
		<?php include "$themetabs";?>
            </ul>
        </div>
    </div>
    <!-- End of the tabs holder -->

    <!-- This is the login controls holder -->



    <!-- End of the login controls holder -->

    <!-- Here's the box for the main article -->
    <div class="articleboxouter">
        <!-- Here's where you can place ur content -->
        <div class="articleboxinner2">
	<?php include "inc/articals.php" ?>
        </div>
        <!-- End of content holder -->
    </div>
	<?php include "inc/filler.php" ?>
    </div>
</body>
</html>
