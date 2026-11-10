using DecoArte.Modelos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Clientes : Form
    {
        public Clientes()
        {
            InitializeComponent();
            CargarClientes();
            // Bloqueamos el ID para que el usuario no lo escriba a mano
            txIdCliente.ReadOnly = true;
        }

        private void txtBucarCliente_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ClienteDAO dao = new ClienteDAO();
                dgvClientes.DataSource = dao.Buscar(txtBucarCliente.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            LimpiarCampos();

            // Lógica nueva: Generar el ID automáticamente al pedir un cliente nuevo
            try
            {
                ClienteDAO dao = new ClienteDAO();
                // Asegúrate de crear este método en ClienteDAO.cs (similar al de Factura)
                txIdCliente.Text = dao.GenerarIdCliente();
                txIdCliente.Tag = "NUEVO"; // Usamos Tag para saber que es un registro nuevo
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarCliente_Click(object sender, EventArgs e)
        {
            // Limpiamos alertas usando un solo ErrorProvider
            errorProvider1.Clear();
            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(txIdCliente.Text))
            {
                errorProvider1.SetError(txIdCliente, "Presione 'NUEVO' para generar un ID.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtNombrecliente.Text))
            {
                errorProvider1.SetError(txtNombrecliente, "El nombre es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtTelefonoCliente.Text))
            {
                errorProvider1.SetError(txtTelefonoCliente, "El teléfono es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtDirreccion.Text))
            {
                errorProvider1.SetError(txtDirreccion, "La dirección es obligatoria.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                Cliente nuevoCliente = new Cliente
                {
                    IdCliente = txIdCliente.Text,
                    NombreApellido = txtNombrecliente.Text,
                    Telefono = txtTelefonoCliente.Text,
                    Direccion = txtDirreccion.Text
                };

                ClienteDAO dao = new ClienteDAO();

                // Evaluamos por medio de la propiedad Tag que seteamos arriba
                if (txIdCliente.Tag != null && txIdCliente.Tag.ToString() == "NUEVO")
                {
                    if (dao.Guardar(nuevoCliente)) // Usar el nombre exacto de tu método Insertar/Guardar
                    {
                        MessageBox.Show("Cliente guardado con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (dao.Actualizar(nuevoCliente))
                    {
                        MessageBox.Show("Cliente actualizado con éxito.", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                CargarClientes();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txIdCliente.Clear();
            txtNombrecliente.Clear();
            txtTelefonoCliente.Clear();
            txtDirreccion.Clear();
            txIdCliente.Tag = null; // Reseteamos el estado
        }

        private void btnEliminarCliente_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txIdCliente.Text) || (txIdCliente.Tag != null && txIdCliente.Tag.ToString() == "NUEVO"))
            {
                MessageBox.Show("Seleccione un cliente de la tabla para eliminar.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de que desea eliminar este cliente?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    ClienteDAO dao = new ClienteDAO();
                    if (dao.Eliminar(txIdCliente.Text))
                    {
                        MessageBox.Show("Cliente eliminado con éxito.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarClientes();
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarClientes()
        {
            try
            {
                ClienteDAO dao = new ClienteDAO();
                dgvClientes.DataSource = dao.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];
                txIdCliente.Text = fila.Cells["IdCliente"].Value.ToString();
                txtNombrecliente.Text = fila.Cells["Nombre_Apellido"].Value.ToString(); // Ajusta si en el DGV dice NombreApellido
                txtTelefonoCliente.Text = fila.Cells["Telefono"].Value.ToString();
                txtDirreccion.Text = fila.Cells["Direccion"].Value.ToString();

                txIdCliente.Tag = "EDITAR"; // Cambiamos el estado a Edición
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