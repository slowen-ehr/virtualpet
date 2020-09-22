using System.Collections.Generic;

namespace DTO
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<int> Animals { get; set; }
    }
}
