<?PHP
function createhash($password, $salt = "")
{
    if ($salt == "")
        $salt = sprintf('%02x%02x', mt_rand(0, 0xff), mt_rand(0, 0xff));
    	return strtolower($salt) . "$" . md5(pack("H*", $salt) . $password);
}

function CreateAccount($username, $password, $charsallowed, $expansion, $email)
{
    $passhash = createhash($password);
    $connection=mysql_connect(DB_HOST, DB_USER, DB_PASSWORD) or die ("Connection failed");	
    mysql_select_db(DB_DATABASE,$connection);
    $query = mysql_query("INSERT INTO login (CreationDate, Email, Username, Password, Allowed_Characters, Flags, Accountflags, Expansions, GM) VALUES (NOW(), '$email', '$username', '$passhash', $charsallowed, 0,0, $expansion, 0)");
    if ($query===false)
    {
        return false;
    }
    return true;
}

function webpass($hash, $password)
{
    $parts = explode("$", $hash);
    $newhash = createhash($password, $parts[0]);
    if ($newhash == $hash)
    {
        return true;
    }
    return false;
}?>