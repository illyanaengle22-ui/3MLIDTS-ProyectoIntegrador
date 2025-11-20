using MySql.Data.MySqlClient;
using Mysqlx.Connection;
using Mysqlx.Cursor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFINALPA
{
    public partial class SignUp : Form
    {
        string ConexionSQL = "Server=localhost; Database = limones; Port = 3306; Uid = root; pwd=1234";
        public SignUp()
        {
            InitializeComponent();
        }
        private void Registros(string nombre, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(ConexionSQL))
            {
                conn.Open();
                string Query = "INSERT INTO usuarios (nombre,email,password) Values (@Nombre, @Email, @Password)";

                using (MySqlCommand cmd = new MySqlCommand(Query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
 

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        private void SignUp_Load(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Close();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string email = txtEmail.Text.Trim();
            string contraseña = txtContraseña.Text.Trim();

            if(string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña) )
            {
                MessageBox.Show("Todos los campos son requeridos");
                return;
            }
            else
            {
                try
                {

                    string dato = $"Nombre: {nombre} \r\n Email: {email} \r\n Password C: {contraseña}";
                    MessageBox.Show("Registro exitoso!");
                    Registros(nombre, email, contraseña);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void lblRegistro_Click(object sender, EventArgs e)
        {
            Form1 log = new Form1();
            log.Show();
            this.Close();
        }
    }
}
