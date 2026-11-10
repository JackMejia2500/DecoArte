using DecoArte.Modelos;
using System;
using System.Windows.Forms;

namespace DecoArte
{
    public partial class FrmNuevoProducto : Form
    {
        private ProductoDAO dao = new ProductoDAO();

        public FrmNuevoProducto()
        {
            InitializeComponent();
            this.Load += FrmNuevoProducto_Load;
        }

        private void FrmNuevoProducto_Load(object sender, EventArgs e)
        {
            txtCodigo.ReadOnly = true;

            // Llenamos las categorías permitidas por la lógica de negocio (Compras)
            cmbCategoria.Items.Clear();
            cmbCategoria.Items.AddRange(new object[] { "Fisico", "Servicio" });
            cmbCategoria.SelectedIndex = 0;

            try
            {
                // Generar el ID al abrir la ventana
                txtCodigo.Text = dao.GenerarIdProducto();
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

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider1.SetError(txtNombre, "El nombre del producto es obligatorio.");
                hayErrores = true;
            }
            if (string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                errorProvider1.SetError(cmbCategoria, "Seleccione una categoría.");
                hayErrores = true;
            }

            if (hayErrores) return;

            try
            {
                ProductoEntidad nuevoProd = new ProductoEntidad
                {
                    IdProducto = txtCodigo.Text,
                    DetalleProducto = txtNombre.Text.Trim().ToUpper(), // Mayúsculas para estandarizar
                    Categoria = cmbCategoria.Text,
                    Cantidad = 0 // El valor inicial que tenías
                };

                if (dao.Guardar(nuevoProd))
                {
                    MessageBox.Show("Producto agregado al catálogo exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // DialogResult.OK le avisa al formulario de Compras que debe recargar el ComboBox
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}