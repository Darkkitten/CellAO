<?php
require_once('../auth.php');
/*if($_REQUEST['password'] != "compaq") {
	die("Invalid page for You.");
}*/
require_once("../configs/config.php");
require_once('../engine.php');
$theme = THEME;
$themetabs = '../'.$theme.'tabs.php';
if(!$_REQUEST['pUsername']) {
	$pUsername = $_SESSION['SESS_USER_NAME'];
} else {
	$pUsername = $_REQUEST['pUsername'];
}
if($_SESSION['SESS_GM'] != 100) {
	echo "Go somewhere else...<br>";
	echo "<a href='../index.php'>HOME</a><br>";
	echo "<a href='../member-profile.php'>My Profile</a><br>";
	die('Access denied! Your not an admin!<br>Attempt Logged.');
}
$link = mysql_connect(DB_HOST, DB_USER, DB_PASSWORD);
if(!$link) {
	die('Failed to connect to server: ' . mysql_error());
}
/////////////////////////////////////
function getPF($albid) {
$xmlFile = new DOMDocument();
$xmlFile->load("../inc/Playfields.xml");
$xml_album = $xmlFile->getElementsByTagName("Playfield");
$i = 0;
while ($xml_album->item($i) && $xml_album->item($i)->getAttribute('id') != $albid) {
	$i += 1;
}		
$album = $xml_album->item($i); 
$children=$album->childNodes;
   foreach($children as $child) {
	if($child->nodeType==XML_ELEMENT_NODE) {
		$PF_value = $child->nodeValue;
		return $PF_value;
	}
   }
}
//////////////////////////////////////
$xdoc = new DOMDocument();
$xdoc->load('itemsicons.xml');
$xpath = new DOMXPath($xdoc); 

function id2img($uid) {
    global $xdoc;
    global $xpath;
    global $ids;
    global $i;
    if(!$xdoc) {
	die("error");
    }
    $i=0;
    $nodeList = $xpath->query('/item2icon/cellao', $xdoc);
    foreach ($nodeList as $node) {
    	$theid = $node->getAttribute('id');
	$ids[$theid]["name"] = $node->getAttribute('name');
	$ids[$theid]["img"] = $node->getAttribute('img');
    	$i++;
    }
    $qry = ("SELECT * FROM charactersinventory WHERE ID = '$uid'");
    $result=mysql_query($qry);
    if ($row = mysql_fetch_assoc($result)) {
    	do {			  	  
    		$id_num = $row["LowID"];
    		$results = "<img src='http://auno.org/res/aoicons/".$ids[$id_num]["img"].".gif' title='".$id_num.": ".$ids[$id_num]["name"]."' alt='".$id_num."'>";
    		echo $results;
	} while ($row = mysql_fetch_assoc($result));
    }
    // <cellao nodeid=27 id=22064 img=13253 name=Assault-Issue Omni-Pol Body Armor />
}
//////////////////////////////////////
//////////////////////////////////////
$xdocs = new DOMDocument();
$xdocs->load('../inc/Stats.xml');
$xpaths = new DOMXPath($xdocs); 

function id2stats($uid) {
    global $xdocs;
    global $xpaths;
    global $idss;
    global $i;
    if(!$xdocs) {
	die("error");
    }
    $i=0;
    $nodeList = $xpaths->query('/Stats/Stat', $xdocs);
    foreach ($nodeList as $node) {
    	$theids = $node->getAttribute('id');
	$idss[$theids]["Name"] = $node->getAttribute('Name');
    	$i++;
    }
    $qry = ("SELECT * FROM characters_stats WHERE ID = '$uid'");
    $result=mysql_query($qry);
    if ($row = mysql_fetch_assoc($result)) {
    	do {			  	  
    		$statid = $row["Stat"];
    		$statvalue = $row["Value"];
    		$results = "<tr><td width=\"10%\">Stat: ".$idss[$statid]["Name"]."</td><td width=\"90%\">Value: ".$statvalue."</td></tr>";
    		echo $results;
	} while ($row = mysql_fetch_assoc($result));
    }
    // <Stat id="10" Name="ProfessionLevel">
}
//////////////////////////////////////
//Select database
$db = mysql_select_db(DB_DATABASE);
if(!$db) {
	die("Unable to select database");
}

