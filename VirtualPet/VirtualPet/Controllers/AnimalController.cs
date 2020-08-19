using Application.Services.Classes;
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
                return Ok("Aún no hay animales");
            }
            else
            {
                return Ok(animalsUpdated); /*devolverlo bonico??*/
            }
        }

        [HttpGet] // GET Animals/id
        [Route("{id}")]
        public IActionResult GetAnimalsFromId(int id) /*tener usuarios distintos y pasarlos por la URL???*/
        {
            Animal animalUpdated;
            var animals = getDataService.GetAnimals();
            setDataService.UpdateAnimals(animals.ToList());
            if (animals == null)
            {
                return Ok("There is no animals yet");
            }
            else
            {
                animalUpdated = getDataService.GetAnimalById(id);
                if (animalUpdated == null)
                    return NotFound("That animal does not exist");
                return Ok(animalUpdated); /*Format?*/
            }
        }

        [HttpPost] // POST Animals
        [Route("create")]
        public IActionResult CreateAnimal([FromBody] PayloadCreate values)
        {
            //{
            //"name": "Monito",
            //"type": 0
            //"UserId": X
            //}

            string name = values.Name;
            int type = values.Type;
            //string name, [FromBody] int type
            if (ModelState.IsValid)
            {
                //var x = value.var1.Value; // JToken

            }
            List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
            Animal animal = setDataService.CreateAnimal(name, type, animalList);
            animalList.Add(animal);
            return CreatedAtAction(nameof(CreateAnimal), animal);
        }

        [HttpPost]
        [Route("Feed")]
        public IActionResult FeedAnimals([FromBody] int id)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(id);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.FeedAnimal(animal, animalList))
                {
                    return Ok($"{animal.Name} is full so it can't eat anymore");
                }
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {id}");
            }
            return Ok();
        }


        [HttpPost]
        [Route("Stroke")]
        public IActionResult StrokeAnimal([FromBody] int id)
        {
            try
            {
                Animal animal = getDataService.GetAnimalById(id);
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                if (!setDataService.StrokeAnimal(animal, animalList))
                {
                    return Ok($"{animal.Name} is completely happy and it do not want more strokes");
                }
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {id}");
            }
            return Ok();
        }

        [HttpDelete] // DELETE Animal/id
        [Route("{id}")]
        public IActionResult DeleteAnimal(int id) /*Devolver el resto de animales?*/
        {
            try
            {
                List<Animal> animalList = getDataService.GetAnimals().ToList<Animal>();
                setDataService.DeleteAnimal(id, animalList);
            }
            catch
            {
                return BadRequest($"There is no animal with ID = {id}");
            }
            return Ok();

        }
    }

    /*
     Payload of the create. To get data from post body
     */
    public class PayloadCreate
    {
        public string Name { get; set; }
        public int Type { get; set; }
    }
}
