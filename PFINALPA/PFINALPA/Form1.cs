using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace PFINALPA
{
    public partial class Form1 : Form
    {
        string ConexionSQL = "Server=localhost; Database = limones; Port = 3306; Uid = root; pwd=1234";
        public Form1()
        {
            InitializeComponent();
        }
        private void Registros(string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(ConexionSQL))
            {
                conn.Open();
                string Query = "SELECT id_usuarios,nombre,email,password FROM usuarios WHERE email = @Email AND password=@Password LIMIT 1";
                MySqlDataAdapter ada = new MySqlDataAdapter(Query, conn);
                ada.SelectCommand.Parameters.AddWithValue("@Email", email);
                ada.SelectCommand.Parameters.AddWithValue("@Password", password);

                DataTable table = new DataTable();
                ada.Fill(table);

                if(table.Rows.Count > 0) {
                    int idUsuario = Convert.ToInt32(table.Rows[0]["id_usuarios"]);
                    MessageBox.Show("Log-In exitoso!");
                    Dashboard dash = new Dashboard(idUsuario);
                    dash.Show();
                    this.Hide();
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("Datos invalidos");
                    conn.Close();
                }
              
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblRegistro_Click(object sender, EventArgs e)
        {
            SignUp sig = new SignUp();
            sig.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string contraseña = txtContraseña.Text.Trim();

            if(string.IsNullOrEmpty(email)||string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Email y contraseño son requeridos");
            }
            else
            {
                try
                {
                    Registros(email,contraseña);
                }
                catch (Exception ex) 
                {
                        MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
