<?php
	//Start session
	session_start();
	
	//Include database connection details
	require_once('configs/config.php');
	require_once('engine.php');
	$theme = THEME;
	//Array to store validation errors
	$errmsg_arr = array();
	
	//Validation error flag
	$errflag = false;
	
	//Connect to mysql server
	$link = mysql_connect(DB_HOST, DB_USER, DB_PASSWORD);
	if(!$link) {
		die('Failed to connect to server: ' . mysql_error());
	}
	
	//Select database
	$db = mysql_select_db(DB_DATABASE);
	if(!$db) {
		die("Unable to select database");
	}
	
	//Function to sanitize values received from the form. Prevents SQL injection
	function clean($str) {
		$str = @trim($str);
		if(get_magic_quotes_gpc()) {
			$str = stripslashes($str);
		}
		return mysql_real_escape_string($str);
	}
	
	//Sanitize the POST values
	$login = clean($_POST['login']);
	$password = clean($_POST['password']);
	
	//Input Validations
	if($login == '') {
		$errmsg_arr[] = 'Login ID missing';
		$errflag = true;
	}
	if($password == '') {
		$errmsg_arr[] = 'Password missing';
		$errflag = true;
	}
	
	//If there are input validations, redirect back to the login form
	if($errflag) {
		$_SESSION['ERRMSG_ARR'] = $errmsg_arr;
		session_write_close();
		header("location: $theme");
		exit();
	}
	
	//Create query
	$qry="SELECT * FROM login WHERE Username='$login'";
	$result=mysql_query($qry);

	//Check whether the query was successful or not
	if($result) {
	if(mysql_num_rows($result) == 1) {
		//Login Successful
		session_regenerate_id();
		$member = mysql_fetch_assoc($result);
		$passhash = $member['Password'];
		if (!webpass($passhash,$password))
		{
			// Login failed
			header("location: login-failed.php");
			exit();
		}
		//CreationDate, Email, Username, Password, Allowed_Characters, Flags, Accountflags, Expansions, GM, FirstName, LastName
		$_SESSION['SESS_ID'] = $member['ID'];
		$_SESSION['SESS_CREATIONDATE'] = $member['CreationDate'];
		$_SESSION['SESS_EMAIL'] = $member['Email'];
		$_SESSION['SESS_USER_NAME'] = $member['Username'];
		$_SESSION['SESS_ALLOWED_CHARACTERS'] = $member['Allowed_Characters'];
		$_SESSION['SESS_FLAGS'] = $member['Flags'];
		$_SESSION['SESS_ACCOUNTFLAGS'] = $member['AccountFlags'];
		$_SESSION['SESS_EXPANSIONS'] = $member['Expansions'];
		$_SESSION['SESS_GM'] = $member['GM'];
		$_SESSION['SESS_FIRST_NAME'] = $member['FirstName'];
		$_SESSION['SESS_LAST_NAME'] = $member['LastName'];
		session_write_close();
		header("location: member-index.php");
		exit();
	}else {
		//Login failed
		header("location: login-failed.php");
		exit();
	}
	}else {
		die("Query failed");
	}
?>