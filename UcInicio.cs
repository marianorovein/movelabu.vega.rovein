using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Movelabu.Clases;

namespace Movelabu.Formularios
{
    public partial class UcInicio : UserControl
    {
        public UcInicio()
        {
            InitializeComponent();
        }

        private void UcInicio_Load(object sender, EventArgs e)
        {
            // Mismo estilo que la tabla de clientes
            dgvMovimientos.Font = new Font("Segoe UI", 10);
            dgvMovimientos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMovimientos.DefaultCellStyle.ForeColor = Color.Black;
            dgvMovimientos.DefaultCellStyle.BackColor = Color.White;
            dgvMovimientos.DefaultCellStyle.SelectionBackColor = Color.FromArgb(95, 174, 174);
            dgvMovimientos.DefaultCellStyle.SelectionForeColor = Color.White;

            try
            {
                var lista = Movimientos.Ultimos(10)
                    .Select(m => new
                    {
                        Fecha = m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        m.Tipo,
                        Descripción = m.Descripcion,
                        m.Usuario
                    })
                    .ToList();

                dgvMovimientos.DataSource = lista;
            }
            catch (Exception)
            {
                lblTitulo.Text = "Últimos movimientos (no se pudo conectar a MongoDB)";
            }
        }
    }
}
