using MongoDB.Driver;

namespace Movelabu.Clases
{
    public static class ConexionMongo
    {
        // El mismo link que usaste en Compass (cluster en MongoDB Atlas)
        private static readonly MongoClient cliente = new MongoClient(
            "mongodb+srv://movelabu:Movelabu2026@cluster0.0z0y1jx.mongodb.net/?serverSelectionTimeoutMS=5000");

        // La base "movelabu" que creamos en Compass
        public static IMongoDatabase Base => cliente.GetDatabase("movelabu");
    }
}