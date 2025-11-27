using QuickPOS.Models;
using System.Collections.Generic;
public interface IItemRepository
{
    Item? GetById(int id);
    List<Item> GetAll();
    int Create(Item item); // retorna ItemId (o void)
    void Update(Item item);
    void Delete(int id);
}
