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
    public partial class Clientes : Form
    {
        public Clientes()
        {
            InitializeComponent();
            CargarClientes(); // Cargar los clientes al iniciar el formulario
        }

        private void txtBucarCliente_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ClienteDAO dao = new ClienteDAO();
                // Pasamos el texto actual del TextBox al método Buscar y actualizamos la tabla
                dgvClientes.DataSource = dao.Buscar(txtBucarCliente.Text);
            }
            catch (Exception ex)
            {
                // Usamos un MessageBox silencioso o lo dejamos en consola para no interrumpir 
                // la escritura del usuario con ventanas emergentes en cada tecla si hay error de conexión
                Console.WriteLine(ex.Message);
            }
        }

        private void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnGuardarCliente_Click(object sender, EventArgs e)
        {

            // Limpiamos alertas previas de los 4 controles

            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();

            bool hayErrores = false;

            // Validaciones asignadas a cada ErrorProvider específico
            if (string.IsNullOrWhiteSpace(txIdCliente.Text))
            {
                errorProvider1.SetError(txIdCliente, "El ID es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtNombrecliente.Text))
            {
                errorProvider2.SetError(txtNombrecliente, "El nombre es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtTelefonoCliente.Text))
            {
                errorProvider3.SetError(txtTelefonoCliente, "El teléfono es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtDirreccion.Text))
            {
                errorProvider4.SetError(txtDirreccion, "La dirección es obligatoria.");
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

                // Evaluamos el estado de la caja de texto del ID
                if (txIdCliente.Enabled)
                {
                    // Si está habilitada, es un registro nuevo
                    if (dao.Guardar(nuevoCliente))
                    {
                        MessageBox.Show("Cliente guardado con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Si está bloqueada, es una edición
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
            txIdCliente.Enabled = true; // Habilitar el campo de ID para nuevos registros
        }

        private void btnEliminarCliente_Click(object sender, EventArgs e)
        {
            if (txIdCliente.Enabled)
            {
                MessageBox.Show("Seleccione un cliente para eliminar.", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                List<Cliente> clientes = dao.Listar();
                dgvClientes.DataSource = clientes;
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

                // Corrección: NombreApellido en lugar de Nombre_Apellido
                txtNombrecliente.Text = fila.Cells["NombreApellido"].Value.ToString();

                txtTelefonoCliente.Text = fila.Cells["Telefono"].Value.ToString();
                txtDirreccion.Text = fila.Cells["Direccion"].Value.ToString();

                txIdCliente.Enabled = false;
            }
        }
    }
}

        
