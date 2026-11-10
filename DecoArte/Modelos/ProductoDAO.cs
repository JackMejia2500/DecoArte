using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;

namespace DecoArte.Modelos
{
    public class ProductoEntidad
    {
        public string IdProducto { get; set; }
        public string DetalleProducto { get; set; }
        public string Categoria { get; set; }
        public decimal Cantidad { get; set; }
    }

    public class ProductoDAO
    {
        private Conexion db = new Conexion();

        // Genera correlativo: PRO-001, PRO-002...
        public string GenerarIdProducto()
        {
            string nuevoId = "PR-001";
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdProducto, 5, 10) AS INT)), 0) + 1 FROM Producto WHERE IdProducto LIKE 'PR-%'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                    nuevoId = "PR-" + siguiente.ToString("D3");
                }
            }
            catch (Exception ex) { throw new Exception("Error al generar ID de Producto: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return nuevoId;
        }

        public bool Guardar(ProductoEntidad prod)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "INSERT INTO Producto (IdProducto, Detalle_Producto, Categoria, Cantidad) VALUES (@Id, @Nom, @Cat, @Cant)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", prod.IdProducto);
                    cmd.Parameters.AddWithValue("@Nom", prod.DetalleProducto);
                    cmd.Parameters.AddWithValue("@Cat", prod.Categoria);
                    cmd.Parameters.AddWithValue("@Cant", prod.Cantidad); // Se inyecta 0 por defecto

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar producto: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }
    }
}