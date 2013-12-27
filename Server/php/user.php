<?php
include "db_connect.php";
include "functions.php";

if (isset($_GET['action']) && $_GET['action'] == 'login') {
    if (isset($_POST['username'], $_POST['password'])) {
        $username = $_POST['username'];
        $password = $_POST['password']; // The hashed password.
        #$username="test";
        #$password="test";
     
        if (login($username, $password, $mysqli) == true) {
            // Login success 
            echo "Success";
        } else {
            // Login failed 
            echo "Fail";
        }
    } else {
        // The correct POST variables were not sent to this page. 
        echo 'Invalid Request';
    }
}
?>
