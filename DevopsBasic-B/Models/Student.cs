namespace DevopsBasic.Models
{
    public class Student
    {
        public int Id { get; set; }             // PK (assigned in-memory)
        public string Name { get; set; } = "";
        public string Dept { get; set; } = "";
        public int Marks { get; set; } = 0;
    }
}
