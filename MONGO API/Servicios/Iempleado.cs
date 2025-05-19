using MONGO_API.modelos;

public interface IEmployeeService
    {
        Task<List<Empleados>> GetAsync();
        Task<Empleados?> GetAsync(string id);
        Task CreateAsync(Empleados emp);
        Task UpdateAsync(string id, Empleados emp);
        Task RemoveAsync(string id);
    }