<?php
$cur_dir = getcwd();
include $cur_dir."/include/db/db_connect.php";
include $cur_dir."/include/db/database.php";
include $cur_dir."/include/api/skill_api.php";

header("Content-type: text/xml"); 
$xml_output = "<?xml version=\"1.0\"?>\n";
$xml_output .= "<response>"; 

/* SERVICE: Get skill by id */
if (isset($_GET['action']) && $_GET['action'] == 'query') {

    if (isset($_GET['id'])) {
        $skill_id = $_GET['id'];
        if ($skill_id == "0") {
           $skills = skill_query_all($mysqli);
        } else {
            $skills =  skill_query_by_id($skill_id, $mysqli);
        }
        if(!count($skills)) {
            $xml_output .= "<code>Skill_Not_Found</code>";
        } else {
            $xml_output .= "<code>OK</code>";
            foreach ($skills as $skill) {
                $xml_output .= "<skill>";
                $xml_output .= "<id>".$skill['id']."</id>";
                $xml_output .= "<name>".$skill['name']."</name>";
                $xml_output .= "<desc>".$skill['desc']."</desc>";
                $xml_output .= "<damage>".$skill['damage']."</damage>";
                $xml_output .= "<unconscious>".$skill['unconscious']."</unconscious>";
                $xml_output .= "</skill>";
            }
        }
    } else if (isset($_GET['user_id'])) {
        $user_id = $_GET['user_id'];
        $skills = skill_query_by_user_id($user_id, $mysqli);
        //print_r($res);
    
        if (!count($skills)) {
            $xml_output .= "<code>No_Skill_Found</code>";
        } else {
            $xml_output .= "<code>OK</code>";
            foreach ($skills as $skill) {
                $xml_output .= "<skill>";
                $xml_output .= "<id>".$skill['id']."</id>";
                $xml_output .= "<name>".$skill['name']."</name>";
                $xml_output .= "<desc>".$skill['desc']."</desc>";
                $xml_output .= "<damage>".$skill['damage']."</damage>";
                $xml_output .= "<unconscious>".$skill['unconscious']."</unconscious>";
                $xml_output .= "</skill>";
            }
        }
    } else {
        $xml_output .= "<code>Invalid_request</code>";
    }
}

$xml_output .= "</response>";
echo $xml_output;
