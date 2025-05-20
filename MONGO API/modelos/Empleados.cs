using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MONGO_API.modelos
{



    public class Empleados
    {
        // MongoDB guardará este campo como ObjectId
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null;  // Inicializar como null


        // Nombre del empleado
        public string FirstName { get; set; } = null!;


        // Apellido
        public string LastName { get; set; } = null!;


        // Departamento al que pertenece
        public string Department { get; set; } = null!;


        // Fecha de contratación
        public DateTime HireDate { get; set; }


        // Salario
        public decimal Salary { get; set; }
    }
}

