using DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Interfaces
{
    public interface IGetDataServices
    {
        IEnumerable<Animal> GetAnimals();
        Animal GetAnimalById(int id);
    }
}
