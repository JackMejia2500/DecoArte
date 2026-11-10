using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DecoArte.Modelos
{
    public class ReservacionEntidad
    {
        public string IdReservacion { get; set; }
        public string IdCliente { get; set; }
        public string DetalleReservacion { get; set; }
        public DateTime FechaEvento { get; set; }
        public string LugarEvento { get; set; }
    }

    public class ReservacionDAO
    {
        private Conexion db = new Conexion();

        // 1. Genera ID con formato RS-001 (Máximo 6 caracteres para varchar(6))
        public string GenerarIdReservacion()
        {
            string nuevoId = "RS-001";
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(IdReservaciones, 4, 3) AS INT)), 0) + 1 FROM Reservaciones WHERE IdReservaciones LIKE 'RS-%'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int siguiente = Convert.ToInt32(cmd.ExecuteScalar());
                    nuevoId = "RS-" + siguiente.ToString("D3");
                }
            }
            catch (Exception ex) { throw new Exception("Error al generar ID: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return nuevoId;
        }

        // 2. Obtiene los clientes para el ComboBox
        public DataTable ObtenerClientes()
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdCliente, Nombre_Apellido FROM Clientes";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn)) { da.Fill(dt); }
            }
            catch (Exception ex) { throw new Exception("Error al cargar clientes: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }

        // 3. Obtiene solo productos de decoración/arreglos para el ComboBox
        public DataTable ObtenerArreglos()
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdProducto, Detalle_Producto FROM Producto";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn)) { da.Fill(dt); }
            }
            catch (Exception ex) { throw new Exception("Error al cargar arreglos: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }

        // 4. Guardar Reservación
        public bool Guardar(ReservacionEntidad res)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "INSERT INTO Reservaciones (IdReservaciones, IdCliente, Detalle_Reservacion, Fecha, Lugar_De_Evento) VALUES (@Id, @IdCli, @Detalle, @Fecha, @Lugar)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", res.IdReservacion);
                    cmd.Parameters.AddWithValue("@IdCli", res.IdCliente);
                    cmd.Parameters.AddWithValue("@Detalle", res.DetalleReservacion);
                    cmd.Parameters.AddWithValue("@Fecha", res.FechaEvento);
                    cmd.Parameters.AddWithValue("@Lugar", res.LugarEvento);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar reserva: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        // 5. Actualizar Reservación
        public bool Actualizar(ReservacionEntidad res)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "UPDATE Reservaciones SET IdCliente = @IdCli, Detalle_Reservacion = @Detalle, Fecha = @Fecha, Lugar_De_Evento = @Lugar WHERE IdReservaciones = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", res.IdReservacion);
                    cmd.Parameters.AddWithValue("@IdCli", res.IdCliente);
                    cmd.Parameters.AddWithValue("@Detalle", res.DetalleReservacion);
                    cmd.Parameters.AddWithValue("@Fecha", res.FechaEvento);
                    cmd.Parameters.AddWithValue("@Lugar", res.LugarEvento);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al actualizar reserva: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        // 6. Eliminar (Cancelar Reserva)
        public bool Eliminar(string id)
        {
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "DELETE FROM Reservaciones WHERE IdReservaciones = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex) { throw new Exception("Error al cancelar reserva: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
        }

        // 7. Listar filtrando por la fecha seleccionada en la agenda
        public DataTable ListarPorFecha(DateTime fecha)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = @"SELECT r.IdReservaciones AS 'ID', c.Nombre_Apellido AS 'Cliente', 
                                        r.Detalle_Reservacion AS 'Detalle', r.Lugar_De_Evento AS 'Lugar' 
                                 FROM Reservaciones r 
                                 INNER JOIN Clientes c ON r.IdCliente = c.IdCliente 
                                 WHERE CONVERT(DATE, r.Fecha) = CONVERT(DATE, @Fecha)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            catch (Exception ex) { throw new Exception("Error al listar agenda: " + ex.Message); }
            finally { db.CerrarConexion(conn); }
            return dt;
        }
    }
}