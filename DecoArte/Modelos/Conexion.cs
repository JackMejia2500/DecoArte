using System;
using System.Data;
using Microsoft.Data.SqlClient; 

namespace DecoArte.conexion
{
    public class Conexion
    {
       
        private readonly string CadenaConexion = "Data Source=localhost;" +
            "Initial Catalog=DecoArte;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        
        public SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(CadenaConexion);
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
            return conexion;
        }

       
        public void CerrarConexion(SqlConnection conexion)
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }
    }
}