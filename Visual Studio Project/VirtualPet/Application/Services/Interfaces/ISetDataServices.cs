using DTO;
using System;
using System.Collections.Generic;

namespace Application.Services.Interfaces
{
    public interface ISetDataServices
    {
        Animal CreateAnimal(string name, int type, List<Animal> animalsList, User user, List<User> userList);
        bool FeedAnimal(Animal animal);
        bool StrokeAnimal(Animal animal);
        bool ActionAnimal(Animal animal, List<Animal> animalList, Func<Animal, bool> action);
        List<Animal> UpdateAnimals(List<Animal> animalList);
        bool DeleteAnimal(int id, List<Animal> animalList, User user, List<User> userList);
    }
}
