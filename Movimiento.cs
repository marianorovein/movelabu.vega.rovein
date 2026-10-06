using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Movelabu.Clases
{
    // Cómo es cada documento de la colección "movimientos"
    public class Movimiento
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("fecha")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fecha { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("usuario")]
        public string Usuario { get; set; }
    }

    // Para guardar y leer movimientos desde cualquier parte del programa
    public static class Movimientos
    {
        private static IMongoCollection<Movimiento> Coleccion =>
            ConexionMongo.Base.GetCollection<Movimiento>("movimientos");

        public static void Registrar(string tipo, string descripcion)
        {
            try
            {
                Coleccion.InsertOne(new Movimiento
                {
                    Fecha = DateTime.Now,
                    Tipo = tipo,
                    Descripcion = descripcion,
                    Usuario = Sesion.Usuario
                });
            }
            catch (Exception)
            {
                // Si no hay internet o Mongo no responde, el programa sigue funcionando
            }
        }

        public static List<Movimiento> Ultimos(int cantidad)
        {
            return Coleccion.Find(_ => true)
                            .SortByDescending(m => m.Fecha)
                            .Limit(cantidad)
                            .ToList();
        }
    }
}