using Movelabu.Clases;
using Movelabu.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Movelabu
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = Sesion.Rol;   // muestra "Administrador" (el rol del que inició sesión)
            MarcarBoton(btnInicio);
            Movimientos.Registrar("Sesión", Sesion.Usuario + " inició sesión");
            MostrarPantalla(new UcInicio());
        }
        // Pinta de verde oscuro el botón activo y deja los demás en blanco
        private void MarcarBoton(Button activo)
        {
            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button b)
                    b.ForeColor = Color.White;
            }
            activo.ForeColor = Color.FromArgb(0, 107, 107);
        }

        // Muestra una pantalla adentro de panelContenido
        private void MostrarPantalla(UserControl pantalla)
        {
            panelContenido.Controls.Clear();
            pantalla.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(pantalla);
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            MarcarBoton(btnInicio);
            MostrarPantalla(new UcInicio());
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            MarcarBoton(btnClientes);
            MostrarPantalla(new UcClientes());
        }

        private void btnCuotas_Click(object sender, EventArgs e)
        {
            MarcarBoton(btnCuotas);
        }

        private void btnPagos_Click(object sender, EventArgs e)
        {
            MarcarBoton(btnPagos);
        }

        private void btnEntrenadores_Click(object sender, EventArgs e)
        {
            MarcarBoton(btnEntrenadores);
            MostrarPantalla(new UcEntrenadores());
        }
    }
}
