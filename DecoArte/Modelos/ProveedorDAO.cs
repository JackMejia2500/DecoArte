using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DecoArte.Modelos
{
    public class ProveedorEntidad
    {
        public string IdProveedor { get; set; }
        public string NombreEmpresa { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }

    public class ProveedorDAO
    {
        private Conexion db = new Conexion();

        // Genera ID con formato PR-001 (máximo 6 caracteres)
        public string GenerarIdProveedor()
        {
            string nuevoId = "PR-001";
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdProveedor, 4, 3) AS INT)), 0) + 1 FROM Proveedor WHERE IdProveedor LIKE 'PR-%'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                    nuevoId = "PR-" + siguiente.ToString("D3");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar ID de proveedor: " + ex.Message);
            }
            finally { db.CerrarConexion(conn); }
            return nuevoId;
        }

        public bool Guardar(ProveedorEntidad proveedor)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "INSERT INTO Proveedor (IdProveedor, Nombre_Empresa, RUC, Direccion, Telefono) VALUES (@Id, @Nombre, @RUC, @Dir, @Tel)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", proveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("@Nombre", proveedor.NombreEmpresa);
                    cmd.Parameters.AddWithValue("@RUC", proveedor.RUC);
                    cmd.Parameters.AddWithValue("@Dir", proveedor.Direccion);
                    cmd.Parameters.AddWithValue("@Tel", proveedor.Telefono);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public bool Actualizar(ProveedorEntidad proveedor)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "UPDATE Proveedor SET Nombre_Empresa = @Nombre, RUC = @RUC, Direccion = @Dir, Telefono = @Tel WHERE IdProveedor = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", proveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("@Nombre", proveedor.NombreEmpresa);
                    cmd.Parameters.AddWithValue("@RUC", proveedor.RUC);
                    cmd.Parameters.AddWithValue("@Dir", proveedor.Direccion);
                    cmd.Parameters.AddWithValue("@Tel", proveedor.Telefono);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al actualizar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public bool Eliminar(string idProveedor)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "DELETE FROM Proveedor WHERE IdProveedor = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idProveedor);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al eliminar: (Verifique si este proveedor tiene compras registradas) " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdProveedor, Nombre_Empresa, RUC, Telefono, Direccion FROM Proveedor";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }
            catch (Exception ex) { throw new Exception("Error al listar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }

        public DataTable Buscar(string texto)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdProveedor, Nombre_Empresa, RUC, Telefono, Direccion FROM Proveedor WHERE IdProveedor LIKE @Texto OR Nombre_Empresa LIKE @Texto";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al buscar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }
    }
}