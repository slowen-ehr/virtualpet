using System;

namespace DTO
{
    public class Sheep : DTO.Animal
    {
        public Sheep(int id, string n, int userId)
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
            this.HappynessPerMinute = 0.5;
            this.HungryPerMinute = 0.5;
            this.CaressValue = 5;
            this.MealValue = 20;
        }        
    }
}
