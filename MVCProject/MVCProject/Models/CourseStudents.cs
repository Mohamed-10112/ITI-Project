namespace MVCProject.Models
{
    public class CourseStudents
    {
        public int Id { get; set; }
        public int Degree { get; set; }

        public int CrsId { get; set; }
        public virtual Course Course { get; set; }

        public int StdId { get; set; }
        public virtual Student Student { get; set; }

        public bool IsCompleted { get; set; } = false;

    }
}
