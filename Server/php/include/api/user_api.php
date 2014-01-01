<?php

/* Return: true/false */
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

/* Return: String OK/Bind_Param_Error/Database_Error */
function user_mark_user_online_status($username, $is_online, $mysqli) {
    $is_online = ($is_online == true ? 1 : 0);
    return db_mark_user_online_status($username, $is_online, $mysqli);
}

/* Return: String OK/Bind_Param_Error/Database_Error */
function user_logout($username, $mysqli) {
    return user_mark_user_online_status($username, false, $mysqli);
}

/* Return: String OK/User_Exists/Database_Error */
function user_register($username, $password, $email, $mysqli) {
    $user = array("username" => $username, "password" => $password, "email" => $email);
    return db_insert_user($user, $mysqli);
}

/* Return: Array User["id", "username", "email", "is_online"]; */
function user_query($username, $mysqli) {
    $user = db_query_user($username, $mysqli);
    if ($user != null) {
        unset($user["password"]);
        unset($user["score_id"]);
    }

    return $user;
}

/* Return: Array User["id", "username", "email", "skills", "score" => array("win", "lose", "spirit"), "is_online"]; */
function user_query_complete($username, $mysqli) {
    $user = db_query_user($username, $mysqli);

    //TODO: add $user["skills"] = array(), $user["items"] = array(), $user["score"] = array()

    return $user;
}
