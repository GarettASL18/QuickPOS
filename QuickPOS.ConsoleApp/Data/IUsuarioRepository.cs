using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data
{
    public interface IUsuarioRepository
    {
        Usuario? GetById(int id);
        Usuario? GetByUsername(string username);
        List<Usuario> GetAll();
        int Create(Usuario u); // retorna id
        void Update(Usuario u);
        void Delete(int id);
    }
}
