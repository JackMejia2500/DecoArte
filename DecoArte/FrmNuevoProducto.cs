using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DecoArte
{
    public partial class FrmNuevoProducto : Form
    {
        public FrmNuevoProducto()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                MessageBox.Show("Llena todos los campos.");
                return;
            }

            try
            {
                Conexion db = new Conexion();
                using (SqlConnection conn = db.ObtenerConexion())
                {
                    // La consulta debe tener 4 columnas y 4 parámetros
                    string query = "INSERT INTO Producto (IdProducto, Detalle_Producto, Categoria, Cantidad) VALUES (@Id, @Nom, @Cat, @Cant)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", txtCodigo.Text);
                        cmd.Parameters.AddWithValue("@Nom", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@Cat", cmbCategoria.Text);
                        cmd.Parameters.AddWithValue("@Cant", 0); // El valor por defecto

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto agregado al catálogo.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
