using Xunit;
using DevopsBasic.Controllers;
using DevopsBasic.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace DevopsBasic.Tests
{
    public class StudentsControllerTests
    {
        public StudentsControllerTests()
        {
            // Reset store before each test
            InMemoryStudentStore.Clear();
        }

        [Fact]
        public void GetStudents_ShouldReturnAllStudents()
        {
            // Arrange
            InMemoryStudentStore.Add(new Student { Id = 1, Name = "John", Age = 22 });

            var controller = new StudentsController();

            // Act
            var result = controller.GetStudents().Result as OkObjectResult;

            // Assert
            var list = result.Value as IEnumerable<Student>;
            list.Should().HaveCount(1);
        }

        [Fact]
        public void GetStudent_ShouldReturnStudent_IfExists()
        {
            InMemoryStudentStore.Add(new Student { Id = 1, Name = "John", Age = 22 });
            var controller = new StudentsController();

            var result = controller.GetStudent(1).Result as OkObjectResult;

            var student = result.Value as Student;
            student.Id.Should().Be(1);
        }

        [Fact]
        public void GetStudent_ShouldReturnNotFound_IfNotExists()
        {
            var controller = new StudentsController();

            var result = controller.GetStudent(100).Result;

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PostStudent_ShouldCreateStudent()
        {
            var controller = new StudentsController();

            var newStudent = new Student { Name = "Sara", Age = 20 };

            var result = controller.PostStudent(newStudent).Result as CreatedAtActionResult;

            var created = result.Value as Student;

            created.Id.Should().BeGreaterThan(0);
            created.Name.Should().Be("Sara");
        }

        [Fact]
        public void PostStudent_ShouldReturnBadRequest_IfNull()
        {
            var controller = new StudentsController();

            var result = controller.PostStudent(null).Result;

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void PutStudent_ShouldUpdateStudent_IfExists()
        {
            InMemoryStudentStore.Add(new Student { Id = 1, Name = "Old", Age = 20 });
            var controller = new StudentsController();

            var updated = new Student { Name = "New", Age = 25 };

            var result = controller.PutStudent(1, updated).Result;

            result.Should().BeOfType<NoContentResult>();

            var stored = InMemoryStudentStore.GetById(1);
            stored.Name.Should().Be("New");
            stored.Age.Should().Be(25);
        }

        [Fact]
        public void PutStudent_ShouldReturnNotFound_IfMissing()
        {
            var controller = new StudentsController();

            var updated = new Student { Name = "New", Age = 25 };

            var result = controller.PutStudent(100, updated).Result;

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PutStudent_ShouldReturnBadRequest_IfNull()
        {
            var controller = new StudentsController();

            var result = controller.PutStudent(1, null).Result;

            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public void DeleteStudent_ShouldDelete_IfExists()
        {
            InMemoryStudentStore.Add(new Student { Id = 1, Name = "John", Age = 22 });
            var controller = new StudentsController();

            var result = controller.DeleteStudent(1).Result;

            result.Should().BeOfType<NoContentResult>();
            InMemoryStudentStore.GetAll().Should().BeEmpty();
        }

        [Fact]
        public void DeleteStudent_ShouldReturnNotFound_IfMissing()
        {
            var controller = new StudentsController();

            var result = controller.DeleteStudent(100).Result;

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
