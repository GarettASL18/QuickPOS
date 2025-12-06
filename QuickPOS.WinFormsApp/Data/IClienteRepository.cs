using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public interface IClienteRepository
{
    Cliente? GetById(int id);
    List<Cliente> GetAll();
    int Create(Cliente c); // o void Create(Cliente c)
    void Update(Cliente c);
    void Delete(int id);

}
