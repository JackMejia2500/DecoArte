using DecoArte.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Usuario : Form
    {
        private UsuarioDAO dao = new UsuarioDAO();

        public Usuario()
        {
            InitializeComponent();
        }

        private void Usuario_Load(object sender, EventArgs e)
        {
            txtIdUsuario.ReadOnly = true;
            CargarTabla();
        }

        private void CargarTabla()
        {
            try
            {
                dgvUsuarios.DataSource = dao.Listar();
                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            try
            {
                txtIdUsuario.Text = dao.GenerarIdUsuario();
                txtIdUsuario.Tag = "NUEVO";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(txtIdUsuario.Text))
            {
                errorProvider1.SetError(txtIdUsuario, "Presione 'NUEVO' para generar un ID.");
                hayErrores = true;
            }
            // Agrega txtNombreUsuario en tu diseño para esto
            if (string.IsNullOrWhiteSpace(txtNombreApellido.Text))
            {
                errorProvider1.SetError(txtNombreApellido, "El usuario de login es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                errorProvider1.SetError(txtContraseña, "La contraseña es obligatoria.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtNombreApellido.Text))
            {
                errorProvider1.SetError(txtNombreApellido, "El nombre completo es obligatorio.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                // Recopilar los permisos seleccionados
                List<string> listaPermisos = new List<string>();
                if (cbFacturacion.Checked) listaPermisos.Add("Facturacion");
                if (cbInventario.Checked) listaPermisos.Add("Inventario");
                if (cbUsuarios.Checked) listaPermisos.Add("Usuarios");
                if (cbNuevoProducto.Checked) listaPermisos.Add("Nuevo Producto");
                if (cbProveedores.Checked) listaPermisos.Add("Proveedores");
                if (cbReservaciones.Checked) listaPermisos.Add("Reservaciones");
                if (cbComprasyServicios.Checked) listaPermisos.Add("Compras y Servicios");

                string permisosConcatenados = string.Join(",", listaPermisos);

                UsuarioEntidad usu = new UsuarioEntidad
                {
                    IdUsuario = txtIdUsuario.Text,
                    NombreUsuario = txtNombreApellido.Text,
                    Contrasena = txtContraseña.Text,
                    NombreApellido = txtNombreApellido.Text,
                    Telefono = txtTelefono.Text,
                    Direccion = txtDireccion.Text,
                    FechaContratacion = dtpFechaContratacion.Value,
                    Rol = txtRol.Text,
                    Permisos = permisosConcatenados
                };

                if (txtIdUsuario.Tag != null && txtIdUsuario.Tag.ToString() == "NUEVO")
                {
                    if (dao.Guardar(usu)) MessageBox.Show("Usuario guardado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (dao.Actualizar(usu)) MessageBox.Show("Usuario actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarTabla();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdUsuario.Text) || (txtIdUsuario.Tag != null && txtIdUsuario.Tag.ToString() == "NUEVO"))
            {
                MessageBox.Show("Seleccione un usuario de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de eliminar este usuario del sistema?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (dao.Eliminar(txtIdUsuario.Text))
                    {
                        MessageBox.Show("Usuario eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarTabla();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvUsuarios.Rows[e.RowIndex];
                string idSeleccionado = fila.Cells["IdUsuario"].Value.ToString();

                // Traemos los datos completos (incluyendo permisos y contraseña)
                UsuarioEntidad usu = dao.ObtenerPorId(idSeleccionado);

                if (usu != null)
                {
                    txtIdUsuario.Text = usu.IdUsuario;
                    txtNombreApellido.Text = usu.NombreUsuario;
                    txtContraseña.Text = usu.Contrasena;
                    txtNombreApellido.Text = usu.NombreApellido;
                    txtTelefono.Text = usu.Telefono;
                    txtDireccion.Text = usu.Direccion;
                    dtpFechaContratacion.Value = usu.FechaContratacion;
                    txtRol.Text = usu.Rol;

                    // Marcar checkboxes según el string guardado
                    cbFacturacion.Checked = usu.Permisos.Contains("Facturacion");
                    cbInventario.Checked = usu.Permisos.Contains("Inventario");
                    cbUsuarios.Checked = usu.Permisos.Contains("Usuarios");
                    cbNuevoProducto.Checked = usu.Permisos.Contains("Nuevo Producto");
                    cbProveedores.Checked = usu.Permisos.Contains("Proveedores");
                    cbReservaciones.Checked = usu.Permisos.Contains("Reservaciones");
                    cbComprasyServicios.Checked = usu.Permisos.Contains("Compras y Servicios");

                    txtIdUsuario.Tag = "EDITAR";
                }
            }
        }

        private void txtBuscarUsuario_TextChanged(object sender, EventArgs e)
        {
            try { dgvUsuarios.DataSource = dao.Buscar(txtBuscarUsuario.Text); }
            catch (Exception ex) { Console.WriteLine("Búsqueda: " + ex.Message); }
        }

        private void LimpiarCampos()
        {
            txtIdUsuario.Clear();
            txtNombreApellido.Clear();
            txtContraseña.Clear();
            txtNombreApellido.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtRol.Clear();
            dtpFechaContratacion.Value = DateTime.Now;

            // Desmarcar todos los checks
            cbFacturacion.Checked = false;
            cbInventario.Checked = false;
            cbUsuarios.Checked = false;
            cbNuevoProducto.Checked = false;
            cbProveedores.Checked = false;
            cbReservaciones.Checked = false;
            cbComprasyServicios.Checked = false;

            txtIdUsuario.Tag = null;
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