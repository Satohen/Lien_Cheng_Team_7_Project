namespace 第7小組專題.Models.Checkin
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // admin / user
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
