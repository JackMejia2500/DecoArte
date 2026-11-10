using DecoArte.Modelos;
using System;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class PanelCentral : Form
    {
        public PanelCentral()
        {
            InitializeComponent();

            // Suscribimos el evento Load para que valide permisos apenas se abra el menú
            this.Load += PanelCentral_Load;
        }

        private void PanelCentral_Load(object sender, EventArgs e)
        {
            AplicarPermisos();
        }

        private void AplicarPermisos()
        {
            // Traemos el string de permisos del usuario logueado (ej: "Facturacion,Inventario,Usuarios")
            string permisos = SesionUsuario.Permisos ?? "";

            // Ocultamos los botones si el usuario no tiene la palabra clave en sus permisos
            // Usamos .Visible = false para que los botones prohibidos desaparezcan del panel izquierdo
            btnFactura.Visible = permisos.Contains("Facturacion");
            btnInventario.Visible = permisos.Contains("Inventario");
            btnUsuarios.Visible = permisos.Contains("Usuarios");
            btnProveedores.Visible = permisos.Contains("Proveedores");
            btnReservaciones.Visible = permisos.Contains("Reservaciones");
            btnCompraServicios.Visible = permisos.Contains("Compras y Servicios");

            // El botón de Clientes lo dejamos visible si tiene acceso a Facturar o a Reservar
            btnClientes.Visible = permisos.Contains("Facturacion") || permisos.Contains("Reservaciones");
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Limpiamos la sesión por seguridad al salir
            SesionUsuario.IdUsuario = null;
            SesionUsuario.NombreUsuario = null;
            SesionUsuario.Permisos = null;

            frm_inicio inicio = new frm_inicio();
            inicio.Show();
            this.Hide();
        }

        private void AbrirFormularioEnPanel(Form formularioHijo)
        {
            if (this.PanelContenedor.Controls.Count > 0)
            {
                this.PanelContenedor.Controls.RemoveAt(0);
            }
            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;
            this.PanelContenedor.Controls.Add(formularioHijo);
            this.PanelContenedor.Tag = formularioHijo;
            formularioHijo.Show();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Clientes());
        }

        private void btnReservaciones_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Reservaciones());
        }

        private void btnProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Proveedores());
        }

        private void btnCompraServicios_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FrmComprasServicios());
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Usuario());
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Inventario());
        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new Factura());
        }
    }
}