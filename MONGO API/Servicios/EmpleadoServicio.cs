using MONGO_API.modelos;
using MONGO_API.Settings;
using MongoDB.Driver;
namespace MONGO_API.EmpleadoServicio
{


    public class EmployeeService : IEmployeeService
    {
    private readonly IMongoCollection<Empleados> _employees;


        public EmployeeService(MongoDbSettings settings)
        {
            // Crear cliente y obtener colección
            var client = new MongoClient(settings.ConnectionString);
            var db     = client.GetDatabase(settings.DatabaseName);
            _employees = db.GetCollection<Empleados>("Employees");


            // Índice simple en LastName para acelerar búsquedas por apellido
            var indexLastName = Builders<Empleados>.IndexKeys
                .Ascending(e => e.LastName);
            _employees.Indexes.CreateOne(new CreateIndexModel<Empleados>(indexLastName));


            // Índice compuesto en Department + HireDate para consultas agrupadas o filtradas
            var indexDeptHire = Builders<Empleados>.IndexKeys
                .Ascending(e => e.Department)
                .Descending(e => e.HireDate);
            _employees.Indexes.CreateOne(new CreateIndexModel<Empleados>(indexDeptHire));
        }


        public async Task<List<Empleados>> GetAsync() =>
            await _employees.Find(emp => true).ToListAsync();


        public async Task<Empleados?> GetAsync(string id) =>
            await _employees.Find(emp => emp.Id == id).FirstOrDefaultAsync();


        public async Task CreateAsync(Empleados emp) =>
            await _employees.InsertOneAsync(emp);


        public async Task UpdateAsync(string id, Empleados emp) =>
            await _employees.ReplaceOneAsync(e => e.Id == id, emp);


        public async Task RemoveAsync(string id) =>
            await _employees.DeleteOneAsync(e => e.Id == id);

    }
}