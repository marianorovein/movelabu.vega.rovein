using Movelabu.Clases;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Movelabu.Formularios
{
    public partial class FrmCliente : Form
    {
        private readonly int idCliente;   // 0 = cliente nuevo

        public FrmCliente(int id = 0)
        {
            InitializeComponent();
            idCliente = id;
        }

        private void FrmCliente_Load(object sender, EventArgs e)
        {
            cmbEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
            cmbPlan.Items.AddRange(new object[] { "Mensual", "Trimestral", "Semestral", "Anual" });
            cmbEstado.SelectedIndex = 0;
            cmbPlan.SelectedIndex = 0;

            CargarEntrenadoresCombo();

            if (idCliente > 0)
            {
                this.Text = "Editar cliente";
                CargarCliente();
            }
            else
            {
                this.Text = "Nuevo cliente";
            }
        }

        private void CargarCliente()
        {
            using (MySqlConnection con = Conexion.Obtener())
            {
                con.Open();
                string sql = "SELECT nombre, apellido, dni, telefono, fecha_ingreso, estado, tipo_plan, entrenador_id " +
                             "FROM clientes WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtNombre.Text = dr["nombre"].ToString();
                        txtApellido.Text = dr["apellido"].ToString();
                        txtDni.Text = dr["dni"].ToString();
                        txtTelefono.Text = dr["telefono"].ToString();
                        dtpFechaIngreso.Value = Convert.ToDateTime(dr["fecha_ingreso"]);
                        cmbEstado.SelectedItem = dr["estado"].ToString();
                        cmbPlan.SelectedItem = dr["tipo_plan"].ToString();
                        cmbEntrenador.SelectedValue = dr["entrenador_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["entrenador_id"]);
                    }
                }
            }
        }

        // Revisa que los datos estén bien escritos antes de guardar
        private bool Validar()
        {
            if (!Regex.IsMatch(txtNombre.Text.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{2,50}$"))
            {
                MessageBox.Show("Ingresá un nombre válido (solo letras).", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (!Regex.IsMatch(txtApellido.Text.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{2,50}$"))
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
            if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{8,15}$"))
            {
                MessageBox.Show("El teléfono debe tener entre 8 y 15 números.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return false;
            }
            return true;
        }

        // Devuelve true si otro cliente ya tiene ese DNI
        private bool DniRepetido(MySqlConnection con)
        {
            string sql = "SELECT COUNT(*) FROM clientes WHERE dni = @dni AND id <> @id";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
            cmd.Parameters.AddWithValue("@id", idCliente);
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
                        MessageBox.Show("Ya existe un cliente con ese DNI.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDni.Focus();
                        return;
                    }

                    string sql;
                    if (idCliente == 0)
                        sql = "INSERT INTO clientes (nombre, apellido, dni, telefono, fecha_ingreso, estado, tipo_plan, entrenador_id) " +
                              "VALUES (@nombre, @apellido, @dni, @telefono, @fecha, @estado, @plan, @entrenador)";
                    else
                        sql = "UPDATE clientes SET nombre=@nombre, apellido=@apellido, dni=@dni, telefono=@telefono, " +
                              "fecha_ingreso=@fecha, estado=@estado, tipo_plan=@plan, entrenador_id=@entrenador WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@dni", txtDni.Text.Trim());
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@fecha", dtpFechaIngreso.Value.Date);
                    cmd.Parameters.AddWithValue("@estado", cmbEstado.Text);
                    cmd.Parameters.AddWithValue("@plan", cmbPlan.Text);
                    int idEntrenador = Convert.ToInt32(cmbEntrenador.SelectedValue);
                    cmd.Parameters.AddWithValue("@entrenador", idEntrenador == 0 ? (object)DBNull.Value : idEntrenador);
                    cmd.Parameters.AddWithValue("@id", idCliente);
                    cmd.ExecuteNonQuery();
                }

                string nombreCompleto = txtNombre.Text.Trim() + " " + txtApellido.Text.Trim();
                if (idCliente == 0)
                    Movimientos.Registrar("Cliente", "Se agregó el cliente " + nombreCompleto);
                else
                    Movimientos.Registrar("Cliente", "Se modificó el cliente " + nombreCompleto);

                MessageBox.Show("Cliente guardado correctamente.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;   // cierra el formulario y avisa que se guardó
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;   // cierra sin guardar
        }
        // Llena la lista con los entrenadores activos
        private void CargarEntrenadoresCombo()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection con = Conexion.Obtener())
            {
                string sql = "SELECT id, CONCAT(nombre, ' ', apellido) AS nombre_completo " +
                             "FROM entrenadores WHERE estado = 'Activo' ORDER BY nombre";
                MySqlDataAdapter da = new MySqlDataAdapter(sql, con);
                da.Fill(dt);
            }

            // Primera opción: sin entrenador
            DataRow fila = dt.NewRow();
            fila["id"] = 0;
            fila["nombre_completo"] = "(Sin entrenador)";
            dt.Rows.InsertAt(fila, 0);

            cmbEntrenador.DataSource = dt;
            cmbEntrenador.DisplayMember = "nombre_completo";   // lo que se ve
            cmbEntrenador.ValueMember = "id";                  // lo que se guarda
        }
    }
}