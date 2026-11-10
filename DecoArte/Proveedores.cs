using DecoArte.Modelos;
using System;
using System.Data;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Proveedores : Form
    {
        private ProveedorDAO dao = new ProveedorDAO();

        public Proveedores()
        {
            InitializeComponent();
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
            CargarTabla();
            txtIdProveedor.ReadOnly = true;
        }

        private void CargarTabla()
        {
            try
            {
                dgvProveedores.DataSource = dao.Listar();
                dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            try
            {
                txtIdProveedor.Text = dao.GenerarIdProveedor();
                txtIdProveedor.Tag = "NUEVO";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(txtIdProveedor.Text))
            {
                errorProvider1.SetError(txtIdProveedor, "Debe generar un ID presionando 'NUEVO'.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtEmpresa.Text))
            {
                errorProvider1.SetError(txtEmpresa, "El nombre de la empresa es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtRuc.Text))
            {
                errorProvider1.SetError(txtRuc, "El RUC es obligatorio.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                ProveedorEntidad prov = new ProveedorEntidad
                {
                    IdProveedor = txtIdProveedor.Text,
                    NombreEmpresa = txtEmpresa.Text,
                    RUC = txtRuc.Text,
                    Telefono = txtTelefono.Text,
                    Direccion = txtDireccion.Text
                };

                if (txtIdProveedor.Tag != null && txtIdProveedor.Tag.ToString() == "NUEVO")
                {
                    if (dao.Guardar(prov))
                    {
                        MessageBox.Show("Proveedor guardado con éxito.", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (dao.Actualizar(prov))
                    {
                        MessageBox.Show("Proveedor actualizado con éxito.", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LimpiarCampos();
                CargarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdProveedor.Text) || (txtIdProveedor.Tag != null && txtIdProveedor.Tag.ToString() == "NUEVO"))
            {
                MessageBox.Show("Seleccione un proveedor de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (dao.Eliminar(txtIdProveedor.Text))
                    {
                        MessageBox.Show("Proveedor eliminado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarTabla();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvProveedores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProveedores.Rows[e.RowIndex];
                txtIdProveedor.Text = fila.Cells["IdProveedor"].Value.ToString();
                txtEmpresa.Text = fila.Cells["Nombre_Empresa"].Value.ToString();
                txtRuc.Text = fila.Cells["RUC"].Value.ToString();
                txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();
                txtDireccion.Text = fila.Cells["Direccion"].Value.ToString();

                txtIdProveedor.Tag = "EDITAR";
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dgvProveedores.DataSource = dao.Buscar(txtBuscar.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Búsqueda: " + ex.Message);
            }
        }

        private void LimpiarCampos()
        {
            txtIdProveedor.Clear();
            txtEmpresa.Clear();
            txtRuc.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtIdProveedor.Tag = null;
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