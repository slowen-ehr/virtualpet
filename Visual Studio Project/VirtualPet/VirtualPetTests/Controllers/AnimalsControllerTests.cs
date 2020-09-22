using Application.Services.Interfaces;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace VirtualPet.Controllers.Tests
{
    [TestClass()]
    public class AnimalsControllerTests
    {
        /* Test GetAnimals method with out animals */
        [TestMethod()]
        public void GetAnimalsTestWithoutAnimals()
        {
            // arrange
            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(new List<Animal>());

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.UpdateAnimals(new List<Animal>())).Returns(new List<Animal>());

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.GetAnimals();
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("There is no animals yet", okResult.Value);
        }

        /* Test GetAnimals method with animals */
        [TestMethod()]
        public void GetAnimalsTestWithAnimals()
        {
            List<Animal> animalList = new List<Animal>();
            animalList.Add(new Sheep(0, "Dory", 0));
            animalList.Add(new Sheep(1, "Clone", 0));

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.UpdateAnimals(animalList)).Returns(animalList);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.GetAnimals();
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.ReferenceEquals(animalList, okResult.Value);
        }

        /* Test GetAnimalsFromUser method with an user with animals */
        [TestMethod()]
        public void GetAnimalsFromUserIsUserTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            User u = new User
            {
                ID = 0,
                Animals = new List<int> { 0, 1 },
                Name = "UserMocked"
            };

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(gu => gu.GetUserById(0)).Returns(u);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.UpdateAnimals(animalList)).Returns(animalList);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.GetAnimalsFromUser(0);
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.ReferenceEquals(animalList, okResult.Value);
        }
        
        /* Test GetAnimalsFromUser method with an user with no animals */
        [TestMethod()]
        public void GetAnimalsFromUserWithoutAnimalsTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 1),
                new Sheep(1, "Clone", 1)
            };
            User user = new User
            {
                ID = 0,
                Animals = new List<int> { },
                Name = "UserMocked"
            };

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(gu => gu.GetUserById(0)).Returns(user);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.UpdateAnimals(animalList)).Returns(animalList);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.GetAnimalsFromUser(0);
            var NotFoundResult = result as NotFoundObjectResult;

            // assert
            Assert.IsNotNull(NotFoundResult);
            Assert.AreEqual(404, NotFoundResult.StatusCode);
            Assert.ReferenceEquals("That user has no animals", NotFoundResult.Value);
        }

        /* Test GetAnimalsFromUser method with an user that does not exists */
        [TestMethod()]
        public void GetAnimalsFromUserIsNullTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 1),
                new Sheep(1, "Clone", 1)
            };

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(gu => gu.GetUserById(0)).Returns((User)null);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.UpdateAnimals(animalList)).Returns(animalList);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.GetAnimalsFromUser(0);
            var BadResult = result as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(BadResult);
            Assert.AreEqual(400, BadResult.StatusCode);
            Assert.ReferenceEquals("There is no user with ID = 25", BadResult.Value);
        }

        /* Test CreateAnimal method */
        [TestMethod()]
        public void CreateAnimalWithCorrectParametersTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            Animal animalCreated = new Sheep(2, "CloneTwo", 0);
            User user = new User
            {
                ID = 0,
                Animals = new List<int> { 0, 1 },
                Name = "UserMocked"
            };
            List<User> listUser = new List<User>();
            listUser.Add(user);

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(gu => gu.GetUsers()).Returns(listUser);
            mockGet.Setup(gu => gu.GetUserById(0)).Returns(user);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.CreateAnimal("CloneTwo", 2, animalList, user, listUser)).Returns(animalCreated);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.CreateAnimal(new PayloadCreate
            {
                Name = "CloneTwo",
                Type = 2,
                UserId = 0
            });
            var createdResult = result as CreatedAtActionResult;

            // assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.ReferenceEquals(animalCreated, createdResult.Value);
        }

        /* Test CreateAnimal method using wrong parameters */
        [TestMethod()]
        public void CreateAnimalWithWrongParametersTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            Animal animalCreated = new Sheep(2, "CloneTwo", 0);
            List<User> listUser = new List<User>();

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(gu => gu.GetUsers()).Returns(listUser);
            mockGet.Setup(gu => gu.GetUserById(25)).Returns((User)null);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.CreateAnimal("CloneTwo", 2, animalList, null, listUser)).Returns((Animal)null);

            // arrange
            var controller = new AnimalsController(mockGet.Object);

            // act
            var result = controller.CreateAnimal(new PayloadCreate
            {
                Name = "CloneTwo",
                Type = 2,
                UserId = 25
            });
            var BadResult = result as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(BadResult);
            Assert.AreEqual(400, BadResult.StatusCode);
            Assert.ReferenceEquals("There is no user with ID = 25", BadResult.Value);
        }

        /* Test FeedAnimal method */
        [TestMethod()]
        public void FeedAnimalTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(repo => repo.GetAnimalById(0)).Returns(animalList[0]);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.ActionAnimal(animalList[0], animalList, It.IsAny<Func<Animal, bool>>())).Returns(true);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.FeedAnimal(0);
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        /* Test FeedAnimal method with an animal that can not eat more */
        [TestMethod()]
        public void FeedAnimalFullTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };


            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(repo => repo.GetAnimalById(0)).Returns(animalList[0]);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.ActionAnimal(animalList[0], animalList, It.IsAny<Func<Animal, bool>>())).Returns(false);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.FeedAnimal(0);
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.ReferenceEquals($"{animalList[0].Name} is full so it can't eat anymore", okResult.Value);
        }

        /* Test StrokeAnimal method */
        [TestMethod()]
        public void StrokeAnimalTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };


            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(repo => repo.GetAnimalById(0)).Returns(animalList[0]);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.ActionAnimal(animalList[0], animalList, It.IsAny<Func<Animal, bool>>())).Returns(true);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.StrokeAnimal(0);
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        /* Test StrokeAnimal method with an animal that can not be happier */
        [TestMethod()]
        public void StrokeAnimalHappyTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(repo => repo.GetAnimalById(0)).Returns(animalList[0]);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.ActionAnimal(animalList[0], animalList, It.IsAny<Func<Animal, bool>>())).Returns(false);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.FeedAnimal(0);
            var okResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.ReferenceEquals($"{animalList[0]} is completely happy and it do not want more strokes", okResult.Value);
        }

        /* Test DeleteAnimal method */
        [TestMethod()]
        public void DeleteAnimalTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            User user = new User
            {
                ID = 0,
                Animals = new List<int> { 0, 1 },
                Name = "UserMocked"
            };
            List<User> userList = new List<User>();
            userList.Add(user);

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(ga => ga.GetAnimalById(0)).Returns(animalList[0]);
            mockGet.Setup(gu => gu.GetUsers()).Returns(userList);

            var mockSet = new Mock<ISetDataServices>();
            mockSet.Setup(m => m.DeleteAnimal(0, animalList, user, userList)).Returns(true);

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.DeleteAnimal(0);
            var deletedResult = result as OkObjectResult;

            // assert
            Assert.IsNotNull(deletedResult);
            Assert.AreEqual(200, deletedResult.StatusCode);
        }

        /* Test DeleteAnimal method given an id from an animal that does not exists */
        [TestMethod()]
        public void DeleteAnimalNotExistsTest()
        {
            List<Animal> animalList = new List<Animal>
            {
                new Sheep(0, "Dory", 0),
                new Sheep(1, "Clone", 0)
            };
            User user = new User
            {
                ID = 0,
                Animals = new List<int> { 0, 1 },
                Name = "UserMocked"
            };
            List<User> userList = new List<User>();
            userList.Add(user);

            var mockGet = new Mock<IGetDataServices>();
            mockGet.Setup(repo => repo.GetAnimals()).Returns(animalList);
            mockGet.Setup(ga => ga.GetAnimalById(8)).Returns((Animal)null);
            mockGet.Setup(gu => gu.GetUsers()).Returns(userList);

            var mockSet = new Mock<ISetDataServices>();

            // arrange
            var controller = new AnimalsController(mockGet.Object, mockSet.Object);

            // act
            var result = controller.DeleteAnimal(8);
            var deletedResult = result as BadRequestObjectResult;

            // assert
            Assert.IsNotNull(deletedResult);
            Assert.AreEqual(400, deletedResult.StatusCode);
        }
    }
}