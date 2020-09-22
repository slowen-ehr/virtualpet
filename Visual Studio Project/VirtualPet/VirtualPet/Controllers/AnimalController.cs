using Application.Services.Classes;
using Application.Services.Interfaces;
using DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace VirtualPet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnimalsController : ControllerBase
    {
        #region private properties

        private readonly IGetDataServices getDataService;
        private readonly ISetDataServices setDataService;
        #endregion
        public AnimalsController(IGetDataServices getData = null, ISetDataServices setData = null)
        {

            this.getDataService = getData ?? new GetDataServices();
            this.setDataService = setData ?? new SetDataServices();
        }

        /*
        Endpoint: GET Animals
        Show all the animals from the json file. This is a debug endpoint, to get the status of every animal created.
        
        Return:
            OK 200: No errors and the animal list. A message if there is no animals yet in json file.
        */
        [HttpGet]
        public IActionResult GetAnimals()
        {
            var animals = getDataService.GetAnimals();
            List<Animal> animalsUpdated = setDataService.UpdateAnimals(animals.ToList());

            return (animals.Count() == 0) ? Ok("There is no animals yet") : Ok(animalsUpdated);
        }

        /*
        Endpoint: GET Animals/User/id
        Show all the animals tha the user with ID = id owns.

        Return:
            OK 200: No errors and the users animals. A message if there are no animals en json file yet
            NotFound 404: The user has no animals yet.
            BadRequest 400: A message if that user do not exists.
        */
        [HttpGet]
        [Route("User/{idUser}")]
        public IActionResult GetAnimalsFromUser(int idUser)
        {
            Animal animalUpdated;
            User user;
            var animals = getDataService.GetAnimals();
            setDataService.UpdateAnimals(animals.ToList());
            if (animals == null)
            {
                return Ok("There is no animals yet");
            }
            else
            {
                try
                {
                    user = getDataService.GetUserById(idUser);
                    List<Animal> AnimalsUserList = new List<Animal>();
                    foreach (int animalId in user.Animals)
                    {
                        animalUpdated = getDataService.GetAnimalById(animalId);
                        AnimalsUserList.Add(animalUpdated);
                    }
                    if (!AnimalsUserList.Any())
                        return NotFound("That user has no animals");
                    return Ok(AnimalsUserList);
                }
                catch (NullReferenceException userException) { return BadRequest(userException.Message); }
                catch (InvalidEnumArgumentException animalException) { return BadRequest(animalException.Message); }
            }
        }

        /*
        Endpoint: POST Animals
        Params Note: Name, type and owner of the animal given by a json from Body.
        Body Format example:
            {
            "name": "AnimalName",
            "type": 0,
            "userId": 0
            }
            
        Creates a new animal.

        Return:
            OK 200: No errors, the animal was created. Shows the animal data
            BadRequest 400: A message if that user do not exists.
            BadRequest 400: A message if that animal type do not exists.
        */
        [HttpPost]
        public IActionResult CreateAnimal([FromBody] PayloadCreate values)
        {
            if (ModelState.IsValid)
            {
                string name = values.Name;
                int type = values.Type;
                int userId = values.UserId;

                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                try
                {
                    User user = getDataService.GetUserById(userId);
                    List<User> userList = getDataService.GetUsers().ToList<User>();
                    Animal animal = setDataService.CreateAnimal(name, type, animalList, user, userList);
                    animalList.Add(animal);
                    return CreatedAtAction(nameof(CreateAnimal), animal);
                }
                catch (NullReferenceException userException) { return BadRequest(userException.Message); }
                catch (InvalidEnumArgumentException animalException) { return BadRequest(animalException.Message); }

            }
            else return BadRequest("The animal could not be created");
        }

        /*
        Endpoint: POST Animals/Feed
        Params Note: animalId from Body.
        Body Format example:
            5
            
        Feeds a created animal.
        It is assumed the user will only order actions to their animals from the client side.

        Return:
            OK 200: No errors, the animal was feeded. Shows a message if the animal does not need more food.
            BadRequest 400: A message if that animal do not exists.
        */
        [HttpPost]
        [Route("Feed")]
        public IActionResult FeedAnimal([FromBody] int animalId)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(animalId);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.ActionAnimal(animal, animalList, setDataService.FeedAnimal))
                {
                    return Ok($"{animal.Name} is full so it can't eat anymore");
                }
            }
            catch (NullReferenceException animalException) { return BadRequest(animalException.Message); }
            return Ok("");
        }

        /*
        Endpoint: POST Animals/Stroke
        Params Note: animalId from Body.
        Body Format example:
            5
            
        Caress an animal.
        It is assumed the user will only order actions to their animals from the client side.

        Return:
            OK 200: The animal has been petted. Shows a message if the animal does not need more caress.
            BadRequest 400: A message if that animal do not exists.
        */
        [HttpPost]
        [Route("Stroke")]
        public IActionResult StrokeAnimal([FromBody] int animalId)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(animalId);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.ActionAnimal(animal, animalList, setDataService.StrokeAnimal))
                {
                    return Ok($"{animal.Name} is completely happy and it do not want more strokes");
                }
            }
            catch (NullReferenceException animalException) { return BadRequest(animalException.Message); }
            return Ok("");
        }

        /*
        Endpoint: DELETE Animals/id            
        Deletes the animal with the id given by the uri.

        Return:
            OK 200: The animal has been deleted.
            BadRequest 400: A message if that animal do not exists.
        */
        [HttpDelete] // 
        [Route("{animalId}")]
        public IActionResult DeleteAnimal(int animalId)
        {
            try
            {
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                List<User> userList = getDataService.GetUsers().ToList<User>();
                int userId = getDataService.GetAnimalById(animalId).UserId;
                User user = getDataService.GetUserById(userId);
                setDataService.DeleteAnimal(animalId, animalList, user, userList);
            }
            catch (NullReferenceException animalException) { return BadRequest(animalException.Message); }
            return Ok("");
        }
    }
}
