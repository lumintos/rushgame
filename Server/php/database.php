<?php 

define("STATUS_OK", "1");
define("DATABASE_ERROR", "DB_ERROR");

function db_query_user($username, $mysqli) {
    if($stmt = $mysqli->prepare("SELECT id, username, password, email, score_table_id, is_online FROM RushUser WHERE username = ? AND status = ? LIMIT 1")) {
        $stmt->bind_param('si', $username, STATUS_OK);
        $stmt->execute();
        $stmt->store_result();

        $stmt->bind_result($user_id, $username, $password, $email, $is_online);

        //$user_info
    } else {
        die(DATABASE_ERROR);
    }
    //TODO: query skills and items
    $skills = db_query_user_skill($user_id, $mysqli);
    $items = db_query_user_item($user_id, $mysqli);

    $user_stat = array("id" => $user_id,
                       "username" => $username,
                       "password" => $password,
                       "email" => $email,
                       "skills" => $skills,
                       "items" => $items,
                       "is_online" => $is_online);

    return $user_stat;
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

?>
