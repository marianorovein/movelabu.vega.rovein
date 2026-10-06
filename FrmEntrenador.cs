using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Movelabu.Clases;

namespace Movelabu.Formularios
{
    public partial class FrmEntrenador : Form
    {
        private readonly int idEntrenador;   // 0 = entrenador nuevo

        public FrmEntrenador(int id = 0)
        {
            InitializeComponent();
            idEntrenador = id;
        }

        private void FrmEntrenador_Load(object sender, EventArgs e)
        {
            cmbCargo.Items.AddRange(new object[] { "Entrenador personal", "Instructor de sala", "Profesor de clases", "Coordinador" });
            cmbEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
            cmbCargo.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;

            if (idEntrenador > 0)
            {
                this.Text = "Editar entrenador";
                CargarEntrenador();
            }
            else
            {
                this.Text = "Nuevo entrenador";
            }
        }
        private void CargarEntrenador()
        {
            using (MySqlConnection con = Conexion.Obtener())
            {
                con.Open();
                string sql = "SELECT nombre, apellido, dni, telefono, fecha_ingreso, cargo, estado " +
                             "FROM entrenadores WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idEntrenador);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtNombre.Text = dr["nombre"].ToString();
                        txtApellido.Text = dr["apellido"].ToString();
                        txtDni.Text = dr["dni"].ToString();
                        txtTelefono.Text = dr["telefono"].ToString();
                        dtpFechaIngreso.Value = Convert.ToDateTime(dr["fecha_ingreso"]);
                        cmbCargo.SelectedItem = dr["cargo"].ToString();
                        cmbEstado.SelectedItem = dr["estado"].ToString();
                    }
                }
            }
        }

        private bool Validar()
        {
            if (!Regex.IsMatch(txtNombre.Text.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{2,60}$"))
            {
                MessageBox.Show("Ingresá un nombre válido (solo letras).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtApellido.Text.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{2,60}$"))
            {
                MessageBox.Show("Ingresá un apellido válido (solo letras).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtDni.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show("El DNI debe tener 7 u 8 números.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }
            // El teléfono es opcional: solo se revisa si escribieron algo
            if (txtTelefono.Text.Trim() != "" && !Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{8,15}$"))
            {
                MessageBox.Show("El teléfono debe tener entre 8 y 15 números.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }
            return true;
        }

        private bool DniRepetido(MySqlConnection con)
        {
            string sql = "SELECT COUNT(*) FROM entrenadores WHERE dni = @dni AND id <> @id";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
            cmd.Parameters.AddWithValue("@id", idEntrenador);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            try
            {
                using (MySqlConnection con = Conexion.Obtener())
                {
                    con.Open();

                    if (DniRepetido(con))
                    {
                        MessageBox.Show("Ya existe un entrenador con ese DNI.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDni.Focus();
                        return;
                    }

                    string sql;
                    if (idEntrenador == 0)
                        sql = "INSERT INTO entrenadores (nombre, apellido, dni, telefono, fecha_ingreso, cargo, estado) " +
                              "VALUES (@nombre, @apellido, @dni, @telefono, @fecha, @cargo, @estado)";
                    else
                        sql = "UPDATE entrenadores SET nombre=@nombre, apellido=@apellido, dni=@dni, telefono=@telefono, " +
                              "fecha_ingreso=@fecha, cargo=@cargo, estado=@estado WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                    // Si el teléfono está vacío se guarda NULL
                    cmd.Parameters.AddWithValue("@telefono",
                        txtTelefono.Text.Trim() == "" ? (object)DBNull.Value : txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@fecha", dtpFechaIngreso.Value.Date);
                    cmd.Parameters.AddWithValue("@cargo", cmbCargo.Text);
                    cmd.Parameters.AddWithValue("@estado", cmbEstado.Text);
                    cmd.Parameters.AddWithValue("@id", idEntrenador);
                    cmd.ExecuteNonQuery();
                }

                string nombreCompleto = txtNombre.Text.Trim() + " " + txtApellido.Text.Trim();
                if (idEntrenador == 0)
                    Movimientos.Registrar("Entrenador", "Se agregó el entrenador " + nombreCompleto);
                else
                    Movimientos.Registrar("Entrenador", "Se modificó el entrenador " + nombreCompleto);

                MessageBox.Show("Entrenador guardado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
