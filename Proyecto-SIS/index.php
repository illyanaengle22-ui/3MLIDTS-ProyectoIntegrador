<?php
session_start();
include("conexion.php");
include("funciones.php");

$mensaje = "";

if($_SERVER['REQUEST_METHOD'] == "POST"){

 
    $user_email=$_POST['email'] ?? '';
    $password=$_POST['password'] ?? '';

    if(!empty($user_email) && !empty($password)){
          
        $query = "SELECT * FROM usuarios WHERE email = '$user_email' limit 1";
        
        $result = mysqli_query($con,$query);
       
        if($result){
               if($result && mysqli_num_rows($result)>0){

                $user_data = mysqli_fetch_assoc($result);
                
                if($user_data['password'] == $password){
                    $_SESSION['user_id'] = $user_data['user_id'];
                    header("Location: dashboard.php ");
                        die;
                }

    }
        }
        $mensaje= "<script>
                Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Usuario o contraseña incorrectos',
                confirmButtonColor: '#3085d6'
                });
                </script>";
    }
    else
    {
       $mensaje= "<script>
        Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Usuario o contraseña incorrectos',
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
    <form action="" method="post">

        <div class="login-container">
            <h1>LOGIN</h1>
            
        <div class="input-group">
            <label for="email">EMAIL</label>
            <input type="email" id="email" placeholder="Tu@email.com" name="email">
        </div>
        
        <div class="input-group">
            <label for="password">CONTRASEÑA</label>
            <input type="password" id="password" placeholder="••••••••" name="password">
        </div>
        
        <button type="submit">LOGIN</button>
        
        <div class="divider">O</div>
        
        <div class="social-login">
            
            </div>
            
            <div class="footer">
                ¿NO TIENES CUENTA? <a href="Registro.php">Sign up</a>
            </div>
        </div>
    </form>
    <?php if(!empty($mensaje)) echo $mensaje; ?>

</body>
</html>

