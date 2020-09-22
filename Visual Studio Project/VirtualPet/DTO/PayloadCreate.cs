namespace DTO
{
    /*
     Payload of the create. To get data from post body
    */
    public class PayloadCreate
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int UserId { get; set; }
    }
}
