<?php
include "db_connect.php";
include "functions.php";

header("Content-type: text/xml"); 
$xml_output = "<?xml version=\"1.0\"?>\n"; 


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
        echo $xml_output;
    } else {
        if (isset($_GET['username'], $_GET['password'])) {
            if (user_login($_GET['username'], $_GET['password'], $mysqli) == true) {
                $xml_output .= "<code>Success</code>";

                
            } else { 
                $xml_output .= "<code>Fail</code>";
            }

            echo $xml_output;
        }
        // The correct POST variables were not sent to this page. 
        echo 'Invalid Request';
    }
} else if (isset($_GET['action']) && $_GET['action'] == 'register') {
    if (!isset($_POST["username"]) || !isset($_POST["password"])) {
        echo "<code>missing</code>";
    } else {
        $username = $_POST["username"];
        $password = $_POST["password"];
        if (!isset($_POST["email"])) {
            $email = "";
        } else {
            $email = $_POST["email"];
        }

        $result = user_register($username, $password, $email);
        echo "<code>".$result."</code>";
    }
}
?>
