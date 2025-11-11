
<?php
session_start();
include("conexion.php");
include("funciones.php");
$mensaje = "";


if($_SERVER['REQUEST_METHOD'] == "POST"){

    $user_name = $_POST['name'];
    $user_email=$_POST['email'];
    $password=$_POST['password'];

    if(!empty($user_name) && !empty($user_email) && !empty($password) && !is_numeric($user_name)){
         $check_query = "SELECT * FROM usuarios WHERE email = '$user_email' LIMIT 1";
        $check_result = mysqli_query($con, $check_query);

        if($check_result && mysqli_num_rows($check_result) > 0){
            // 🔸 El correo ya está registrado
            $mensaje = "<script>
                Swal.fire({
                    icon: 'warning',
                    title: 'Correo ya registrado',
                    text: 'El email ingresado ya está en uso. Intenta con otro o inicia sesión.',
                    confirmButtonColor: '#3085d6'
                });
            </script>";
        }else{

            $user_id = random_num(20);
            $query = "INSERT INTO usuarios (user_id, nombre, email, password) values ('$user_id','$user_name','$user_email','$password')";
            
            mysqli_query($con,$query);
            
            header("Location: index.php ");
            die;
        }
    }
    else
    {
         $mensaje= "<script>
                Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Ingrese informacion valida',
                confirmButtonColor: '#3085d6'
                });
                </script>";
    }

}

?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="CSS/index.css">
    <title>Index</title>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

</head>
<body>
    <form action="" method = "POST">

        <div class="login-container">
            <h1>SIGN-UP</h1>
            <div class="input-group">
                <label for="name">NOMBRE</label>
                <input type="name" id="name" name="name" placeholder="Tu nombre">
            </div>
            <div class="input-group">
                <label for="email">EMAIL</label>
                <input type="email" id="email" name="email" placeholder="Tu@email.com">
            </div>
            
            <div class="input-group">
                <label for="password">CONTRASEÑA</label>
                <input type="password" id="password" name="password" placeholder="••••••••">
            </div>
            
            <button type="submit">SIGN IN</button>
            
            <div class="divider">O</div>
            
            <div class="social-login">
                
                </div>
                
                <div class="footer">
                    ¿Ya tienes cuenta? <a href="index.php">Login</a>
                </div>
            </div>
        </form>
        <?php if(!empty($mensaje)) echo $mensaje; ?>

</body>
</html>