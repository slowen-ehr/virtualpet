using Application.Services.Interfaces;
using DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Application.Services.Classes
{
    public class SetDataServices : ISetDataServices
    {
        readonly bool dbFlag = true;
        /*
        Creates an Animal object and saves it to the json file.        
        Params:
            name: Name of the animal
            type: Type of the animal, as high as number of classes inherited from Animal
            animalList: List with the animals in json file        
        Return: The Animal object created
        */
        public Animal CreateAnimal(string name, int type, List<Animal> animalsList, User user, List<User> userList)
        {
            Animal animal;
            int newId;

            newId = (animalsList.Count > 0) ? animalsList[animalsList.Count - 1].ID + 1 : 0;

            switch (type) // Add new cases if new animal classes appears
            {
                case 0: animal = new TyrannosaurusRex(newId, name, user.ID); break;
                case 1: animal = new Cat(newId, name, user.ID); break;
                case 2: animal = new Sheep(newId, name, user.ID); break;
                default: throw new InvalidEnumArgumentException($"There is no animal type with Type = {type}");
            }

            animalsList.Add(animal);
            user.Animals.Add(animal.ID);
            RewriteAnimalsDB(animalsList);
            RewriteUsersDB(userList, user);

            return animal;
        }

        /*
        Feeds an animal, decreasing its hungry value.        
        Params:
            animal: The animal to feed
        
        Return:
            true: boolean that indicates the animal was feeded and no error occurred
            false: boolean to indicate an error occurred
        */        
        public bool FeedAnimal(Animal animal)
        {
            if (animal.Hungry == animal.MinStatus)
                return false;
            animal.Feed();
            return true;
        }

        /*
        Strokes an animal, increasing its hapiness value.        
        Params:
            animal: The animal to feed
        
        Return:
            true: boolean that indicate animal was stroked, no error occurred
            false: boolean that indicate an error occurred
        */
        public bool StrokeAnimal(Animal animal)
        {
            if (animal.Hapiness == animal.MaxStatus) // top happiness
                return false;
            animal.Stroke();
            return true;
        }

        /*
        Make an action for an animal.        
        Params:
            animal: The animal to feed
            animalList: List with the animals in json file
            action: The action the animal is going to take
        Return:
            true: boolean that indicate animal takes the action, no error occurred
            false: boolean that indicate an error occurred
        */
        public bool ActionAnimal(Animal animal, List<Animal> animalList, Func<Animal, bool> action)
        {
            DateTime now = DateTime.UtcNow;
            animal.LastUpdatedDate = now;
            if (!action(animal))
                return false;
            RewriteAnimalsDB(animalList, newAnimal: animal);
            return true;
        }

        /*
        Update animals data with the happines decreased and the hungry increased using the params of each animal
        Params:
            animalList: List of animal objects to rewrite the json file.
            newAnimal (optional): New animal object to be inserted in to the animalList and the json file.                
        */
        public List<Animal> UpdateAnimals(List<Animal> animalList)
        {
            List<Animal> animalListUpdated = new List<Animal>();
            foreach (Animal animal in animalList)
            {
                animalListUpdated.Add(animal.Update(DateTime.UtcNow));
            }
            RewriteAnimalsDB(animalListUpdated);
            return animalListUpdated;
        }

        /*
        Update animals data in json file
        Params:
            animalList: List of animal objects to rewrite the json file.
            newAnimal (optional): New animal object to be inserted in to the animalList and the json file.                
        */
        private bool RewriteAnimalsDB(List<Animal> animalList, Animal newAnimal = null)
        {
            string path;
            if (newAnimal != null)
            {
                Animal oldAnimal = animalList.Where(i => i.ID == newAnimal.ID).First();
                var index = animalList.IndexOf(oldAnimal);
                if (index != -1)
                    animalList[index] = newAnimal;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            string strJson = JsonConvert.SerializeObject(animalList, settings);

            if (this.dbFlag)
            {
                string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\")); //Debugging with Visual Studio
                path = Path.Combine(basePath, @"Virtualpet\Data", "AnimalsDB.json");
            }
            else {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "AnimalsDB.json");  // After publishing project
            }            
            System.IO.File.WriteAllText(path, strJson);
            return true;
        }

        /*
        Delete an animal from the json file and the user animals list
        Params:
            animalList: List of animal objects with the animals to rewrite the json file
            newAnimal (optional): New animal object to be inserted in to the animalList and the json file                
        */
        public bool DeleteAnimal(int id, List<Animal> animalList, User user, List<User> userList)
        {
            Animal animalToDelete = animalList.Find(x => x.ID == id);
            animalList.Remove(animalToDelete);
            user = DeleteAnimalFromUser(animalToDelete.ID, user);
            RewriteAnimalsDB(animalList);
            RewriteUsersDB(userList, user);
            return true;
        }

        /*
        Delete an animal from the Animal user animals list
        Params:
            idAnimal: List of animal objects with the animals to rewrite the json file
            user: User who is going to lose the animal
        Return:
            User: User objecto without the animal
        */
        public User DeleteAnimalFromUser(int idAnimal, User user)
        {
            user.Animals.Remove(idAnimal);
            return user;
        }

        /*
        Update user data in json file
        Params:
            userList: List of users objects to rewrite the json file.
            newUser: New user object to be updated in to the json file.                
        */
        private bool RewriteUsersDB(List<User> userList, User newUser)
        {
            string path;
            if (newUser != null)
            {
                User oldUser = userList.Where(i => i.ID == newUser.ID).First();
                var index = userList.IndexOf(oldUser);
                if (index != -1)
                    userList[index] = newUser;
            }
            string strJson = JsonConvert.SerializeObject(userList);
            
            if (this.dbFlag)
            {
                string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\")); // Debugging with visual studio
                path = Path.Combine(basePath, @"Virtualpet\Data", "UsersDB.json");
            }
            else
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "UsersDB.json");  // After publishing project
            }
            System.IO.File.WriteAllText(path, strJson);
            return true;
        }
    }
}
