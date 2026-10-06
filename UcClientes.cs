using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Movelabu.Clases;

namespace Movelabu.Formularios
{
    public partial class UcClientes : UserControl
    {
        public UcClientes()
        {
            InitializeComponent();
        }

        private void UcClientes_Load(object sender, EventArgs e)
        {
            // Estilo de la tabla: letra más chica y texto negro en todas las filas
            dgvClientes.Font = new Font("Segoe UI", 10);
            dgvClientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClientes.DefaultCellStyle.ForeColor = Color.Black;
            dgvClientes.DefaultCellStyle.BackColor = Color.White;
            dgvClientes.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvClientes.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvClientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(95, 174, 174);
            dgvClientes.DefaultCellStyle.SelectionForeColor = Color.White;

            CargarClientes("");
        
        CargarClientes("");
        }
        // Trae los clientes de la base y los muestra en la tabla ("" = todos)
        private void CargarClientes(string filtro)
        {
            string sql = "SELECT c.id AS ID, c.nombre AS Nombre, c.apellido AS Apellido, c.dni AS DNI, " +
             "c.telefono AS `Teléfono`, c.fecha_ingreso AS `Fecha de ingreso`, c.estado AS Estado, " +
             "c.tipo_plan AS `Tipo de plan`, " +
             "IFNULL(CONCAT(e.nombre, ' ', e.apellido), '-') AS Entrenador " +
             "FROM clientes c " +
             "LEFT JOIN entrenadores e ON c.entrenador_id = e.id " +
             "WHERE c.nombre LIKE @f OR c.apellido LIKE @f OR c.dni LIKE @f " +
             "ORDER BY c.apellido, c.nombre";
            try
            {
                using (var con = Conexion.Obtener())
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@f", "%" + filtro + "%");
                    DataTable tabla = new DataTable();
                    new MySqlDataAdapter(cmd).Fill(tabla);
                    dgvClientes.DataSource = tabla;
                }
                AgregarBotones();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message);
            }
        }

        // Columnas con los botones Editar y Eliminar
        private void AgregarBotones()
        {
            if (!dgvClientes.Columns.Contains("colEditar"))
            {
                dgvClientes.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "colEditar",
                    HeaderText = "",
                    Text = "Editar",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 80
                });
                dgvClientes.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "colEliminar",
                    HeaderText = "",
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 80
                });
            }
            dgvClientes.Columns["colEditar"].DisplayIndex = dgvClientes.Columns.Count - 2;
            dgvClientes.Columns["colEliminar"].DisplayIndex = dgvClientes.Columns.Count - 1;
            dgvClientes.Columns["Fecha de ingreso"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarClientes(txtBuscar.Text.Trim());
        }

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;   // si tocó el encabezado, no hace nada

            DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];
            int id = Convert.ToInt32(fila.Cells["ID"].Value);
            string nombre = fila.Cells["Nombre"].Value + " " + fila.Cells["Apellido"].Value;

            if (dgvClientes.Columns[e.ColumnIndex].Name == "colEditar")
                AbrirFormulario(id);
            else if (dgvClientes.Columns[e.ColumnIndex].Name == "colEliminar")
                EliminarCliente(id, nombre);
        }

        // Abre FrmCliente: id 0 = nuevo, otro número = editar ese cliente
        private void AbrirFormulario(int id)
        {
            using (FrmCliente frm = new FrmCliente(id))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarClientes(txtBuscar.Text.Trim());   // refresca la tabla
                }
            }
        }

        private void EliminarCliente(int id, string nombre)
        {
            DialogResult resp = MessageBox.Show("¿Seguro que querés eliminar a " + nombre + "?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection con = Conexion.Obtener())
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM clientes WHERE id = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                Movimientos.Registrar("Cliente", "Se eliminó el cliente " + nombre);
                CargarClientes(txtBuscar.Text.Trim());
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)   // tiene cuotas o pagos asociados
                    MessageBox.Show("No se puede eliminar porque tiene cuotas o pagos cargados.\nPodés ponerlo como Inactivo.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            AbrirFormulario(0);
        }
    }
}
