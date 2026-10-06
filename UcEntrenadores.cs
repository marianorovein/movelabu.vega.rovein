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
    public partial class UcEntrenadores : UserControl
    {
        public UcEntrenadores()
        {
            InitializeComponent();
        }

        private void UcEntrenadores_Load(object sender, EventArgs e)
        {
            dgvEntrenadores.Font = new Font("Segoe UI", 10);
            dgvEntrenadores.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvEntrenadores.DefaultCellStyle.ForeColor = Color.Black;
            dgvEntrenadores.DefaultCellStyle.BackColor = Color.White;
            dgvEntrenadores.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvEntrenadores.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgvEntrenadores.DefaultCellStyle.SelectionBackColor = Color.FromArgb(95, 174, 174);
            dgvEntrenadores.DefaultCellStyle.SelectionForeColor = Color.White;

            CargarEntrenadores("");
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarEntrenadores(txtBuscar.Text.Trim());
        }

        private void CargarEntrenadores(string filtro)
        {
            string sql = "SELECT id AS ID, nombre AS Nombre, apellido AS Apellido, dni AS DNI, " +
                         "telefono AS `Teléfono`, fecha_ingreso AS `Fecha de ingreso`, cargo AS Cargo, estado AS Estado " +
                         "FROM entrenadores " +
                         "WHERE nombre LIKE @f OR apellido LIKE @f OR dni LIKE @f " +
                         "ORDER BY apellido, nombre";

            using (MySqlConnection con = Conexion.Obtener())
            {
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@f", "%" + filtro + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvEntrenadores.DataSource = dt;
            }
            AgregarBotones();
        }

        private void AgregarBotones()
        {
            if (!dgvEntrenadores.Columns.Contains("colEditar"))
            {
                DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn();
                colEditar.Name = "colEditar";
                colEditar.HeaderText = "";
                colEditar.Text = "Editar";
                colEditar.UseColumnTextForButtonValue = true;
                dgvEntrenadores.Columns.Add(colEditar);

                DataGridViewButtonColumn colEliminar = new DataGridViewButtonColumn();
                colEliminar.Name = "colEliminar";
                colEliminar.HeaderText = "";
                colEliminar.Text = "Eliminar";
                colEliminar.UseColumnTextForButtonValue = true;
                dgvEntrenadores.Columns.Add(colEliminar);
            }

            // Los botones siempre al final
            dgvEntrenadores.Columns["colEditar"].DisplayIndex = dgvEntrenadores.Columns.Count - 2;
            dgvEntrenadores.Columns["colEliminar"].DisplayIndex = dgvEntrenadores.Columns.Count - 1;

            dgvEntrenadores.Columns["Fecha de ingreso"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            AbrirFormulario(0);
        }
        private void AbrirFormulario(int id)
        {
            using (FrmEntrenador frm = new FrmEntrenador(id))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    CargarEntrenadores(txtBuscar.Text.Trim());
                }
            }
        }

        private void dgvEntrenadores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvEntrenadores.Rows[e.RowIndex];
            int id = Convert.ToInt32(fila.Cells["ID"].Value);
            string nombre = fila.Cells["Nombre"].Value + " " + fila.Cells["Apellido"].Value;

            if (dgvEntrenadores.Columns[e.ColumnIndex].Name == "colEditar")
                AbrirFormulario(id);
            else if (dgvEntrenadores.Columns[e.ColumnIndex].Name == "colEliminar")
                EliminarEntrenador(id, nombre);
        }
        private void EliminarEntrenador(int id, string nombre)
        {
            DialogResult resp = MessageBox.Show("¿Seguro que querés eliminar a " + nombre + "?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection con = Conexion.Obtener())
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM entrenadores WHERE id = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                Movimientos.Registrar("Entrenador", "Se eliminó el entrenador " + nombre);
                CargarEntrenadores(txtBuscar.Text.Trim());
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)   // tiene clientes asignados
                    MessageBox.Show("No se puede eliminar porque tiene clientes asignados.\nPodés ponerlo como Inactivo.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
