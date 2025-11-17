using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ProyectoWishList.Data
{
    internal class ConexionSQL
    {
        private static readonly string Connection = @"Server=DESKTOP-CUHOBBB\MYSQL;Database=WishlistDB;Integrated Security=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(Connection);
        }
    }
}
