using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

public class ComprasDAO
{
    Conexion db = new Conexion();

   
    public bool GuardarCompraTransaccion(string idCompra, string proveedor, DateTime fecha, decimal total, List<(string Producto, decimal Cantidad, decimal Precio, decimal Subtotal, string Categoria)> detalles)
    {
        SqlConnection conn = null;
        SqlTransaction transaccion = null;

        try
        {
            conn = db.ObtenerConexion();
            transaccion = conn.BeginTransaction();

            string queryCompra = "INSERT INTO Compras_Servicios (IdCompra_Servicios, IdProveedor, Fecha, Total) VALUES (@Id, @Prov, @Fecha, @Total)";
            using (SqlCommand cmd = new SqlCommand(queryCompra, conn, transaccion))
            {
                cmd.Parameters.AddWithValue("@Id", idCompra);
                cmd.Parameters.AddWithValue("@Prov", proveedor);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Total", total);
                cmd.ExecuteNonQuery();
            }

            string queryDetalle = "INSERT INTO Detalle_Compras_Servicios (IdCompra_Servicios, IdProducto, Cantidad, Precio_Unitario, Subtotal) VALUES (@IdCompra, @Prod, @Cant, @Prec, @Sub)";

            string queryInventario = @"
                IF EXISTS (SELECT 1 FROM Inventario WHERE IdProducto = @Prod)
                    UPDATE Inventario SET Stock = Stock + @Cant, Total = Total + @Sub, Precio_Unitario = @Prec WHERE IdProducto = @Prod
                ELSE
                    INSERT INTO Inventario (IdProducto, IdProducto_Servicio, Stock, Total, Precio_Unitario) 
                    VALUES (@Prod, @Prod, @Cant, @Sub, @Prec)";

            using (SqlCommand cmdDet = new SqlCommand(queryDetalle, conn, transaccion))
            using (SqlCommand cmdInv = new SqlCommand(queryInventario, conn, transaccion))
            {
                foreach (var item in detalles)
                {
                    cmdDet.Parameters.Clear();
                    cmdDet.Parameters.AddWithValue("@IdCompra", idCompra);
                    cmdDet.Parameters.AddWithValue("@Prod", item.Producto);
                    cmdDet.Parameters.AddWithValue("@Cant", item.Cantidad);
                    cmdDet.Parameters.AddWithValue("@Prec", item.Precio);
                    cmdDet.Parameters.AddWithValue("@Sub", item.Subtotal);
                    cmdDet.ExecuteNonQuery();

                    // Candado flexible y detección de errores silenciosos
                    string categoriaLimpia = item.Categoria != null ? item.Categoria.Trim().ToUpper() : "";

                    if (categoriaLimpia.Contains("FISI") || categoriaLimpia.Contains("FÍSI") || categoriaLimpia.Contains("PROD"))
                    {
                        cmdInv.Parameters.Clear();
                        cmdInv.Parameters.AddWithValue("@Prod", item.Producto);
                        cmdInv.Parameters.AddWithValue("@Cant", item.Cantidad);
                        cmdInv.Parameters.AddWithValue("@Sub", item.Subtotal);
                        cmdInv.Parameters.AddWithValue("@Prec", item.Precio);

                        int filasModificadas = cmdInv.ExecuteNonQuery();
                        if (filasModificadas == 0)
                        {
                            throw new Exception("Error interno SQL: No se pudo inyectar el artículo '" + item.Producto + "' en el Inventario.");
                        }
                    }
                }
                
            }

            transaccion.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaccion?.Rollback();
            MessageBox.Show("Error SQL al procesar compra: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally { db.CerrarConexion(conn); }
    }

    public DataTable ObtenerProveedores()
    {
        DataTable tabla = new DataTable();
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            
            string query = "SELECT IdProveedor, Nombre_Empresa FROM Proveedor";
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
            MessageBox.Show("Error SQL en Proveedores: " + ex.Message, "Aviso Técnico");
        }
        finally { db.CerrarConexion(conn); }
        return tabla;
    }

    public DataTable ObtenerProductos()
    {
        DataTable tabla = new DataTable();
        SqlConnection conn = null;
        try
        {
            conn = db.ObtenerConexion();
            
            string query = "SELECT IdProducto, Detalle_Producto, Categoria FROM Producto";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) { adapter.Fill(tabla); }
            }
        }
        catch (Exception ex) { MessageBox.Show("Error SQL: " + ex.Message); }
        finally { db.CerrarConexion(conn); }
        return tabla;
    }
}