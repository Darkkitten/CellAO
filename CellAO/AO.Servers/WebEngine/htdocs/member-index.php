<?php
	require_once('configs/config.php');
	$theme = THEME;
	$themetabs = $theme.'tabs.php';
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<title>Member Index</title>
<link href="<?php echo $theme; ?>css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
<div id="container">
    <!-- The topmost bar -->
    <div id="topbar">
	.:Welcome <font color='EEE'><?php echo $_SESSION['SESS_USER_NAME'];?></font>:.&nbsp;&nbsp;&nbsp;&nbsp;
	<a href="member-profile.php">My Profile</a> | <a href="logout.php">Logout</a>
    </div>
    <!-- End of top bar -->

    <!-- This holds the main header -->
    <div id="headerwrapper">
    <h1>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Login Successful!</h1>        <div>
            <!-- This is the site slogan -->
            <h6>
                &nbsp;
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
