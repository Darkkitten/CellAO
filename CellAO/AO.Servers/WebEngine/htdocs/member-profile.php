<?php
	require_once('configs/config.php');
	$theme = THEME;
	$themetabs = $theme.'tabs.php';
	require_once('auth.php');
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<title>My Profile</title>
<link href="<?php echo $theme; ?>css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
<div id="container">
    <!-- The topmost bar -->
    <div id="topbar">
	.:<font color='EEE'><?php echo $_SESSION['SESS_USER_NAME'];?>'s</font> Profile:.&nbsp;&nbsp;&nbsp;&nbsp;
	<a href="member-index.php">Home</a> | <a href="logout.php">Logout</a>
    </div>
    <!-- End of top bar -->

    <!-- This holds the main header -->
    <div id="headerwrapper">
    <h1>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Login Successful!</h1>        <div>
            <!-- This is the site slogan -->
            <h6>
                <a href="admins/administrator.php?pUsername=<?php echo $_SESSION['SESS_USER_NAME']; ?>&changeuser=yes&password=">Admin Area</a>
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
	<?php include "inc/profiler.php" ?>
        </div>
        <!-- End of content holder -->
    </div>
	<?php include "inc/filler.php" ?>
    </div>
</body>
</html>
