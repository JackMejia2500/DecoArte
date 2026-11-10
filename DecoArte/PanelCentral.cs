using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class PanelCentral : Form
    {
     

        public PanelCentral()
        {
            InitializeComponent();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
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
