using DecoArte.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class FrmComprasServicios : Form
    {
        private ComprasDAO dao = new ComprasDAO();

        public FrmComprasServicios()
        {
            InitializeComponent();
        }

        private void FrmComprasServicios_Load(object sender, EventArgs e)
        {
            ConfigurarColumnasDataGrid();
            CargarDatosIniciales();

            // Bloqueamos ID y Total para evitar manipulaciones manuales
            txtIdCompra.ReadOnly = true;
            txtTotal.ReadOnly = true;
        }

        private void ConfigurarColumnasDataGrid()
        {
            dgvProductocompra.Columns.Clear();
            dgvProductocompra.Columns.Add("Producto", "Producto");
            dgvProductocompra.Columns.Add("Cantidad", "Cantidad");
            dgvProductocompra.Columns.Add("PrecioUnitario", "Precio Unit.");
            dgvProductocompra.Columns.Add("Subtotal", "Subtotal");

            dgvProductocompra.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductocompra.MultiSelect = false;
            dgvProductocompra.AllowUserToAddRows = false;
            dgvProductocompra.ReadOnly = true;
        }

        private void CargarDatosIniciales()
        {
            try
            {
                txtIdCompra.Text = dao.GenerarIdCompra();

                cmProveedor.DataSource = dao.ObtenerProveedores();
                cmProveedor.DisplayMember = "Nombre_Empresa";
                cmProveedor.ValueMember = "IdProveedor";
                cmProveedor.SelectedIndex = -1;

                cmProductoServicio.DataSource = dao.ObtenerProductos();
                cmProductoServicio.DisplayMember = "Detalle_Producto";
                cmProductoServicio.ValueMember = "IdProducto";
                cmProductoServicio.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al Cargar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón verde "+" grande para agregar fila
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool hayErrores = false;

            if (cmProductoServicio.SelectedIndex == -1)
            {
                errorProvider1.SetError(cmProductoServicio, "Selecciona un producto o servicio.");
                hayErrores = true;
            }
            if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad) || cantidad <= 0)
            {
                errorProvider1.SetError(txtCantidad, "Ingresa una cantidad válida mayor a 0.");
                hayErrores = true;
            }
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 0)
            {
                errorProvider1.SetError(txtPrecio, "Ingresa un precio válido.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                DataRowView filaSeleccionada = (DataRowView)cmProductoServicio.SelectedItem;
                string idProducto = filaSeleccionada["IdProducto"].ToString();
                string nombreProducto = cmProductoServicio.Text;

                string categoria = "PRODUCTO";
                if (filaSeleccionada.Row.Table.Columns.Contains("Categoria"))
                {
                    categoria = filaSeleccionada["Categoria"].ToString();
                }

                decimal subtotal = cantidad * precio;

                int indiceFila = dgvProductocompra.Rows.Add(nombreProducto, cantidad, precio.ToString("0.00"), subtotal.ToString("0.00"));
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

        // NUEVO: Botón para eliminar fila del DGV
        private void btnEliminarFila_Click(object sender, EventArgs e)
        {
            if (dgvProductocompra.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una fila para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Desea quitar este producto de la compra?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dgvProductocompra.Rows.RemoveAt(dgvProductocompra.SelectedRows[0].Index);
                CalcularTotalCompra();
            }
        }

        private void CalcularTotalCompra()
        {
            decimal sumaTotal = 0;
            foreach (DataGridViewRow fila in dgvProductocompra.Rows)
            {
                if (fila.Cells["Subtotal"].Value != null)
                {
                    sumaTotal += Convert.ToDecimal(fila.Cells["Subtotal"].Value);
                }
            }
            txtTotal.Text = sumaTotal.ToString("0.00");
        }

        private void btnProcesarCompra_Click(object sender, EventArgs e)
        {
            if (cmProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, selecciona un proveedor.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmProveedor.Focus();
                return;
            }

            if (dgvProductocompra.Rows.Count == 0)
            {
                MessageBox.Show("Agrega al menos un producto a la lista.", "Dato requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string idCompra = txtIdCompra.Text;
                string proveedor = cmProveedor.SelectedValue.ToString();
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
                    MessageBox.Show("Compra registrada y stock actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dgvProductocompra.Rows.Clear();
                    txtTotal.Clear();
                    CargarDatosIniciales(); // Recarga combos y genera nuevo ID (COM-0002)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón verde "+" pequeño para Crear Producto Nuevo
        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            FrmNuevoProducto frm = new FrmNuevoProducto();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Solo recargamos los productos si se guardó uno nuevo
                cmProductoServicio.DataSource = dao.ObtenerProductos();
                cmProductoServicio.DisplayMember = "Detalle_Producto";
                cmProductoServicio.ValueMember = "IdProducto";
                cmProductoServicio.SelectedIndex = -1;
            }
        }

        // NUEVO: Botón Historico para abrir la nueva pantalla
        private void btnHistorico_Click(object sender, EventArgs e)
        {
            Historico_Compras_y_Servicios formHistorico = new Historico_Compras_y_Servicios();
            formHistorico.ShowDialog();
        }

        
    }
}