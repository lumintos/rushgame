<?php

/* Return: Array skill[0] = [id, name, desc, damage, unconscious] */
function skill_query_by_id($skill_id, $mysqli) {    
    $res = db_query_skill_by_id($skill_id, $mysqli);

    if ($res != null) {
        return array('0' => $res);
    }
    return $res;
}

/* Return: Numeric array of skills; skill[0] = [id, name, desc, damage, unconscious] */ 
function skill_query_all($mysqli) {
    return db_query_all_skill($mysqli);
}

/* Return: Numeric array of skills; skill[0] = [id, name, desc, damage, unconscious] */ 
function skill_query_by_user_id($user_id, $mysqli) {
    $skills = db_query_user_skill($user_id, $mysqli);

    if (!count($skills)) {
        return null;
    } else {
        $result = array();
        foreach ($skills as $skill) {
            $result[] = db_query_skill_by_id($skill["skill_id"], $mysqli);
        }
        return $result;
    }
}
