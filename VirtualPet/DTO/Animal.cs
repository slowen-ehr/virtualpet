using System;

namespace DTO
{
    public abstract class Animal
    {
        public int ID { get; set; }
        public string Name { get; set;}
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public double MinStatus { get; set; }
        public double MaxStatus { get; set; }
        public double Hapiness { get; set; }
        public double Hungry { get; set; }
        public double HappynessPerMinute { get; set; }
        public double HungryPerMinute { get; set; }
        public double CaressValue { get; set; }
        public double MealValue { get; set; }


        public double Feed()
        {
            this.Hungry -= MealValue;
            if (Hungry < MinStatus)
            {
                this.Hungry = MinStatus;
            }
            return this.Hungry;
        }

        public double Stroke()
        {
            this.Hapiness += CaressValue;
            if (Hapiness > MaxStatus)
            {
                this.Hapiness = MaxStatus;
            }
            return this.Hapiness;
        }

        public Animal Update(DateTime newDate)
        {
            double minutes = (newDate - this.LastUpdatedDate).Minutes;
            this.Hapiness -= HappynessPerMinute * Math.Round(minutes, 2);
            if (Hapiness < MinStatus) Hapiness = MinStatus;
            this.Hungry += HungryPerMinute * Math.Round(minutes, 2);
            if (Hungry > MaxStatus) Hungry = MaxStatus;
            this.LastUpdatedDate = newDate;
            return this;
        }
    }    
}
