<?php

$dbhost = "localhost";
$dbuser = "root";
$dbpass = "";
$dbname = "limones";
$dbport = "3307";

if(!$con = mysqli_connect($dbhost,$dbuser,$dbpass,$dbname, $dbport)){
    die ("Error de conexion");
}


