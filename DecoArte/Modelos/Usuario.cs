using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DecoArte.Modelos
{
    public class UsuarioEntidad
    {
        public string IdUsuario { get; set; }
        public string NombreUsuario { get; set; } // Para el Login
        public string Contrasena { get; set; }
        public string NombreApellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaContratacion { get; set; }
        public string Rol { get; set; }
        public string Permisos { get; set; } // Guardaremos un texto: "Facturacion, Inventario"
    }

    public class UsuarioDAO
    {
        private Conexion db = new Conexion();

        public string GenerarIdUsuario()
        {
            string nuevoId = "US-001";
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdUsuario, 4, 3) AS INT)), 0) + 1 FROM Usuarios WHERE IdUsuario LIKE 'US-%'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                    nuevoId = "US-" + siguiente.ToString("D3");
                }
            }
            catch (Exception ex) { throw new Exception("Error al generar ID: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return nuevoId;
        }

        public bool Guardar(UsuarioEntidad usu)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "INSERT INTO Usuarios (IdUsuario, NombreUsuario, Contrasena, Nombre_Apellido, Telefono, Direccion, Fecha_Contratacion, Rol, Permisos) " +
                               "VALUES (@Id, @Usu, @Pass, @Nom, @Tel, @Dir, @Fecha, @Rol, @Permisos)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", usu.IdUsuario);
                    cmd.Parameters.AddWithValue("@Usu", usu.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Pass", usu.Contrasena);
                    cmd.Parameters.AddWithValue("@Nom", usu.NombreApellido);
                    cmd.Parameters.AddWithValue("@Tel", usu.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dir", usu.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fecha", usu.FechaContratacion);
                    cmd.Parameters.AddWithValue("@Rol", usu.Rol ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Permisos", usu.Permisos ?? (object)DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public bool Actualizar(UsuarioEntidad usu)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "UPDATE Usuarios SET NombreUsuario = @Usu, Contrasena = @Pass, Nombre_Apellido = @Nom, " +
                               "Telefono = @Tel, Direccion = @Dir, Fecha_Contratacion = @Fecha, Rol = @Rol, Permisos = @Permisos WHERE IdUsuario = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", usu.IdUsuario);
                    cmd.Parameters.AddWithValue("@Usu", usu.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Pass", usu.Contrasena);
                    cmd.Parameters.AddWithValue("@Nom", usu.NombreApellido);
                    cmd.Parameters.AddWithValue("@Tel", usu.Telefono ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dir", usu.Direccion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fecha", usu.FechaContratacion);
                    cmd.Parameters.AddWithValue("@Rol", usu.Rol ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Permisos", usu.Permisos ?? (object)DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al actualizar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public bool Eliminar(string idUsuario)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "DELETE FROM Usuarios WHERE IdUsuario = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idUsuario);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al eliminar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        public DataTable Listar()
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdUsuario, NombreUsuario, Nombre_Apellido, Rol, Telefono FROM Usuarios";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn)) { da.Fill(dt); }
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
                string query = "SELECT IdUsuario, NombreUsuario, Nombre_Apellido, Rol, Telefono FROM Usuarios WHERE IdUsuario LIKE @Texto OR Nombre_Apellido LIKE @Texto";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            catch (Exception ex) { throw new Exception("Error al buscar: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }

        // Método extra para cargar los datos completos al hacer doble clic
        public UsuarioEntidad ObtenerPorId(string id)
        {
            UsuarioEntidad usu = null;
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT * FROM Usuarios WHERE IdUsuario = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usu = new UsuarioEntidad
                            {
                                IdUsuario = dr["IdUsuario"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                Contrasena = dr["Contrasena"].ToString(),
                                NombreApellido = dr["Nombre_Apellido"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                Direccion = dr["Direccion"].ToString(),
                                FechaContratacion = dr["Fecha_Contratacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Contratacion"]) : DateTime.Now,
                                Rol = dr["Rol"].ToString(),
                                Permisos = dr["Permisos"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al obtener usuario: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return usu;
        }
        // Agrégalo dentro de public class UsuarioDAO
        public UsuarioEntidad ValidarLogin(string nombreUsuario, string contrasena)
        {
            UsuarioEntidad usu = null;
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                // Solo traemos lo necesario para la sesión, incluyendo los Permisos
                string query = "SELECT IdUsuario, NombreUsuario, Nombre_Apellido, Permisos FROM Usuarios WHERE NombreUsuario = @Usu AND Contrasena = @Pass";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Usu", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Pass", contrasena);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usu = new UsuarioEntidad
                            {
                                IdUsuario = dr["IdUsuario"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                NombreApellido = dr["Nombre_Apellido"].ToString(),
                                Permisos = dr["Permisos"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al validar login: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return usu;
        }
    }
}