<?php
function user_login($username, $password, $mysqli) {
    $user = db_query_user($username, $mysqli);
    if ($user == null) { return false; }
    $password = hash('sha512', $password);
    if ($password == $user['password']) {
        user_mark_user_online_status($username, true, $mysqli);
        return true;
    } else {
        return false;
    }
}

function user_mark_user_online_status($username, $is_online, $mysqli) {
    $is_online = ($is_online == true ? 1 : 0);
    return db_mark_user_online_status($username, $is_online, $mysqli);
}

function user_logout($username, $mysqli) {
    return user_mark_user_online_status($username, false, $mysqli);
}

function user_register($username, $password, $email, $mysqli) {
    $user = array("username" => $username, "password" => $password, "email" => $email);
    return db_insert_user($user, $mysqli);
    
}

function user_query($username, $mysqli) {
    return db_query_user($username, $mysqli);
}

