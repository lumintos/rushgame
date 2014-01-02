<?php

/* Return: Array skill[0] = [id, name, desc, damage, unconscious] */
function skill_query_by_id($skill_id, $mysqli) {    
    $res = db_query_skill_by_id($skill_id, $mysqli);

    if ($res != null) {
        return array('0' => $res);
    }
    return $res;
}

function skill_query_all($mysqli) {
    return db_query_all_skill($mysqli);
}
