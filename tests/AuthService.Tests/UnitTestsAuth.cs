using AuthService.Data.Interface;
using AuthService.Model;
using AuthService.Services.Implementations;
using AuthService.Services.Interfaces;
using AutoMapper;
using CommonPackage;
using Moq;


namespace AuthService.Tests;

public class UnitTestsAuth
{
 public class UserServiceTests
    {
        private readonly Mock<IHashingLogic> _hashingLogicMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITracingService> _tracingServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _hashingLogicMock = new Mock<IHashingLogic>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _tracingServiceMock = new Mock<ITracingService>();
            _tracingServiceMock.Setup(t => t.StartActiveSpan(It.IsAny<string>())).Returns(Mock.Of<IDisposable>);
            _userService = new UserService(_hashingLogicMock.Object, _userRepositoryMock.Object, _mapperMock.Object, _tracingServiceMock.Object);
        }

        [Fact]
        public void CreateUser_ShouldReturnUser_WhenUserIsCreated()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Email = "test@example.com", password = "password123" };
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            _hashingLogicMock.Setup(h => h.GenerateHash(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny))
                             .Callback(new GenerateHashCallback((string password, out byte[] passwordHash, out byte[] passwordSalt) =>
                             {
                                 passwordHash = new byte[] { 1, 2, 3 };
                                 passwordSalt = new byte[] { 4, 5, 6 };
                             }));
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserDto>())).Returns(user);
            _userRepositoryMock.Setup(r => r.CreateUser(It.IsAny<User>())).Returns(user);

            // Act
            var result = _userService.CreateUser(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            _userRepositoryMock.Verify(r => r.CreateUser(It.Is<User>(u => u.Username == "testuser")), Times.Once);
        }

        [Fact]
        public void GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetUserById(1)).Returns(user);

            // Act
            var result = _userService.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _userRepositoryMock.Verify(r => r.GetUserById(1), Times.Once);
        }
        

        [Fact]
        public void UpdateUser_ShouldReturnUpdatedUser_WhenUserIsUpdated()
        {
            // Arrange
            var userDto = new UserDto { Username = "updateduser", Email = "updated@example.com", password = "newpassword123" };
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };

            _mapperMock.Setup(m => m.Map(It.IsAny<UserDto>(), It.IsAny<User>())).Callback<UserDto, User>((src, dest) => 
            {
                dest.Username = src.Username;
                dest.Email = src.Email;
            });
            _hashingLogicMock.Setup(h => h.GenerateHash(It.IsAny<string>(), out It.Ref<byte[]>.IsAny, out It.Ref<byte[]>.IsAny))
                             .Callback(new GenerateHashCallback((string password, out byte[] passwordHash, out byte[] passwordSalt) =>
                             {
                                 passwordHash = new byte[] { 1, 2, 3 };
                                 passwordSalt = new byte[] { 4, 5, 6 };
                             }));
            _userRepositoryMock.Setup(r => r.UpdateUser(It.IsAny<User>())).Returns(user);

            // Act
            var result = _userService.UpdateUser(user, userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Username, result.Username);
            _userRepositoryMock.Verify(r => r.UpdateUser(It.Is<User>(u => u.Username == "updateduser")), Times.Once);
        }

        [Fact]
        public void DeleteUser_ShouldReturnUser_WhenUserIsDeleted()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.DeleteUser(1)).Returns(user);

            // Act
            var result = _userService.DeleteUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _userRepositoryMock.Verify(r => r.DeleteUser(1), Times.Once);
        }

        [Fact]
        public void GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetUserByEmail("test@example.com")).Returns(user);

            // Act
            var result = _userService.GetUserByEmail("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            _userRepositoryMock.Verify(r => r.GetUserByEmail("test@example.com"), Times.Once);
        }

        // Delegate for callback in Setup
        private delegate void GenerateHashCallback(string password, out byte[] passwordHash, out byte[] passwordSalt);
    }
}