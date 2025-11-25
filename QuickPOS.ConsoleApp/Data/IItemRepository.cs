using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public interface IItemRepository
{
    Item? GetById(int id);
    List<Item> GetAll();
}
