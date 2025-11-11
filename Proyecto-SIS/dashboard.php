<?php
session_start();
if (!isset($_SESSION['user_id'])) header("Location: index.php");
?>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Panel de Control</title>
  <link rel="stylesheet" href="CSS/dashboard.css">
  <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</head>
<body>
<header>
  <h1>🍋 Clasificador de Limones</h1>
  <form action="logout.php" method="POST" style="display:inline;">
    <button class="btn logout-btn" type="submit">Cerrar Sesión</button>
  </form>
</header>

<div class="dashboard-container">
  <div class="controls">
    <button class="btn" onclick="controlBanda('START')">Iniciar Banda</button>
    <button class="btn" onclick="controlBanda('STOP')">Detener Banda</button>
  </div>

  <div class="status" id="statusText">Estado: En espera...</div>

  <table>
    <thead>
      <tr>
        <th>Limones Verdes</th>
        <th>Limones Amarillos</th>
        <th>Última Detección</th>
      </tr>
    </thead>
    <tbody id="tablaDatos">
      <tr><td colspan="3">Sin registros</td></tr>
    </tbody>
  </table>
</div>

<script>
async function controlBanda(accion) {
  const res = await fetch(`api_guardar.php?action=${accion}`);
  const data = await res.json();

  const statusText = document.getElementById('statusText');
  
  if (data.status === 'success') {
    if (accion === 'START') {
      statusText.textContent = "Estado: ✅ Banda en ejecución...";
      statusText.style.color = "green";
    } else {
      statusText.textContent = "Estado: ⏸️ Banda detenida";
      statusText.style.color = "orange";
    }
  } else {
    statusText.textContent = "❌ Error: " + data.mensaje;
    statusText.style.color = "red";
  }

  cargarDatos();
}
</script>
</body>
</html>
