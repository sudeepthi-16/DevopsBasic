using DevopsBasic.Models;

namespace DevopsBasic.Data;

public static class InMemoryStudentStore
{
    private static readonly List<Student> _students = new();
    private static int _nextId = 1;
    private static readonly object _lock = new();

    static InMemoryStudentStore()
    {
        // initial seed
        Add(new Student { Name = "Asha", Dept = "CSE", Marks = 85 });
        Add(new Student { Name = "Vikram", Dept = "ECE", Marks = 78 });
    }

    public static List<Student> GetAll()
    {
        lock (_lock)
        {
            // return a shallow copy to avoid external mutation
            return _students.Select(s => new Student
            {
                Id = s.Id,
                Name = s.Name,
                Dept = s.Dept,
                Marks = s.Marks
            }).ToList();
        }
    }

    public static Student? GetById(int id)
    {
        lock (_lock)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
    }

    public static Student Add(Student student)
    {
        lock (_lock)
        {
            student.Id = _nextId++;
            // store a copy to protect the internal list
            var copy = new Student
            {
                Id = student.Id,
                Name = student.Name,
                Dept = student.Dept,
                Marks = student.Marks
            };
            _students.Add(copy);
            return copy;
        }
    }

    public static bool Update(int id, Student updated)
    {
        lock (_lock)
        {
            var idx = _students.FindIndex(s => s.Id == id);
            if (idx == -1) return false;
            // maintain Id from route
            _students[idx] = new Student
            {
                Id = id,
                Name = updated.Name,
                Dept = updated.Dept,
                Marks = updated.Marks
            };
            return true;
        }
    }

    public static bool Delete(int id)
    {
        lock (_lock)
        {
            var existing = _students.FirstOrDefault(s => s.Id == id);
            if (existing == null) return false;
            _students.Remove(existing);
            return true;
        }
    }
}