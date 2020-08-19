using Application.Services.Interfaces;
using DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Application.Services.Classes
{
    public class GetDataServices : IGetDataServices
    {
        /*
        Show the Animals database.
        
        Return: All the animals as an IEnumerable object
        */
        public IEnumerable<Animal> GetAnimals()
        {            
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "AnimalsDB.json");
                if (new FileInfo(path).Length != 0)
                {
                    using (StreamReader jsonStream = System.IO.File.OpenText(path))
                    {
                        var json = jsonStream.ReadToEnd();
                        return JsonConvert.DeserializeObject<IEnumerable<Animal>>(json, settings);
                    }
                }
                else
                {
                    return new List<Animal>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         Find an Animal object from the animal database.        
         Params:
             id: ID of the animal to be found     
         Return: The animal object found
         */
        public Animal GetAnimalById(int id)
        {
            List<Animal> list = GetAnimals().ToList();
            return list.Find(x => x.ID == id);
        }
    }
}

