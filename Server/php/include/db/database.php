<?php 

define("STATUS_OK", 1);
define("DATABASE_ERROR", "DB_ERROR");

function db_query_user($username, $mysqli) {
    //if ($stmt = $mysqli->prepare("SELECT id, username, password FROM RushUser WHERE username = ? LIMIT 1")) {
    if($stmt = $mysqli->prepare("SELECT id, username, password, email, score_table_id, is_online, status FROM RushUser WHERE username =? AND status =1 LIMIT 1")) {
        $stmt->bind_param("s", $username);
        $stmt->execute();
        $stmt->store_result();

        $stmt->bind_result($user_id, $username, $password, $email, $score_table_id, $is_online, $status);
        $stmt->fetch();
    } else {
        die(DATABASE_ERROR);
    }
    //TODO: query skills and items
    //$skills = db_query_user_skill($user_id, $mysqli);
    //$items = db_query_user_item($user_id, $mysqli);

    if ($stmt->num_rows == 0) {
        return null;
    }

    $user_stat = array("id" => $user_id,
                       "username" => $username,
                       "password" => $password,
                       "email" => $email,
                       "score" => $score_table_id,
                       //"items" => $items,
                       "is_online" => $is_online);

    return $user_stat;
}

function db_insert_user($user, $mysqli) {
    //if user exists
    if (db_query_user($user['username'], $mysqli) != null) {
        return "User_Exists";
    }

    $query = "INSERT INTO RushUser(username,password,email) VALUES (?,?,?)";

    if($stmt = $mysqli->prepare($query)) {
        //die("before");
        $stmt->bind_param("sss", $user['username'], hash('sha512', $user['password']), $user['email']);
        //die("OK");
        if($stmt->execute()) {
            return "OK";
        } else {
            return "Database_Error";
        }
    } else {
        return "Database_Query_Error";
    }
}

function db_query_user_skill($user_id, $mysqli) {
    $skills = array();
    if($stmt = $mysqli->prepare("SELECT id, user_id, skill_id FROM RushUserSkill WHERE user_id = ? AND status = ?")) {
        $stmt->bind_param('ii', $user_id, STATUS_OK);
        $stmt->execute();
        $stmt->store_result();

        $stmt->bind_result($user_id, $skill_id);
        $stml->fetch_all();
        //foreach skill_id, query names
        //return list of skills

        $stmt = $mysqli->prepare("SELECT id, skill_name, desc FROM RushUserSkill WHERE id = ? AND status = ? LIMIT 1");

        foreach ($skill_id as $si) {

            $stmt->bind_param('ii', $si, STATUS_OK);
            $stmt->execute();
            $stmt->store_result();

            $stmt->bind_result($si, $skill_name, $desc);
            $stmt->fetch();
            $skills[] = array("id" => $si, "name" => $skill_name, "desc" => $desc);
        }

    } else {
        die("database error");
    }

    return $skills;
}

function db_query_user_item($user_id, $mysqli) {
    return null;
}

function db_mark_user_online_status($username, $is_online, $mysqli) {
    $query = "UPDATE RushUser SET is_online=? WHERE username=?";
    if ($stmt = $mysqli->prepare($query)) {
        if(!$stmt->bind_param('is', $is_online, $username)) {
            //die("Bind_Param_Error");
            return "Bind_Param_Error";
        }
        if($stmt->execute()) {
            return "OK";
        }
        return "Database_Error";
    }

    return "Database_Error";
}
