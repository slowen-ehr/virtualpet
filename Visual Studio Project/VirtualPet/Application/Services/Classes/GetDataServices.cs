using Application.Services.Interfaces;
using DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.Services.Classes
{
    public class GetDataServices : IGetDataServices
    {
        readonly bool dbFlag = true;
        /*
        Show the Animals database.
        
        Return: All the animals as an IEnumerable object
        */
        public IEnumerable<Animal> GetAnimals()
        {            
            try
            {
                string path;
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                if (this.dbFlag)
                {
                    string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\")); //Debugging with Visual Studio
                    path = Path.Combine(basePath, @"Virtualpet\Data", "AnimalsDB.json");
                }
                else
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "AnimalsDB.json");  // After publishing project
                }
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
            Animal animal = list.Find(x => x.ID == id);
            if (animal == null) throw new NullReferenceException($"There is no animall with ID = {id}");
            return animal;
        }

        /*
        Show the Users database.
        
        Return: All the users as an IEnumerable object
        */
        public IEnumerable<User> GetUsers()
        {
            try
            {
                string path;
                if (this.dbFlag)
                {
                    string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\")); // Debugging with visual studio
                    path = Path.Combine(basePath, @"Virtualpet\Data", "UsersDB.json");
                }
                else
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "UsersDB.json");  // After publishing project
                }
                if (new FileInfo(path).Length != 0)
                {
                    using (StreamReader jsonStream = System.IO.File.OpenText(path))
                    {
                        var json = jsonStream.ReadToEnd();
                        return JsonConvert.DeserializeObject<IEnumerable<User>>(json);
                    }
                }
                else
                {
                    return new List<User>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         Find an User object from the users database.        
         Params:
             id: ID of the user to be found     
         Return: The user object found
         */
        public User GetUserById(int id)
        {
            List<User> list = GetUsers().ToList();
            User user = list.Find(x => x.ID == id);
            if (user == null) throw new NullReferenceException($"There is no user with ID = {id}");
            return user;
        }
    }
}

