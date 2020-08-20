using DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Interfaces
{
    public interface ISetDataServices
    {
        Animal CreateAnimal(string name, int type, List<Animal> animalsList, User user, List<User> userList);
        bool FeedAnimal(Animal animal, List<Animal> animalsList);
        bool StrokeAnimal(Animal animal, List<Animal> animalsList);
        List<Animal> UpdateAnimals(List<Animal> animalList);
        bool DeleteAnimal(int id, List<Animal> animalList, User user, List<User> userList);
    }
}
