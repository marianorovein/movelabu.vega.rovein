using Movelabu.Clases;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Movelabu.Formularios
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            // 1) Validar que no estén vacíos
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Completá usuario y contraseña.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = Conexion.Obtener())
                {
                    con.Open();

                    // 2) Buscar el usuario con esa contraseña (encriptada)
                    string sql = "SELECT id, usuario, rol FROM usuarios " +
                                 "WHERE usuario = @usuario AND contrasena = @contrasena";

                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        // Parámetros: evitan la inyección SQL
                        cmd.Parameters.AddWithValue("@usuario", txtUsuario.Text.Trim());
                        cmd.Parameters.AddWithValue("@contrasena", Seguridad.Encriptar(txtContrasena.Text));

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // 3) Datos correctos: guardar sesión y cerrar el login con OK
                                Sesion.IdUsuario = dr.GetInt32("id");
                                Sesion.Usuario = dr.GetString("usuario");
                                Sesion.Rol = dr.GetString("rol");

                                this.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtContrasena.Clear();
                                txtContrasena.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
        }
    }
}
