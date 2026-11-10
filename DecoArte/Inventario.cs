using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms; // ¡Vital! Esto devuelve a la vida a InitializeComponent y los controles

namespace DecoArte
{
    // La clave está aquí: "partial class" y ": Form"
    public partial class Inventario : Form
    {
        // 1. Agrega esta línea maestra aquí arriba para que todo el formulario la vea
        InventarioDAO dao = new InventarioDAO();

        public Inventario()
        {
            InitializeComponent();
            CargarDatos();
        }

        public void CargarDatos()
        {
            try
            {
                DataTable dt = dao.CargarInventario();

                // 1. Le decimos a C# que cree las columnas automáticamente 
                // basándose en los nombres de tu consulta SQL
                dgvInventario.AutoGenerateColumns = true;

                // 2. Inyectamos los datos directamente
                dgvInventario.DataSource = dt;

                // 3. Diagnóstico definitivo

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error visual: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Usando el nombre exacto que muestra tu error: txtBuscarProducto
        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            dgvInventario.DataSource = dao.BuscarEnInventario(txtBuscarProducto.Text);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            // Validamos que haya una fila seleccionada
            if (dgvInventario.CurrentRow != null)
            {
                try
                {
                    // Capturamos los datos directamente de las celdas de la fila seleccionada
                    string id = dgvInventario.CurrentRow.Cells["IdProducto"].Value.ToString();
                    decimal stockModificado = Convert.ToDecimal(dgvInventario.CurrentRow.Cells["Stock"].Value);
                    decimal totalModificado = Convert.ToDecimal(dgvInventario.CurrentRow.Cells["Total"].Value);

                    if (dao.ActualizarStock(id, stockModificado, totalModificado))
                    {
                        MessageBox.Show("Inventario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvInventario.DataSource = dao.CargarInventario(); // Recargamos para ver los cambios
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar. Verifica que los números tengan el formato correcto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (dgvInventario.CurrentRow != null)
            {
                string idSeleccionado = dgvInventario.CurrentRow.Cells["IdProducto"].Value.ToString();

                // Pedimos confirmación antes de destruir el dato
                DialogResult respuesta = MessageBox.Show($"¿Estás seguro de eliminar el producto {idSeleccionado} del inventario?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    if (dao.EliminarProducto(idSeleccionado))
                    {
                        MessageBox.Show("Producto eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvInventario.DataSource = dao.CargarInventario(); // Refrescamos la tabla
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona toda la fila del producto que deseas borrar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            // Validamos que la tabla tenga datos
            if (dgvInventario.Rows.Count > 0)
            {
                // Abrimos la ventana de Windows para elegir dónde guardar el archivo
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Archivo de Excel (*.csv)|*.csv",
                    FileName = "Reporte_Inventario_DecoArte.csv",
                    Title = "Guardar reporte de inventario"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Preparamos la estructura de las columnas
                        int cantidadColumnas = dgvInventario.Columns.Count;
                        string cabeceras = "";
                        string[] lineas = new string[dgvInventario.Rows.Count + 1];

                        // 1. Extraemos los nombres de las columnas
                        for (int i = 0; i < cantidadColumnas; i++)
                        {
                            cabeceras += dgvInventario.Columns[i].HeaderText + ",";
                        }
                        lineas[0] = cabeceras;

                        // 2. Extraemos los datos fila por fila
                        for (int i = 1; i <= dgvInventario.Rows.Count; i++)
                        {
                            string filaDatos = "";
                            for (int j = 0; j < cantidadColumnas; j++)
                            {
                                // Revisamos que la celda no esté vacía para evitar errores
                                var valorCelda = dgvInventario.Rows[i - 1].Cells[j].Value;
                                filaDatos += (valorCelda != null ? valorCelda.ToString() : "") + ",";
                            }
                            lineas[i] = filaDatos;
                        }

                        // 3. Escribimos el archivo físico en la computadora
                        File.WriteAllLines(sfd.FileName, lineas, Encoding.UTF8);

                        MessageBox.Show("Inventario exportado a Excel con éxito.", "Exportación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar. Asegúrate de que el archivo no esté abierto en Excel en este momento. \n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No hay datos en el inventario para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void dgvClientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}