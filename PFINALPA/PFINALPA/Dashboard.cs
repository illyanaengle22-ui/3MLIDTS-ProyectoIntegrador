using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PFINALPA
{

    public partial class Dashboard : Form
    {
        int conteoVerde = 0;
        int conteoRojo = 0;
        bool sesionActiva = true; 
        int idUsuarioActual;

        delegate void SetTextDelegate(string value);
        public SerialPort ArduinoPort { get; }
        public Dashboard(int idUsuario)
        {
            InitializeComponent();
            idUsuarioActual = idUsuario; 
            ArduinoPort = new System.IO.Ports.SerialPort();
            ArduinoPort.PortName = "COM9";
            ArduinoPort.BaudRate = 9600;
            ArduinoPort.DataBits = 8;
            ArduinoPort.WriteTimeout = 500;
            ArduinoPort.ReadTimeout = 500;
            ArduinoPort.DataReceived += ArduinoPort_DataReceived;
            ArduinoPort.Open();
        }
        private void ArduinoPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!sesionActiva) return;

            string data = ArduinoPort.ReadLine().Trim();

            if (data == "VERDE")
                conteoVerde++;

            if (data == "ROJO")
                conteoRojo++;

            // Actualiza las labels en el formulario
            this.Invoke((MethodInvoker)(() =>
            {
                lblVerde.Text = conteoVerde.ToString();
                lblRojo.Text = conteoRojo.ToString();
            }));
        }
        private void GuardarSesion()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost; Database=limones; Port=3306; Uid=root; pwd=1234"))
                {
                    conn.Open();

                    string query = "INSERT INTO fruta (verde, rojo, id_usuario) VALUES (@v, @r, @u)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@v", conteoVerde);
                        cmd.Parameters.AddWithValue("@r", conteoRojo);
                        cmd.Parameters.AddWithValue("@u", idUsuarioActual);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Sesión guardada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar sesión: " + ex.Message);
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ArduinoPort.IsOpen)
                    ArduinoPort.Write("1");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar comando: " + ex.Message);
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            try
            {
                if (ArduinoPort.IsOpen)
                    ArduinoPort.Write("0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar comando: " + ex.Message);
            }
        }
        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ArduinoPort.IsOpen)
                ArduinoPort.Close();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            sesionActiva = false;

            GuardarSesion();

            // Reiniciar contadores por si usuario vuelve a iniciar sesión
            conteoVerde = 0;
            conteoRojo = 0;
            try
            {
                if (ArduinoPort.IsOpen)
                    ArduinoPort.Write("0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar comando: " + ex.Message);
            }

            Form1 log = new Form1();
            log.Show();
            this.Hide();
        }
    }
}
