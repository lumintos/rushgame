<?php
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

function user_register($username, $password, $email, $mysqli) {
    $user = array("username" => $username, "password" => $password, "email" => $email);
    return db_insert_user($user, $mysqli);
    
}

function user_query($username, $mysqli) {
    return db_query_user($username, $mysqli);
}

function user_mark_user_online_status($username, $is_online, $mysqli) {
    $is_online = ($is_online == true ? 1 : 0);
    return db_mark_user_online_status($username, $is_online, $mysqli);
}
