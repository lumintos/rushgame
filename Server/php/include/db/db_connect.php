<?php
define("HOST", "localhost");     // The host you want to connect to.
define("USER", "root");    // The database username. 
define("PASSWORD", "abc");    // The database password. 
define("DATABASE", "rush");    // The database name.
$mysqli = new mysqli(HOST, USER, PASSWORD, DATABASE);
