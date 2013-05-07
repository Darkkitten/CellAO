<?php
// .:My Profile Information:.
//
//
// BitFields:
//
// -Expansions
// 0 = "Notum Wars"
// 1 = "Shadow Lands"
// 2 = "Shadow Lands Pre-Order"
// 3 = "Alien Invasion"
// 4 = "Alien Invasion Pre-Order"
// 5 = "Lost Eden"
// 6 = "Lost Eden Pre-Order"
// 7 = "Legacy of Xan"
// 8 = "Legacy of Xan Pre-Order"
// 9 = "Mail"
//10 = "PMV Obsidian Edition"
// 0=1, 1=2, 2=4, 3=8, 4=16, 5=32, 6=64, 7=128, 8=256, 9=512, 10 = 1024
// So if you want to give someone all expansions is 1+2+4+8+16+32+64+128+256+512+1024 = 2047
//
// -GM Levels
// 1-100
// 100=blackmanes statbuffer usable

$exp = $_SESSION['SESS_EXPANSIONS'];
$Expansion = array('NW', 'SL', 'SL Preorder', 'AI', 'AI Preorder', 'LE', 'LE Preorder', 'LoX', 'LoX Preorder', 'Mail', 'PMV Obsidian Edition');
for ($i = 0; $i < sizeof($Expansion); $i++) {
	if ($exp & pow(2, $i)) {
 		$Expansions .= $Expansion[$i] . ', ';
	}
}
echo "My User ID: <b>".$_SESSION['SESS_ID']."<br></b>";
echo "Created on: <b>".$_SESSION['SESS_CREATIONDATE']."<br></b>";
echo "My Email: <b>".$_SESSION['SESS_EMAIL']."<br></b>";
echo "My User Name: <b>".$_SESSION['SESS_USER_NAME']."<br></b>";
echo "My Max Chars: <b>".$_SESSION['SESS_ALLOWED_CHARACTERS']."<br></b>";
echo "My Gen Flags: <b>".$_SESSION['SESS_FLAGS']."<br></b>";
echo "My Acnt Flags: <b>".$_SESSION['SESS_ACCOUNTFLAGS']."<br></b>";
echo "My Expansions: <b>(".$exp.") ".$Expansions."<br></b>";
echo "My GM LvL: <b>".$_SESSION['SESS_GM']."<br></b>";
echo "My First Name: <b>".$_SESSION['SESS_FIRST_NAME']."<br></b>";
echo "My Last Name: <b>".$_SESSION['SESS_LAST_NAME']."<br></b>";

?>