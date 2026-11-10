using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace DecoArte.Modelos
{
    // Variable global sencilla para guardar quién inició sesión
    public static class SesionUsuario
    {
        public static string IdUsuario { get; set; }
        public static string NombreUsuario { get; set; }
        public static string Permisos { get; set; } // <--- Agrega esta línea
    }

    // PILAR 1: ENCAPSULAMIENTO (Entidades de Factura y Detalle)
    public class FacturaEntidad
    {
        public string IdFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string IdUsuario { get; set; }
        public string IdCliente { get; set; }
        public decimal Total { get; set; }
        public List<DetalleFacturaEntidad> Detalles { get; set; } = new List<DetalleFacturaEntidad>();
    }

    public class DetalleFacturaEntidad
    {
        public string IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }

    // PILAR 2 Y 3: HERENCIA (Hereda de Conexion) Y ABSTRACCIÓN
    public class FacturaDAO : Conexion
    {
        // 1. Genera correlativo limpio: FAC-0001, FAC-0002...
        public virtual string GenerarIdFactura()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdFactura, 5, 10) AS INT)), 0) + 1 FROM Factura WHERE IdFactura LIKE 'FAC-%'";
                SqlCommand cmd = new SqlCommand(query, con);
                int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                return "FAC-" + siguiente.ToString("D4");
            }
        }

        // 2. Obtiene los clientes con su teléfono
        public DataTable ObtenerClientes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = ObtenerConexion())
            {
                string query = "SELECT IdCliente, Nombre_Apellido, Telefono FROM Clientes";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 3. Obtiene los productos en inventario con su Precio y Stock
        public DataTable ObtenerProductosInventario()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = ObtenerConexion())
            {
                string query = @"SELECT p.IdProducto, p.Detalle_Producto, i.Precio_Unitario, i.Stock 
                                 FROM Producto p 
                                 INNER JOIN Inventario i ON p.IdProducto = i.IdProducto 
                                 WHERE i.Stock > 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        // 4. Obtiene el usuario activo (o carga el primero de la tabla Usuarios si estás haciendo pruebas)
        public void CargarUsuarioActivo()
        {
            if (!string.IsNullOrEmpty(SesionUsuario.NombreUsuario)) return;

            using (SqlConnection con = ObtenerConexion())
            {
                string query = "SELECT TOP 1 IdUsuario, NombreUsuario FROM Usuarios";
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        SesionUsuario.IdUsuario = dr["IdUsuario"].ToString();
                        SesionUsuario.NombreUsuario = dr["NombreUsuario"].ToString();
                    }
                }
            }
        }

        // PILAR 4: POLIMORFISMO (Método virtual para guardar Factura, Detalle y restar Stock)
        public virtual bool GuardarFactura(FacturaEntidad factura)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                SqlTransaction transaccion = con.BeginTransaction();
                try
                {
                    // A. Insertar cabecera en tabla Factura
                    string qFactura = "INSERT INTO Factura (IdFactura, Fecha, IdUsuario, IdCliente, Total) VALUES (@idFac, @fecha, @idUsu, @idCli, @total)";
                    SqlCommand cmdFac = new SqlCommand(qFactura, con, transaccion);
                    cmdFac.Parameters.AddWithValue("@idFac", factura.IdFactura);
                    cmdFac.Parameters.AddWithValue("@fecha", factura.Fecha);
                    cmdFac.Parameters.AddWithValue("@idUsu", factura.IdUsuario);
                    cmdFac.Parameters.AddWithValue("@idCli", factura.IdCliente);
                    cmdFac.Parameters.AddWithValue("@total", factura.Total);
                    cmdFac.ExecuteNonQuery();

                    // B. Insertar cada fila en Detalle_Factura y restar Stock en Inventario
                    foreach (var item in factura.Detalles)
                    {
                        string qDetalle = @"INSERT INTO Detalle_Factura (IdFactura, IdProducto, Cantidad, Precio_Unitario, Sub_Total) 
                                            VALUES (@idFac, @idProd, @cant, @precio, @sub)";
                        SqlCommand cmdDet = new SqlCommand(qDetalle, con, transaccion);
                        cmdDet.Parameters.AddWithValue("@idFac", factura.IdFactura);
                        cmdDet.Parameters.AddWithValue("@idProd", item.IdProducto);
                        cmdDet.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdDet.Parameters.AddWithValue("@precio", item.PrecioUnitario);
                        cmdDet.Parameters.AddWithValue("@sub", item.SubTotal);
                        cmdDet.ExecuteNonQuery();

                        // C. Restar del Inventario
                        string qStock = "UPDATE Inventario SET Stock = Stock - @cant WHERE IdProducto = @idProd";
                        SqlCommand cmdStock = new SqlCommand(qStock, con, transaccion);
                        cmdStock.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdStock.Parameters.AddWithValue("@idProd", item.IdProducto);
                        cmdStock.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}