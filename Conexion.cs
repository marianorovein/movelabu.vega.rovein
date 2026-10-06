using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace Movelabu.Clases
{
    public static class Conexion
    {
        // XAMPP por defecto: usuario root, sin contraseña, puerto 3306
        private static readonly string cadena =
            "Server=localhost;Port=3306;Database=movelabu_escritorio;Uid=root;Pwd=;";

        public static MySqlConnection Obtener()
        {
            return new MySqlConnection(cadena);
        }
    }
}