using DecoArte.conexion;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DecoArte.Modelos
{
    public class ClienteDAO
    {
        private Conexion db = new Conexion();

        public bool Guardar(Cliente cliente)
        {
            bool exito = false;
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "INSERT INTO Clientes (IdCliente, Nombre_Apellido, Telefono, Direccion) VALUES (@IdCliente, @Nombre, @Telefono, @Direccion)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.NombreApellido);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                    exito = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en la base de datos: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn); 
            }
            return exito;
        }

        public bool Actualizar(Cliente cliente)
        {
            bool exito = false;
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "UPDATE Clientes SET Nombre_Apellido = @Nombre, Telefono = @Telefono, Direccion = @Direccion WHERE IdCliente = @IdCliente";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.NombreApellido);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);

                    exito = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la base de datos: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
            return exito;
        }

        public List<Cliente> Listar()
        {
            List<Cliente> clientes = new List<Cliente>();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "SELECT IdCliente, Nombre_Apellido, Telefono, Direccion FROM Clientes";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                IdCliente = reader["IdCliente"].ToString(),
                                NombreApellido = reader["Nombre_Apellido"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Direccion = reader["Direccion"].ToString()
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
            return clientes;
        }


        public bool Eliminar(string idCliente)
        {
            bool exito = false;
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
                string query = "DELETE FROM Clientes WHERE IdCliente = @IdCliente";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                    exito = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar en la base de datos: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
            return exito;
        }
        public List<Cliente> Buscar(string texto)
        {
            List<Cliente> lista = new List<Cliente>();
            SqlConnection conn = null;
            try
            {
                conn = db.ObtenerConexion();
               
                string query = "SELECT IdCliente, Nombre_Apellido, Telefono, Direccion FROM Clientes WHERE IdCliente LIKE @Texto OR Nombre_Apellido LIKE @Texto";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    
                    cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) // Lee cada fila del resultado el while es necesario para recorrer todas
                                              // las filas del resultado de la consulta. Si no se utiliza,
                                              // solo se leerá la primera fila y se perderán los demás resultados.
                        {
                            Cliente cliente = new Cliente
                            {
                                IdCliente = reader["IdCliente"].ToString(),
                                NombreApellido = reader["Nombre_Apellido"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Direccion = reader["Direccion"].ToString()
                            };
                            lista.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex) // el catch es necesario para capturar cualquier excepción que pueda ocurrir durante la ejecución del código dentro del bloque try.
                                 // Si ocurre una excepción, el flujo de ejecución se transfiere al bloque catch, donde se puede manejar la excepción de manera
            {
                throw new Exception("Error al buscar clientes: " + ex.Message);
            }
            finally
            {
                db.CerrarConexion(conn);
            }
            return lista;
        }

    }
}