?>
<html>
<head>
<title>CellAO - [User] Administration Panel</title>
<link href="../<?php echo $theme; ?>css/style.css" rel="stylesheet" type="text/css" />
</head>
<body>
<div id="container">
    <!-- The topmost bar -->
    <div id="topbar">
	.:<font color='EEE'>Welcome, <?php echo $_SESSION['SESS_USER_NAME'];?></font>:.&nbsp;&nbsp;&nbsp;&nbsp;
	<a href="../admins/administrator.php?pUsername=<?php echo $_SESSION['SESS_USER_NAME']; ?>&changeuser=yes&password=">Administration</a> | <a href="../logout.php">Logout</a>
    </div>
    <!-- End of top bar -->

    <!-- This holds the main header -->
    <div id="headerwrapper">
    <h1>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Administration</h1>        <div>
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
    <div class="articleboxinner4">
	&nbsp;
        </div>
        <!-- Here's the box for the main article -->
    <div class="articleboxouter">
        <!-- Here's where you can place ur content -->
        <div class="articleboxinner3">
<?php
if(@$_REQUEST['changeme'])
{

$userID = $_REQUEST['userID'];
$Email = $_REQUEST['Email'];
$Username = $_REQUEST['Username'];
$Allowed_Characters = $_REQUEST['Allowed_Characters'];
$Flags = $_REQUEST['Flags'];
$AccountFlags = $_REQUEST['AccountFlags'];
$Expansions = $_REQUEST['Expansions'];
$GM = $_REQUEST['GM'];
$FirstName = $_REQUEST['FirstName'];
$LastName = $_REQUEST['LastName'];

	$qry=("UPDATE login SET Email='$Email', Username='$Username', Allowed_Characters='$Allowed_Characters', Flags='$Flags', AccountFlags='$AccountFlags', Expansions='$Expansions', GM='$GM', FirstName='$FirstName', LastName='$LastName' WHERE ID='$userID'");
	$result=mysql_query($qry);
	if(!$result) {
		die("Query Failed: ". mysql_error());
	}
	else
	{
	echo "<table width=\"100%\" height=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">
	  <tr>
	  <td align=center valign=middle height=\"100%\" width=\"100%\">
		  <table border=\"0\" cellspacing=\"1\" cellpadding=\"0\">
		  <tr>
		  <td colspan=\"2\" align=center>
		  	<b>User '$Username' has been changed!</b>
		  </td>
		  </tr>
		  <tr>
		  <td colspan=\"2\" align=center>
		  	<form><input type='button' onClick=\"parent.location='administrator.php?pUsername=$Username&changeuser=yes'\" value='OK'></form>
		  </td>
		  </tr>
		  </table>
		 </td>
		 </tr>
		 </table>";
	}
}
elseif(@$_REQUEST['changeuser'])
{
	$SESS_ID = $_SESSION['SESS_ID'];
	
	$qry=("SELECT ID FROM characters WHERE Username='$pUsername'");
	$result = mysql_query($qry);
	$member = mysql_fetch_assoc($result);
	$ID = $member['ID'];
	
	$qry=("SELECT ID FROM login WHERE Username='$pUsername'");
	$result = mysql_query($qry);
	$member = mysql_fetch_assoc($result);
	$userID = $member['ID'];
	
	$qry=("SELECT * FROM login WHERE Username='$pUsername'");
	$result=mysql_query($qry);
	while ($row = mysql_fetch_assoc($result)) {
		$eFirstName = $row["FirstName"];
		$eLastName = $row["LastName"];
		$eUsername = $row["Username"];
		$CreationDate = $row["CreationDate"];
		$eEmail = $row["Email"];
		$eAllowed_Characters = $row["Allowed_Characters"];
		$eFlags = $row["Flags"];
		$eAccountFlags = $row["AccountFlags"];
		$eExpansions = $row["Expansions"];
		$eGM = $row["GM"];
		
		if($eFirstName == "") {
			$FirstName = "Please select a First Name";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$FirstName = $row["FirstName"];}
		}
		
		if($eLastName == "") {
			$LastName = "Please select a Last Name";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$LastName = $row["LastName"];}
		}
				
		if($eUsername == "") {
			$Username = "Please select a Username";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$Username = $row["Username"];}
		}
		
		if($eEmail == "") {
			$Email = "Please type an Email";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$Email = $row["Email"];}
		}
		
		if($eAllowed_Characters == "") {
			$Allowed_Characters = "Please input Max Characters";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$Allowed_Characters = $row["Allowed_Characters"];}
		}
		
		if($eFlags == "") {
			$Flags = "Please input other Flags";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$Flags = $row["Flags"];}
		}
		
		if($eAccountFlags == "") {
			$AccountFlags = "Please input Account Flags";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$AccountFlags = $row["AccountFlags"];}
		}
		
		if($eExpansions == "") {
			$Expansions = "Please Type Exansion bits";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$Expansions = $row["Expansions"];}
		}
		
		if($eGM == "") {
			$GM = "Please select a GM level";
		} else {
			$qry = ("SELECT * FROM login WHERE Username = '$pUsername'");
			$result=mysql_query($qry);
			if ($row = mysql_fetch_assoc($result)) {$GM = $row["GM"];}
		}
	}
