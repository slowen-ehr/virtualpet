using DTO;
using System.Collections.Generic;

namespace Application.Services.Interfaces
{
    public interface IGetDataServices
    {
        IEnumerable<Animal> GetAnimals();
        Animal GetAnimalById(int id);
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
    }
}
