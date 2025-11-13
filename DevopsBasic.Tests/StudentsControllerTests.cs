
using Xunit;
using DevopsBasic.Controllers;
using DevopsBasic.Models;
using DevopsBasic.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DevopsBasic.Tests
{
    public class StudentsControllerTests
    {
        public StudentsControllerTests()
        {
            InMemoryStudentStore.Clear(); // reset before each test
        }

        [Fact]
        public async Task GetStudents_ReturnsOk_WithList()
        {
            InMemoryStudentStore.Add(new Student { Name = "Alice", Dept = "CS" });

            var controller = new StudentsController();
            var result = await controller.GetStudents();

            var ok = result.Result as OkObjectResult;
            Assert.NotNull(ok);

            var data = ok!.Value as IEnumerable<Student>;
            Assert.Single(data!);
        }

        [Fact]
        public async Task GetStudent_ReturnsOk_WhenFound()
        {
            var created = InMemoryStudentStore.Add(new Student { Name = "Bob", Dept = "IT" });

            var controller = new StudentsController();
            var result = await controller.GetStudent(created.Id);

            var ok = result.Result as OkObjectResult;
            Assert.NotNull(ok);

            var s = ok!.Value as Student;
            Assert.Equal("Bob", s!.Name);
            Assert.Equal("IT", s!.Dept);
        }

        [Fact]
        public async Task GetStudent_ReturnsNotFound_WhenMissing()
        {
            var controller = new StudentsController();
            var result = await controller.GetStudent(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostStudent_ReturnsCreated()
        {
            var controller = new StudentsController();

            var student = new Student { Name = "Carol", Dept = "ECE" };

            var result = await controller.PostStudent(student);
            var created = result.Result as CreatedAtActionResult;

            Assert.NotNull(created);

            var s = created!.Value as Student;
            Assert.Equal("Carol", s!.Name);
            Assert.Equal("ECE", s!.Dept);
        }

        [Fact]
        public async Task PutStudent_Updates_WhenExists()
        {
            var created = InMemoryStudentStore.Add(new Student { Name = "Dan", Dept = "Mech" });

            var controller = new StudentsController();

            var updated = new Student { Name = "Daniel", Dept = "ME Updated" };

            var result = await controller.PutStudent(created.Id, updated);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutStudent_ReturnsNotFound_WhenMissing()
        {
            var controller = new StudentsController();
            var s = new Student { Name = "Test", Dept = "None" };

            var result = await controller.PutStudent(999, s);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteStudent_Deletes_WhenExists()
        {
            var saved = InMemoryStudentStore.Add(new Student { Name = "Eve", Dept = "EEE" });

            var controller = new StudentsController();
            var result = await controller.DeleteStudent(saved.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteStudent_ReturnsNotFound_WhenMissing()
        {
            var controller = new StudentsController();
            var result = await controller.DeleteStudent(488);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