?>
  <table width="600" height="100%" align=center valign=middle border="0" cellpadding="0" cellspacing="0">
	<tr>
	<td align=center valign=middle height="100%" width="100%">
  <table width="600" border="1" cellspacing="0" cellpadding="3">
    <tr style=background:#3366ee;>
      <td colspan="2">
        <div align="center"><b>Editing User: <font color="red"><form id="form2" name="form2" action="administrator.php">
	<input type="hidden" name="changeuser" value="yes"><font color=Green size=2>Username: </font><input type="text" name="pUsername" value="<?php echo $Username?>"></font><input type="submit" name="submit" value='OK'></form></div>
      </td>
    </tr>
    <form id="form1" name="form1" action="administrator.php?changeme=true&userID=<?php echo $userID;?>" method="post">
	<input type="hidden" name="id" value="<?php echo $ID?>">
    <tr>
    	<td colspan="2"><center><b>LOCATION</b></center></td>
    </tr>
    <tr> 
      <td width="50%"><font color="#555555">Playfield</font></b></td>
      <td width="50%"><font color="#555555">
			  <?php
			  $qry = ("SELECT * FROM characters WHERE Username = '$Username'");
			  $result=mysql_query($qry);
			  $row = mysql_fetch_assoc($result);
			  if ($row["playfield"] == "") {
			  	print "?";
			  } else {
			  	$userPF = $row["playfield"];
			  	print getPF($userPF);
			  }
			  ?>
        </font></td>
    </tr>
    <tr>
      <td colspan="2"><center><b>INVENTORY</b></center></td>
    </tr>
    <tr> 
      <td width="50%"><font color="#555555">Users Inventory</font></b></td>
      <td width="50%"><font color="#555555">
	  		  <?php
	  		  print ("<select id=\"inventory\" onchange=\"sendIt(this);\">");
			  $qry = ("SELECT * FROM charactersinventory WHERE ID = '$ID'");
			  $result=mysql_query($qry);
			  if ($row = mysql_fetch_assoc($result)) {
				  do {				  	  
					  $id_num = $row["LowID"];
					  print ("<option value=$id_num>$id_num</option>");
				  } while ($row = mysql_fetch_assoc($result));
			  }
			  print ("</select>");
			  ?>
        </font></td>
    <tr>
    		<td colspan="2"><?php
 		print ("<div align=\"center\" id=\"id2img\">");
		$imagesrc = id2img($ID);
		echo $imagesrc;
		print ("</div>");
		?></td><?php
		$statsrc = id2stats($ID);
		echo $statsrc;
		?>
    </tr></tr></tr>
    <tr>
      <td width="50%"><font color="#555555">User Name</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="Username" value="<?php echo $Username?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">First Name</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="FirstName" value="<?php echo $FirstName?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Last Name</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="LastName" value="<?php echo $LastName?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Email</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="Email" value="<?php echo $Email?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Max Allowed Characters</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="Allowed_Characters" value="<?php echo $Allowed_Characters?>">
        </font></td>
    </tr>
    <tr>
    	<td colspan="2"><center><b>ATTRIBUTES</b></center></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Flags</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="Flags" value="<?php echo $Flags?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Account Flags</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="AccountFlags" value="<?php echo $AccountFlags?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">Expansions</font></td>
      <td width="50%"><font color="#555555">
        <input type="text" name="Expansions" value="<?php echo $Expansions?>">
        </font></td>
    </tr>
    <tr>
      <td width="50%"><font color="#555555">GM Level (0 default)</font></td>
      <td width="50%"> <b><font size=2 color="#555555">
        <input type="text" name="GM" value="<?php echo $GM?>"> *100 for admin.
        </font></td>
    </tr>
    <tr> 
      <td colspan="2"> 
        <div align="center"> 
          <input type="submit" name="submit" value="Change"> 
          <input type="button" onClick="#" value="Delete <?php echo $Username?>" "onmouseover="this.value='Not Implimented'" onmouseout="this.value='Delete <?php echo $Username?>'"> 
          <input type="button" onClick="#" value="Back to list" "onmouseover="this.value='Not Implimented'" onmouseout="this.value='Back to list'">
      	</div>
      </td>
    </tr>
  </table>
  </td>
  </tr>
  </table>
</form>
<?php
}
?>
<div class="articleboxouter">
        <!-- Here's where you can place ur content -->
        <div class="articleboxinner2">
	&nbsp;
        </div>
        <!-- End of content holder -->
    </div>
    </div>
</body>
</html>