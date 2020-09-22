# VirtualPet project

VirtualPet project solution

HTTP API which could be used to power a simple virtual pet game. In this game:
* Users have animals
* Stroking animals makes them happy
* Feeding animals makes them less hungry
* Animals start “neutral” on both metrics
* Happiness decreases over time / hunger increases over time (even when the user is offline)
* Users can own multiple animals of different types
* Different animal types have metrics which increase/decrease at different rates

## Prerequisites and instructions

The entire code was made by Visual Studio 2019 and .Net core.

It is advisable to run and debug the application by Visual Studio 2019 but a published version is included.

##### To run the published version:
* Make sure .Net Core is installed, you can download it in [this URL](https://dotnet.microsoft.com/download)
* From "Published Project" path, execute dotnet VirtualPet.dll by console or just run VirtualPet.exe 
* Solution will be listening at https://localhost:5001
* Try the first endpoint like https://localhost:5001/Animals
* I recommend to use postman for the endpoint calls

##### To run the Visual Studio project version:
* Open VirtualPet.sln with Visual Studio 2019
* Make sure variable dbFlagg is true into GetDataServices and SetDataServices class. This is to use the temporal storage solution using JSON files
* Use execute button from Visual Studio
* Solution will be listening at https://localhost:44367
* Try the first endpoint like https://localhost:44367/Animals
* I recommend to use postman for the endpoint calls

If Visual Studio version was published it is necessary to change dbFlagg to false into GetDataServices and SetDataServices class.

## API specification


### GET /Animals

Show all the animals objects from the json file. This is a debug endpoint, to get the status of every animal created.

#### Responses

* 200 OK: No errors and the animal list. A message if there is no animals yet in json file.
```
[
    {
        "id": 0,
        "name": "Reaper",
        "userId": 2,
        "createdDate": "2020-08-20T22:18:52.5033187Z",
        "lastUpdatedDate": "2020-08-22T22:03:12.446031Z",
        "minStatus": 0,
        "maxStatus": 100,
        "hapiness": 49,
        "hungry": 51,
        "happynessPerMinute": 0.5,
        "hungryPerMinute": 0.5,
        "caressValue": 5,
        "mealValue": 20
    },
    {
        "id": 1,
        "name": "Princess",
        "userId": 0,
        "createdDate": "2020-08-20T22:19:13.0209653Z",
        "lastUpdatedDate": "2020-08-22T22:03:12.4460375Z",
        "minStatus": 0,
        "maxStatus": 100,
        "hapiness": 46,
        "hungry": 60,
        "happynessPerMinute": 2,
        "hungryPerMinute": 5,
        "caressValue": 5,
        "mealValue": 15
    },       
        ( ... )
    ]
```

### GET /Animals/User/<integer>

Show all the animals tha the user with ID = <integer> owns.

#### Responses:

* 200 OK: No errors, returns the users animals data. A message if there are no animals en json file yet.

```
[
    {
        "id": 0,
        "name": "Reaper",
        "userId": 2,
        "createdDate": "2020-08-20T22:18:52.5033187Z",
        "lastUpdatedDate": "2020-08-22T22:03:12.446031Z",
        "minStatus": 0,
        "maxStatus": 100,
        "hapiness": 49,
        "hungry": 51,
        "happynessPerMinute": 0.5,
        "hungryPerMinute": 0.5,
        "caressValue": 5,
        "mealValue": 20
    },
        ( ... )
    ]
```

* 404 NOT FOUND: The user has no animals yet.
* 400 BAD REQUEST: Returns a message if that user do not exists.

### POST /Animals

Creates a new animal.

* Body: Required. You must send the credentials for a user registered in database. Example:

```
    {
        "name": "AnimalName",
        "type": 0,
        "userId": 0
    }
```

#### Responses:

* 200 OK: No errors, the animal was created. Shows the animal data.
* 400 BAD REQUEST: A message if that user do not exists.
* 400 BAD REQUEST: A message if that animal type do not exists.

### POST /Animals/Feed

Feeds a created animal.
It is assumed the user will only order actions to their animals from the client side.
* Body: Required. You must send Id for the animal witch is going to do the action.

#### Responses:

* 200 OK: No errors, the animal was feeded. Shows a message if the animal does not need more food.

* 400 BAD REQUEST: A message if that animal do not exists.

### POST /Animals/Stroke

Caress a created animal.
It is assumed the user will only order actions to their animals from the client side.
* Body: Required. You must send Id for the animal witch is going to do the action.

#### Responses:

* 200 OK: No errors, the animal has been petted. Shows a message if the animal does not need more caress.

* 400 BAD REQUEST: A message if that animal do not exists.

### DELETE /Animals/<integer>

Deletes the animal with the id given by <integer> in the uri.


#### Responses

* 200 OK: The animal has been deleted.

* 400 BAD REQUEST: A message if that animal do not exists.

## Development

### Add a new animal type
* Create the following file: <YourAnimalType>.cs in Model module and make it inherit from the Animal class.
* Adds a new case into CreateAnimal function switch(type).

### Add a new action for animals
* Implement the function in Models module, class Animal.cs to modify the Animals values.
* Make a method into SetDataServices class, in Application module,  to be called by ActionAnimal() method. Declare it in ISetDataServices interface.
* Add a new endpoint into AnimalControler, in VirtualPet web module.


## Future work

### Create a data storage solution.

* Current json files are a temporary solution for data storage. The new storage method should manage data access concurrency problems.

### Complete logic for user resource.

* Current version has a json file with 4 users created. It should be necessary to complete methods and endpoints for the user creation, destruction and updates. 

### Improve resources access methods

* Because data storage is a temporary solution, the search for and access to such data can be improved. Currently the data is transformed into lists. Modifying the management to another Abstract Data type using Key and value could be more efficient.

### Complete unit testing and add test methods to Application and Model project.

* Unit testing is currently covering AnimalController's methods and its more important uses cases. It could be improved creating more test methods to cover all the possibilities that the method could manage.

### Adapt Animal parameters to the application use.
* Current Animal values have been scaled to see how the Happiness and Hungry values decrease in a few minutes. They would be scaled to the apropiated use.

## Personal notes

* This project was made by me, Eduardo Hernanz Rodriguez for Mediatonic's recruiting process. It was my first project using .Net Core and C# language, I develop it using my knowledge in software development for other languages and frameworks.
It was developed using Visual Studio 2019 Integrated development environment.
