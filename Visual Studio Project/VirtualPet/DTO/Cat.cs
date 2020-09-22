using System;

namespace DTO
{
    public class Cat : DTO.Animal
    {
        public Cat(int id, string n, int userId)
        {
            var dateTime = DateTime.UtcNow;
            this.ID = id;
            this.Name = n;
            this.UserId = userId;
            this.CreatedDate = dateTime;
            this.LastUpdatedDate = dateTime;
            this.MaxStatus = 100;
            this.MinStatus = 0;
            this.Hungry = 50;   // Neutral
            this.Hapiness = 50; // Neutral
            this.HappynessPerMinute = 0.2;
            this.HungryPerMinute = 0.2;
            this.CaressValue = 10;
            this.MealValue = 10;
        }        
    }
}
