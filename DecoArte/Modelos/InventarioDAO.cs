using System.Data;
using Microsoft.Data.SqlClient;
using DecoArte.conexion;

public class InventarioDAO
{
    Conexion db = new Conexion();

    // Método para llenar el DataGridView al abrir el formulario
    public DataTable CargarInventario()
    {
        DataTable tabla = new DataTable();
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            
            string query = @"SELECT 
                i.IdProducto, 
                p.Detalle_Producto AS Producto, 
                i.Stock, 
                i.Total 
            FROM Inventario i
            INNER JOIN Producto p ON i.IdProducto = p.IdProducto";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(tabla);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar inventario: " + ex.Message);
        }
        finally
        {
            db.CerrarConexion(conn);
        }
        return tabla;
    }

    // Método para la barra superior de "Buscar Producto:"
    public DataTable BuscarEnInventario(string filtro)
    {
        DataTable tabla = new DataTable();
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            // Busca coincidencias en cualquiera de los dos IDs
            string query = "SELECT IdProducto, IdProducto_Servicio, Stock, Total FROM Inventario WHERE IdProducto LIKE @Filtro OR IdProducto_Servicio LIKE @Filtro";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(tabla);
                }
            }
        }
        finally
        {
            db.CerrarConexion(conn);
        }
        return tabla;
    }
    public bool ActualizarStock(string idProducto, decimal nuevoStock, decimal nuevoTotal)
    {
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            string query = "UPDATE Inventario SET Stock = @Stock, Total = @Total WHERE IdProducto = @Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Stock", nuevoStock);
                cmd.Parameters.AddWithValue("@Total", nuevoTotal);
                cmd.Parameters.AddWithValue("@Id", idProducto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            db.CerrarConexion(conn);
        }
    }

    // Método para Eliminar un producto completo
    public bool EliminarProducto(string idProducto)
    {
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            string query = "DELETE FROM Inventario WHERE IdProducto = @Id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idProducto);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            db.CerrarConexion(conn);
        }
    }
}