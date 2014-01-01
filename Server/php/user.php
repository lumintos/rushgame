<?php
$cur_dir = getcwd();
include $cur_dir."/include/db/db_connect.php";
include $cur_dir."/include/db/database.php";
include $cur_dir."/include/api/user_api.php";

header("Content-type: text/xml"); 
$xml_output = "<?xml version=\"1.0\"?>\n";

$xml_output .= "<response>"; 

/* SERVICE: Login */
if (isset($_GET['action']) && $_GET['action'] == 'login') {
    if (isset($_POST['username'], $_POST['password'])) {
        $username = $_POST['username'];
        $password = $_POST['password']; // The hashed password.
        #$username="test";
        #$password="test";
     
        if (user_login($username, $password, $mysqli) == true) {
            // Login success 
            $xml_output .= "<code>Success</code>";
        } else {
            // Login failed 
            $xml_output .= "<code>Fail</code>";
        }
    } else if (isset($_GET['username'], $_GET['password'])) {
        if (user_login($_GET['username'], $_GET['password'], $mysqli) == true) {
            $xml_output .= "<code>Success</code>";

            
        } else { 
            $xml_output .= "<code>Fail</code>";
        }
    } else {
        $xml_output .= "<code>Invalid request</code>";
    }

/* SERVICE: Logout */
} else if (isset($_GET['action']) && $_GET['action'] == 'logout') {
    if (isset($_POST['username'])) {
        $username = $_POST['username'];
        $res = user_logout($username, $mysqli);
    } else {
        $res = "Invalid_Request";
    }
    //$res = isset($_POST['username']) ? user_logout($_POST['username'], $mysqli) : "Invalid request";
    $xml_output .= "<code>".$res."</code>";

/* SERVICE: Register */
} else if (isset($_GET['action']) && $_GET['action'] == 'register') {
    //$username = "hieu".date('Y-m-d H:i:s');
    //$user = array("username"=>$username, "password" => "test", "hieu@hieu.com");
    //$res = db_insert_user($user, $mysqli);

    //die($res);

    if (!isset($_POST["username"]) || !isset($_POST["password"])) {
        $xml_output .= "<code>Missing info</code>";
    } else {
        $username = $_POST["username"];
        $password = $_POST["password"];
        //die($username.$password);
        if (!isset($_POST["email"])) {
            $email = "";
        } else {
            $email = $_POST["email"];
        }
        //die($email);

        $result = user_register($username, $password, $email, $mysqli);
        //$result = user_register($username, $password, $email, $mysqli);
        $xml_output .= "<code>".$result."</code>";
    }

/* SERVICE: Query user information */
} else if (isset($_GET['action']) && $_GET['action'] == 'query') {
    if (isset($_GET['username'])) {
        $username = $_GET['username'];
        $user = user_query($username, $mysqli);
        if ($user != null) {
            $xml_output .= "<code>OK</code>";
            $xml_output .= "<user_info>";
            $xml_output .= "<id>".$user['id']."</id>";
            $xml_output .= "<username>".$user['username']."</username>";
            $xml_output .= "<email>".$user['email']."</email>";
            $xml_output .= "<score>".$user['score']."</score>";
            $xml_output .= "<is_online>".$user['is_online']."</is_online>";
            $xml_output .= "</user_info>";
        } else {
            $xml_output .= "<code>User non exist</code>";
        }
    }

/* SERVICE: Set user online status */
} else if (isset($_GET['action']) && $_GET['action'] == 'set_online') {
    if (isset($_POST['username'])) {
        $username = $_POST['username'];
        if (isset($_POST['is_online']) && $_POST['is_online'] == "true") {
            $res = user_mark_user_online_status($username, true, $mysqli);
            $xml_output .= "<code>". $res . "</code>";
        } else if (isset($_POST['is_online']) && $_POST['is_online'] == "false") {
            $res = user_mark_user_online_status($username, false, $mysqli);
            $xml_output .= "<code>". $res . "</code>";
        } else {
            $xml_output .= "<code>Invalid request</code>";
        }
    } else {
        $xml_output .= "<code>Invalid request</code>";
    }
}

$xml_output .= "</response>";
echo $xml_output;
