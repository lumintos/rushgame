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
    $res = db_insert_user($user, $mysqli);

    if($res == "OK") {
        $user = db_query_user($username, $mysqli);
        $user_id = $user["id"];
        
        $score = array ("user_id" => $user_id, 
                        "win" => 0, 
                        "lose" => 0, 
                        "spirit"=> 100, 
                        "max_spirit" => 100);
        return db_insert_user_score($score, $mysqli);
    } else {
        return $res;
    }
}

/* Return: Array User["id", "username", "email", "is_online"]; */
function user_query($username, $mysqli) {
    $user = db_query_user($username, $mysqli);
    if ($user != null) {
        unset($user["password"]);
        //unset($user["score_id"]);
    }

    return $user;
}

/* Return: Array score['user_id', 'win', 'lose', 'spirit', 'max_spirit'] */
function user_query_user_score_by_id($user_id, $mysqli) {
    return db_query_user_score($user_id, $mysqli);
}

/* Return: Array User["id", "username", "email", "skills", "score" => array("win", "lose", "spirit"), "is_online"]; */
function user_query_complete($username, $mysqli) {
    $user = db_query_user($username, $mysqli);

    //TODO: add $user["skills"] = array(), $user["items"] = array(), $user["score"] = array()

    return $user;
}

