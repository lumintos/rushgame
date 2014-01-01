<?php
//include "database.php";

function user_login($username, $password, $mysqli) {
    // Using prepared statements means that SQL injection is not possible. 
    if ($stmt = $mysqli->prepare("SELECT id, username, password FROM RushUser WHERE username = ? LIMIT 1")) {
        $stmt->bind_param('s', $username);  // Bind "$username" to parameter.
        $stmt->execute();    // Execute the prepared query.
        $stmt->store_result();
 
        // get variables from result.
        $stmt->bind_result($user_id, $username, $db_password);
        $stmt->fetch();

        // hash the password with the unique salt.
        $password = hash('sha512', $password);
        if ($stmt->num_rows == 1) {
            if ($db_password == $password) {
                return true;
           } else {
                return false;
           }
        } else {
            // No user exists.
            return false;
        }
    }
}

function user_register($username, $password, $email) {
    return 1;
}
