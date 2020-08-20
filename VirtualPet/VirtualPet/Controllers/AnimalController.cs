﻿using Application.Services.Classes;
using Application.Services.Interfaces;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public AnimalsController()
        {
            this.getDataService = new GetDataServices();
            this.setDataService = new SetDataServices();
        }

        [HttpGet] // GET Animals
        public IActionResult GetAnimals() /*tener usuarios distintos y pasarlos por la URL???*/
        {
            var animals = getDataService.GetAnimals();
            List<Animal> animalsUpdated = setDataService.UpdateAnimals(animals.ToList());
            if (animals == null)
            {
                return Ok("There is no animals yet");
            }
            else
            {
                return Ok(animalsUpdated);
            }
        }

        [HttpGet] // GET Animals/User
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
        }

        [HttpPost] // POST Animals
        public IActionResult CreateAnimal([FromBody] PayloadCreatesssss values)
        {
            /*
            {
            "name": "Carpacho",
            "type": 0,
            "UserId": 0
            }
            */
            if (ModelState.IsValid)
            {
                string name = values.Name;
                int type = values.Type;
                int userId = values.UserId;

                // EXCEPCION PARA USERID QUE NO EXISTE Y TIPO DE ANIMAL QUE NO EXISTE
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                User user = getDataService.GetUserById(userId);
                List<User> userList = getDataService.GetUsers().ToList<User>();

                Animal animal = setDataService.CreateAnimal(name, type, animalList, user, userList);
                animalList.Add(animal);
                return CreatedAtAction(nameof(CreateAnimal), animal);
            }
            else
            {
                return BadRequest("The animal could not be created");
            }
        }

        [HttpPost]
        [Route("Feed")] // POST Animals
        public IActionResult FeedAnimals([FromBody] int animalId)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(animalId);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.FeedAnimal(animal, animalList))
                {
                    return Ok($"{animal.Name} is full so it can't eat anymore");
                }
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {animalId}");
            }
            return Ok();
        }


        [HttpPost]
        [Route("Stroke")]
        public IActionResult StrokeAnimal([FromBody] int animalId)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(animalId);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.StrokeAnimal(animal, animalList))
                {
                    return Ok($"{animal.Name} is completely happy and it do not want more strokes");
                }
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {animalId}");
            }
            return Ok();
        }

        [HttpDelete] // DELETE Animals/id
        [Route("{animalId}")]
        public IActionResult DeleteAnimal(int animalId) /*Devolver el resto de animales?*/
        {
            try
            {
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                List<User> userList = getDataService.GetUsers().ToList<User>();
                int userId = getDataService.GetAnimalById(animalId).UserId;
                User user = getDataService.GetUserById(userId);
                setDataService.DeleteAnimal(animalId, animalList, user, userList);
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {animalId}");
            }
            return Ok();
        }
    }

    //public class PayloadCreate
    //{
    //    public string Name { get; set; }
    //    public int Type { get; set; }
    //    public int UserId { get; set; }
    //}
}
