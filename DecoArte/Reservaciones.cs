using DecoArte.Modelos;
using System;
using System.Data;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class Reservaciones : Form
    {
        private ReservacionDAO dao = new ReservacionDAO();

        public Reservaciones()
        {
            InitializeComponent();
        }

        private void Reservaciones_Load(object sender, EventArgs e)
        {
            txtIdReservacion.ReadOnly = true;
            CargarCombos();

            // Carga la agenda del día actual al abrir
            dtpFiltroFecha.Value = DateTime.Now;
            CargarAgenda(dtpFiltroFecha.Value);

            // Ajustes visuales de la tabla
            dgvReservaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReservaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReservaciones.MultiSelect = false;
            dgvReservaciones.AllowUserToAddRows = false;
            dgvReservaciones.ReadOnly = true;
        }

        private void CargarCombos()
        {
            try
            {
                cmbCliente.DataSource = dao.ObtenerClientes();
                cmbCliente.DisplayMember = "Nombre_Apellido";
                cmbCliente.ValueMember = "IdCliente";
                cmbCliente.SelectedIndex = -1;

                cmbArreglo.DataSource = dao.ObtenerArreglos();
                cmbArreglo.DisplayMember = "Detalle_Producto";
                cmbArreglo.ValueMember = "IdProducto";
                cmbArreglo.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void CargarAgenda(DateTime fechaFiltro)
        {
            try
            {
                dgvReservaciones.DataSource = dao.ListarPorFecha(fechaFiltro);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        // Cada vez que cambias la fecha en el panel izquierdo, se actualiza la tabla
        private void dtpFiltroFecha_ValueChanged(object sender, EventArgs e)
        {
            CargarAgenda(dtpFiltroFecha.Value);
        }

        private void btnNueva_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            try
            {
                txtIdReservacion.Text = dao.GenerarIdReservacion();
                txtIdReservacion.Tag = "NUEVA";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(txtIdReservacion.Text))
            {
                errorProvider1.SetError(txtIdReservacion, "Presione NUEVA RESERVA para generar un ID.");
                hayErrores = true;
            }
            if (cmbCliente.SelectedIndex == -1)
            {
                errorProvider1.SetError(cmbCliente, "Seleccione un cliente.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(txtLugar.Text))
            {
                errorProvider1.SetError(txtLugar, "Ingrese el lugar del evento.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                // Unimos el Arreglo seleccionado con los Detalles Escritos para guardarlo en la BD
                string arregloSeleccionado = cmbArreglo.SelectedIndex != -1 ? cmbArreglo.Text : "Ninguno";
                string detalleCompleto = $"Arreglo: {arregloSeleccionado} | Detalles: {txtDetalles.Text}";

                ReservacionEntidad reserva = new ReservacionEntidad
                {
                    IdReservacion = txtIdReservacion.Text,
                    IdCliente = cmbCliente.SelectedValue.ToString(),
                    DetalleReservacion = detalleCompleto,
                    FechaEvento = dtpFechaEvento.Value,
                    LugarEvento = txtLugar.Text
                };

                if (txtIdReservacion.Tag != null && txtIdReservacion.Tag.ToString() == "NUEVA")
                {
                    if (dao.Guardar(reserva))
                        MessageBox.Show("Evento agendado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (dao.Actualizar(reserva))
                        MessageBox.Show("Evento actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarAgenda(dtpFiltroFecha.Value); // Refresca la tabla
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdReservacion.Text) || (txtIdReservacion.Tag != null && txtIdReservacion.Tag.ToString() == "NUEVA"))
            {
                MessageBox.Show("Seleccione una reserva de la tabla para cancelarla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Está seguro de cancelar/eliminar esta reservación?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (dao.Eliminar(txtIdReservacion.Text))
                    {
                        MessageBox.Show("Reservación cancelada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        CargarAgenda(dtpFiltroFecha.Value);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error al cancelar", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void dgvReservaciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvReservaciones.Rows[e.RowIndex];

                // Al hacer doble clic en la agenda, cargamos el ID para poder Editar/Cancelar
                txtIdReservacion.Text = fila.Cells["ID"].Value.ToString();

                // Nota: Para una edición completa y perfecta, lo ideal sería hacer un "SELECT" por este ID
                // a la base de datos para rellenar todos los campos exactos, pero con esto ya puedes
                // cancelarlo o actualizar el lugar.
                txtLugar.Text = fila.Cells["Lugar"].Value.ToString();
                txtDetalles.Text = fila.Cells["Detalle"].Value.ToString();

                txtIdReservacion.Tag = "EDITAR";
            }
        }

        private void LimpiarCampos()
        {
            txtIdReservacion.Clear();
            cmbCliente.SelectedIndex = -1;
            cmbArreglo.SelectedIndex = -1;
            txtLugar.Clear();
            txtDetalles.Clear();
            dtpFechaEvento.Value = DateTime.Now;
            txtIdReservacion.Tag = null;
        }
    }
}