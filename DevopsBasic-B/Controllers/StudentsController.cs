using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevopsBasic.Data;
using DevopsBasic.Models;

namespace DevopsBasic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET: api/Students
        [HttpGet]
        public Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var list = InMemoryStudentStore.GetAll();
            return Task.FromResult<ActionResult<IEnumerable<Student>>>(Ok(list));
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = InMemoryStudentStore.GetById(id);
            if (student == null) return Task.FromResult<ActionResult<Student>>(NotFound());
            return Task.FromResult<ActionResult<Student>>(Ok(student));
        }

        // PUT: api/Students/5
        [HttpPut("{id:int}")]
        public Task<IActionResult> PutStudent(int id, Student student)
        {
            if (student == null) return Task.FromResult<IActionResult>(BadRequest());
            var updated = InMemoryStudentStore.Update(id, student);
            if (!updated) return Task.FromResult<IActionResult>(NotFound());
            return Task.FromResult<IActionResult>(NoContent());
        }

        // POST: api/Students
        [HttpPost]
        public Task<ActionResult<Student>> PostStudent(Student student)
        {
            if (student == null) return Task.FromResult<ActionResult<Student>>(BadRequest());
            var created = InMemoryStudentStore.Add(student);
            return Task.FromResult<ActionResult<Student>>(CreatedAtAction(nameof(GetStudent), new { id = created.Id }, created));
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = InMemoryStudentStore.Delete(id);
            if (!deleted) return Task.FromResult<IActionResult>(NotFound());
            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}
