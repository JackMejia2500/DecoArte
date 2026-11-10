using DecoArte.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Factura : Form
    {
        private FacturaDAO dao = new FacturaDAO();
        private decimal precioUnitarioBase = 0; // Guarda el precio de 1 unidad para multiplicar en vivo
        private decimal stockDisponible = 0;

        public Factura()
        {
            InitializeComponent();
        }

        private void Factura_Load(object sender, EventArgs e)
        {
            ConfigurarControlesIniciales();
            CargarCombosYCorrelativo();
        }

        private void ConfigurarControlesIniciales()
        {
            // Bloquear campos que no deben modificarse
            txtIdFactura.ReadOnly = true;
            txtUsuario.ReadOnly = true;
            txtTelefono.ReadOnly = true;
            txtPrecioUnitario.ReadOnly = true;
            txtSubtotal.ReadOnly = true;
            txtTotal.ReadOnly = true;

            // Configurar columnas del DataGridView si no están creadas
            dgvFactura.Columns.Clear();
            dgvFactura.Columns.Add("IdProducto", "ID Producto");
            dgvFactura.Columns.Add("Producto", "Producto");
            dgvFactura.Columns.Add("Cantidad", "Cantidad");
            dgvFactura.Columns.Add("PrecioUnitario", "Precio Unit.");
            dgvFactura.Columns.Add("Subtotal", "Subtotal");
            dgvFactura.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFactura.MultiSelect = false;
            dgvFactura.AllowUserToAddRows = false;
            dgvFactura.ReadOnly = true;

            // Cargar descuentos por defecto (0, 5, 8, 10, 12, 15, 20, 25 %)
            cmbDescuento.Items.Clear();
            cmbDescuento.Items.AddRange(new object[] { "0%", "5%", "8%", "10%", "12%", "15%", "20%", "25%" });
            cmbDescuento.SelectedIndex = 0;
        }

        private void CargarCombosYCorrelativo()
        {
            try
            {
                // 1. Generar ID Factura secuencial (FAC-0001)
                txtIdFactura.Text = dao.GenerarIdFactura();

                // 2. Mostrar Usuario Activo
                dao.CargarUsuarioActivo();
                txtUsuario.Text = SesionUsuario.NombreUsuario;

                // 3. Cargar Clientes
                DataTable dtClientes = dao.ObtenerClientes();
                cmbCliente.DataSource = dtClientes;
                cmbCliente.DisplayMember = "Nombre_Apellido";
                cmbCliente.ValueMember = "IdCliente";
                cmbCliente.SelectedIndex = -1;
                txtTelefono.Clear();

                // 4. Cargar Productos de Inventario
                DataTable dtProductos = dao.ObtenerProductosInventario();
                cmbProducto.DataSource = dtProductos;
                cmbProducto.DisplayMember = "Detalle_Producto";
                cmbProducto.ValueMember = "IdProducto";
                cmbProducto.SelectedIndex = -1;
                txtPrecioUnitario.Clear();
                txtCantidad.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Al seleccionar un cliente, se rellena automáticamente su teléfono
        private void cmbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex != -1 && cmbCliente.SelectedItem is DataRowView fila)
            {
                txtTelefono.Text = fila["Telefono"].ToString();
            }
        }

        // Al seleccionar un producto, se carga su precio unitario y se pone cantidad en 1
        private void cmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex != -1 && cmbProducto.SelectedItem is DataRowView fila)
            {
                precioUnitarioBase = Convert.ToDecimal(fila["Precio_Unitario"]);
                stockDisponible = Convert.ToDecimal(fila["Stock"]);
                txtCantidad.Text = "1";
                txtPrecioUnitario.Text = precioUnitarioBase.ToString("0.00");
            }
        }

        // Opción A: Al escribir en Cantidad, multiplica el precio en vivo en txtPrecioUnitario
        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtCantidad.Text, out decimal cantidad) && cantidad > 0)
            {
                decimal precioMultiplicado = precioUnitarioBase * cantidad;
                txtPrecioUnitario.Text = precioMultiplicado.ToString("0.00");
            }
            else
            {
                txtPrecioUnitario.Text = precioUnitarioBase.ToString("0.00");
            }
        }

        // Botón [+] para agregar el producto al DataGridView
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCantidad.Focus();
                return;
            }

            if (cantidad > stockDisponible)
            {
                MessageBox.Show($"Stock insuficiente. Solo hay {stockDisponible} disponibles.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idProd = cmbProducto.SelectedValue.ToString();
            string nombreProd = cmbProducto.Text;
            decimal subtotalFila = precioUnitarioBase * cantidad;

            // Agregar fila al DataGridView
            dgvFactura.Rows.Add(idProd, nombreProd, cantidad, precioUnitarioBase.ToString("0.00"), subtotalFila.ToString("0.00"));

            CalcularTotales();

            // Limpiar selección de producto para el siguiente ingreso
            cmbProducto.SelectedIndex = -1;
            txtCantidad.Clear();
            txtPrecioUnitario.Clear();
            precioUnitarioBase = 0;
        }

        // Botón Borrar: Elimina solo la fila seleccionada preguntando antes
        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvFactura.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione la fila del producto que desea borrar en la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este producto de la lista?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                dgvFactura.Rows.RemoveAt(dgvFactura.SelectedRows[0].Index);
                CalcularTotales();
            }
        }

        // Cuando cambia el porcentaje de descuento en el ComboBox
        private void cmbDescuento_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotales();
        }

        // Calcula el Subtotal y aplica el Descuento al Total
        private void CalcularTotales()
        {
            decimal subtotalGeneral = 0;

            foreach (DataGridViewRow fila in dgvFactura.Rows)
            {
                if (fila.Cells["Subtotal"].Value != null)
                {
                    subtotalGeneral += Convert.ToDecimal(fila.Cells["Subtotal"].Value);
                }
            }

            txtSubtotal.Text = subtotalGeneral.ToString("0.00");

            // Extraer el número del descuento (ej. "15%" -> 15)
            decimal porcentajeDescuento = 0;
            if (cmbDescuento.SelectedItem != null)
            {
                string textoDesc = cmbDescuento.SelectedItem.ToString().Replace("%", "");
                decimal.TryParse(textoDesc, out porcentajeDescuento);
            }

            decimal montoDescuento = subtotalGeneral * (porcentajeDescuento / 100m);
            decimal totalFinal = subtotalGeneral - montoDescuento;

            txtTotal.Text = totalFinal.ToString("0.00");
        }

        // Botón Guardar e Imprimir: Guarda en SQL, descuenta Stock y genera el siguiente correlativo
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un cliente para la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvFactura.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                FacturaEntidad nuevaFactura = new FacturaEntidad
                {
                    IdFactura = txtIdFactura.Text,
                    Fecha = dtpFecha.Value,
                    IdUsuario = SesionUsuario.IdUsuario,
                    IdCliente = cmbCliente.SelectedValue.ToString(),
                    Total = Convert.ToDecimal(txtTotal.Text)
                };

                foreach (DataGridViewRow fila in dgvFactura.Rows)
                {
                    nuevaFactura.Detalles.Add(new DetalleFacturaEntidad
                    {
                        IdProducto = fila.Cells["IdProducto"].Value.ToString(),
                        Cantidad = Convert.ToDecimal(fila.Cells["Cantidad"].Value),
                        PrecioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value),
                        SubTotal = Convert.ToDecimal(fila.Cells["Subtotal"].Value)
                    });
                }

                if (dao.GuardarFactura(nuevaFactura))
                {
                    MessageBox.Show("Factura guardada exitosamente y stock actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvFactura.Rows.Clear();
                    CalcularTotales();
                    CargarCombosYCorrelativo(); // Prepara el siguiente correlativo FAC-000X
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la factura: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        // Método para bloquear números (Para Nombres, Empresas, Roles)
        private void ValidarSoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla si no es letra, control (borrar) o espacio
            }
        }

        // Método para bloquear letras (Para Teléfonos)
        private void ValidarSoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Bloquea la tecla si no es un número o control (borrar)
            }
        }
    }
}

