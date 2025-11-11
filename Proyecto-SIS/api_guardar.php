<?php
include('conexion.php');

$accion = $_GET['action'] ?? '';

function enviarComandoPython($comando) {
    $socket = @fsockopen('127.0.0.1', 5000, $errno, $errstr, 2);
    if ($socket) {
        fwrite($socket, $comando);
        $respuesta = fread($socket, 1024);
        fclose($socket);
        return true;
    }
    return false;
}

if($accion == 'START'){
    if (enviarComandoPython('START')) {
        echo json_encode(["status" => "success", "mensaje" => "Banda iniciada"]);
    } else {
        echo json_encode(["status" => "error", "mensaje" => "No se pudo conectar con el sistema"]);
    }
} 
elseif($accion == 'STOP'){
    if (enviarComandoPython('STOP')) {
        echo json_encode(["status" => "success", "mensaje" => "Banda detenida"]);
    } else {
        echo json_encode(["status" => "error", "mensaje" => "No se pudo conectar con el sistema"]);
    }
} 
elseif($accion == 'ver'){
    $res = mysqli_query($con, "SELECT verdes, amarillos, fecha_fin FROM sesiones ORDER BY id_sesion DESC LIMIT 1");
    $row = mysqli_fetch_assoc($res);
    echo json_encode($row ? [$row] : []);
}
?>