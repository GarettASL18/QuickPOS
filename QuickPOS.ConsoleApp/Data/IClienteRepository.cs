using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public interface IClienteRepository
{
    Cliente? GetById(int id);
    List<Cliente> GetAll();
    void Create(Cliente c);
}
