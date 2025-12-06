using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data
{
    public interface IItemRepository
    {
        List<Item> GetAll();
        Item? GetById(int id);
        int Create(Item item);
        void Update(Item item);
        void Delete(int id);
    }
}