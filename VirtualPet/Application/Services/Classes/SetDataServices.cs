using Application.Services.Interfaces;
using DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.Services.Classes
{
    public class SetDataServices : ISetDataServices
    {
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
            if (animalsList.Count > 0)
            {
                newId = animalsList[animalsList.Count - 1].ID + 1;
            }
            else
            {
                newId = 0;
            }

            switch (type)
            {
                case 1:
                    animal = new Cat(newId, name, user.ID);
                    break;
                case 2:
                    animal = new Sheep(newId, name, user.ID);
                    break;
                default:
                    animal = new TyrannosaurusRex(newId, name, user.ID);
                    break;
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
            animalList: List with the animals in BD;
        
        Return:
            true: boolean that indicates the animal was feeded and no error occurred
            false: boolean to indicate an error occurred
        */
        public bool FeedAnimal(Animal animal, List<Animal> animalList)
        {
            //UpdateAnimals(animalList);
            DateTime now = DateTime.UtcNow;
            animal.LastUpdatedDate = now;
            if (animal.Hungry == animal.MinStatus)
                return false;
            animal.Feed();

            RewriteAnimalsDB(animalList, newAnimal: animal);
            return true;
        }

        /*
        Strokes an animal, increasing its hapiness value.        
        Params:
            animal: The animal to feed
            animalList: List with the animals in json file;
        
        Return:
            true: boolean that indicate animal was stroked, no error occurred
            false: boolean that indicate an error occurred
        */
        public bool StrokeAnimal(Animal animal, List<Animal> animalList)
        {
            //UpdateAnimals(animalList);
            DateTime now = DateTime.UtcNow;
            animal.LastUpdatedDate = now;
            if (animal.Hapiness == animal.MaxStatus) // top happiness
                return false;
            animal.Stroke();

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
            // replace the new animal into the list if exist
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

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "AnimalsDB.json");
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
            // replace the new User into the list
            if (newUser != null)
            {
                User oldUser = userList.Where(i => i.ID == newUser.ID).First();
                var index = userList.IndexOf(oldUser);
                if (index != -1)
                    userList[index] = newUser;
            }
            string strJson = JsonConvert.SerializeObject(userList);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "UsersDB.json");
            System.IO.File.WriteAllText(path, strJson);
            return true;
        }
    }
}
