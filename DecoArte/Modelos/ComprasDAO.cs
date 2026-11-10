using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DecoArte.Modelos
{
    public class ComprasDAO
    {
        private Conexion db = new Conexion();

        // MÉTODO NUEVO: Generar ID Compra automáticamente (COM-0001)
        public string GenerarIdCompra()
        {
            string nuevoId = "CM-001";
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdCompra_Servicios, 5, 10) AS INT)), 0) + 1 FROM Compras_Servicios WHERE IdCompra_Servicios LIKE 'CM-%'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                    nuevoId = "CM-" + siguiente.ToString("D4");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar ID de Compra: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
            return nuevoId;
        }

        public bool GuardarCompraTransaccion(string idCompra, string proveedor, DateTime fecha, decimal total, List<(string Producto, decimal Cantidad, decimal Precio, decimal Subtotal, string Categoria)> detalles)
        {
            SqlConnection conn = null;
            SqlTransaction transaccion = null;

            try
            {
                conn = db.ObtenerConexion();
                transaccion = conn.BeginTransaction();

                // 1. Cabecera
                string queryCompra = "INSERT INTO Compras_Servicios (IdCompra_Servicios, IdProveedor, Fecha, Total) VALUES (@Id, @Prov, @Fecha, @Total)";
                using (SqlCommand cmd = new SqlCommand(queryCompra, conn, transaccion))
                {
                    cmd.Parameters.AddWithValue("@Id", idCompra);
                    cmd.Parameters.AddWithValue("@Prov", proveedor);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.ExecuteNonQuery();
                }

                // 2. Detalles e Inventario
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

                        string categoriaLimpia = item.Categoria != null ? item.Categoria.Trim().ToUpper() : "";

                        // Solo suma stock si no es un "SERVICIO"
                        if (categoriaLimpia != "SERVICIO")
                        {
                            cmdInv.Parameters.Clear();
                            cmdInv.Parameters.AddWithValue("@Prod", item.Producto);
                            cmdInv.Parameters.AddWithValue("@Cant", item.Cantidad);
                            cmdInv.Parameters.AddWithValue("@Sub", item.Subtotal);
                            cmdInv.Parameters.AddWithValue("@Prec", item.Precio);
                            cmdInv.ExecuteNonQuery();
                        }
                    }
                }

                transaccion.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaccion?.Rollback();
                throw new Exception("Error al guardar compra: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
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
                throw new Exception("Error al cargar Proveedores: " + ex.Message);
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
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(tabla);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar Productos: " + ex.Message);
            }
            finally { db.CerrarConexion(conn); }
            return tabla;
        }
        // Agrégalo dentro de tu clase ComprasDAO
        public DataTable ObtenerHistoricoPorFecha(DateTime fecha)
        {
            DataTable dt = new DataTable();
            Microsoft.Data.SqlClient.SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                // Unimos las 4 tablas para tener nombres reales y no solo IDs
                string query = @"
            SELECT 
                c.IdCompra_Servicios AS 'ID Compra',
                p.Nombre_Empresa AS 'Proveedor',
                c.Fecha,
                prod.Detalle_Producto AS 'Producto/Servicio',
                d.Cantidad,
                d.Precio_Unitario AS 'Precio Unit.',
                d.Subtotal
            FROM Compras_Servicios c
            INNER JOIN Proveedor p ON c.IdProveedor = p.IdProveedor
            INNER JOIN Detalle_Compras_Servicios d ON c.IdCompra_Servicios = d.IdCompra_Servicios
            INNER JOIN Producto prod ON d.IdProducto = prod.IdProducto
            WHERE CONVERT(DATE, c.Fecha) = CONVERT(DATE, @Fecha)
            ORDER BY c.Fecha DESC";

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    using (Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al cargar histórico: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }
    }
}