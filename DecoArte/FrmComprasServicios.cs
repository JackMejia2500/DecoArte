using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class FrmComprasServicios : Form
    {
        public FrmComprasServicios()
        {
            InitializeComponent();
            this.Load += FrmComprasServicios_Load;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvProductocompra_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmComprasServicios_Load(object sender, EventArgs e)
        {
            // Configuramos las columnas del DataGridView si no las has creado en el diseñador
            dgvProductocompra.ColumnCount = 4;
            dgvProductocompra.Columns[0].Name = "Producto";
            dgvProductocompra.Columns[1].Name = "Cantidad";
            dgvProductocompra.Columns[2].Name = "PrecioUnitario";
            dgvProductocompra.Columns[3].Name = "Subtotal";

            // Ajustes visuales de la tabla
            dgvProductocompra.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductocompra.AllowUserToAddRows = false; // Evita la fila vacía al final

            ComprasDAO dao = new ComprasDAO();

            // Llenar Proveedores
            cmProveedor.DataSource = dao.ObtenerProveedores();
            cmProveedor.DisplayMember = "Nombre_Empresa";
            cmProveedor.ValueMember = "IdProveedor";
            cmProveedor.SelectedIndex = -1;

            // Llenar Productos
            cmProductoServicio.DataSource = dao.ObtenerProductos();
            cmProductoServicio.DisplayMember = "Detalle_Producto";
            cmProductoServicio.ValueMember = "IdProducto";
            cmProductoServicio.SelectedIndex = -1;
        }

        
        
           private void btnAgregar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();

            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(cmProductoServicio.Text))
            {
                errorProvider1.SetError(cmProductoServicio, "Selecciona un producto o servicio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                errorProvider2.SetError(txtCantidad, "Ingresa una cantidad válida.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                errorProvider3.SetError(txtPrecio, "Ingresa un precio válido.");
                hayErrores = true;
            }

            // CORRECCIÓN: Si faltan datos, detenemos la ejecución aquí mismo.
            if (hayErrores) return;

            try
            {
                DataRowView filaSeleccionada = (DataRowView)cmProductoServicio.SelectedItem;
                string idProducto = filaSeleccionada["IdProducto"].ToString();

                string categoria = "Fisico";
                if (filaSeleccionada.Row.Table.Columns.Contains("Categoria"))
                {
                    string valorSQL = filaSeleccionada["Categoria"].ToString();
                    if (!string.IsNullOrWhiteSpace(valorSQL))
                    {
                        categoria = valorSQL;
                    }
                }

                string nombreProducto = cmProductoServicio.Text;
                decimal cantidad = Convert.ToDecimal(txtCantidad.Text);
                decimal precio = Convert.ToDecimal(txtPrecio.Text);
                decimal subtotal = cantidad * precio;

                int indiceFila = dgvProductocompra.Rows.Add(nombreProducto, cantidad, precio, subtotal);
                dgvProductocompra.Rows[indiceFila].Tag = $"{idProducto}|{categoria}";

                CalcularTotalCompra();

                cmProductoServicio.SelectedIndex = -1;
                txtCantidad.Clear();
                txtPrecio.Clear();
                cmProductoServicio.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProcesarCompra_Click(object sender, EventArgs e)
        {
            // CORRECCIÓN: Separamos las validaciones para que te avise de una por una
            if (string.IsNullOrWhiteSpace(txtIdCompra.Text))
            {
                MessageBox.Show("Por favor, ingresa un ID para registrar la compra.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIdCompra.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(cmProveedor.Text))
            {
                MessageBox.Show("Por favor, selecciona un proveedor para registrar la compra.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmProveedor.Focus();
                return;
            }

            if (dgvProductocompra.Rows.Count == 0)
            {
                MessageBox.Show("Por favor, agrega al menos un producto para registrar la compra.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ComprasDAO dao = new ComprasDAO();
                string idCompra = txtIdCompra.Text;
                string proveedor = cmProveedor.SelectedValue?.ToString() ?? "";

                var listaDetalles = new List<(string, decimal, decimal, decimal, string)>();

                foreach (DataGridViewRow fila in dgvProductocompra.Rows)
                {
                    string[] datosOcultos = fila.Tag.ToString().Split('|');
                    string prodId = datosOcultos[0];
                    string categoria = datosOcultos[1];

                    decimal cant = Convert.ToDecimal(fila.Cells["Cantidad"].Value);
                    decimal prec = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value);
                    decimal sub = Convert.ToDecimal(fila.Cells["Subtotal"].Value);

                    listaDetalles.Add((prodId, cant, prec, sub, categoria));
                }

                if (dao.GuardarCompraTransaccion(idCompra, proveedor, dtpFecha.Value, Convert.ToDecimal(txtTotal.Text), listaDetalles))
                {
                    MessageBox.Show("Compra registrada con éxito.");
                    dgvProductocompra.Rows.Clear();
                    txtIdCompra.Clear();
                    txtTotal.Clear();
                    cmProveedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
            
        

        private void CalcularTotalCompra()
        {
            decimal sumaTotal = 0;

            foreach (DataGridViewRow fila in dgvProductocompra.Rows)
            {
                // La validación nula ayuda a evitar las advertencias amarillas (CS8600)
                if (fila.Cells["Subtotal"].Value != null)
                {
                    sumaTotal += Convert.ToDecimal(fila.Cells["Subtotal"].Value);
                }
            }

            txtTotal.Text = sumaTotal.ToString("0.00");
        }

       
        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            FrmNuevoProducto frm = new FrmNuevoProducto();

            // Si el usuario guardó con éxito (OK), recargamos el ComboBox
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ComprasDAO dao = new ComprasDAO();
                cmProductoServicio.DataSource = dao.ObtenerProductos();

                // Muestra el nombre, pero oculta el ID (ajusta "Detalle_Producto" si tu columna se llama diferente)
                cmProductoServicio.DisplayMember = "Detalle_Producto";
                cmProductoServicio.ValueMember = "IdProducto";
                cmProductoServicio.SelectedIndex = -1;
            }
        }
    }

}