using System;
using System.Security.Claims;
using System.Threading.Tasks;
using eventRadar.Auth;
using eventRadar.Auth.Model;
using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static eventRadar.Auth.Model.AuthDtos;

namespace eventRadar.Tests
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;
        private AuthController _authController;

        [TestInitialize]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();

            _authController = new AuthController(_userManagerMock.Object, _jwtTokenServiceMock.Object);
        }

        [TestMethod]
        public async Task Register_SuccessfulRegistration_ReturnsCreatedAtActionResult()
        {
            var registerUserDto = new RegisterUserDto("SimonaBlokas", "testemail@gmail.com", "Bakalaurasyragerai1!", "Testinis", "Naudotojas");
            _userManagerMock.Setup(x => x.FindByNameAsync(registerUserDto.Username)).ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), registerUserDto.Password)).ReturnsAsync(IdentityResult.Success);

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerUserDto.Username,
                Email = registerUserDto.Email,
                Name = registerUserDto.Name,
                Surname = registerUserDto.Surname
            };

            var result = await _authController.Register(registerUserDto);

            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdAtResult = result as CreatedAtActionResult;
            Assert.IsInstanceOfType(createdAtResult.Value, typeof(NewUserDto));
        }
        [TestMethod]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            var registerUserDto = new RegisterUserDto("NewUser", "newuser@example.com", "SecurePassword123!", "John", "Doe");
            _userManagerMock.Setup(x => x.FindByNameAsync(registerUserDto.Username)).ReturnsAsync(new User());

            var result = await _authController.Register(registerUserDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_CreateUserFails_ReturnsBadRequest()
        {
            var registerUserDto = new RegisterUserDto("NewUser", "newuser@example.com", "SecurePassword123!", "John", "Doe");
            _userManagerMock.Setup(x => x.FindByNameAsync(registerUserDto.Username)).ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), registerUserDto.Password)).ReturnsAsync(IdentityResult.Failed());

            var result = await _authController.Register(registerUserDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_InvalidUsername_ReturnsBadRequest()
        {
            var loginDto = new LoginDto("Simona123123", "Bakalaurasyragerai1!");
            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync((User)null);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            var loginDto = new LoginDto ("Simona", "Bakalauras");
            var user = new User { UserName = loginDto.Username, LockoutEnd = null };
            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(false);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_UserLockedOut_ReturnsBadRequest()
        {
            var loginDto = new LoginDto ("SimonaBlokas", "Bakalaurasyragerai1!");
            var user = new User { UserName = loginDto.Username, LockoutEnd = DateTimeOffset.MaxValue };
            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync(user);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_SuccessfulLogin_ReturnsOkObjectResult()
        {
            var loginDto = new LoginDto ("Simona", "Bakalaurasyragerai1!");
            var user = new User { Id = Guid.NewGuid().ToString(), UserName = loginDto.Username, LockoutEnd = null };
            var roles = new List<string> { "SystemUser" };
            var accessToken = "SampleAccessToken";

            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _jwtTokenServiceMock.Setup(x => x.CreateAccessToken(user.UserName, user.Id, roles)).Returns(accessToken);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOfType(okResult.Value, typeof(SuccessfullLoginDto));
            var successfulLoginDto = okResult.Value as SuccessfullLoginDto;
            Assert.AreEqual(accessToken, successfulLoginDto.AccessToken);
        }
        [TestMethod]
        public async Task BlockUser_UserNotFound_ReturnsNotFound()
        {
            string userId = Guid.NewGuid().ToString();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _authController.BlockUser(userId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task BlockUser_SuccessfulBlocking_ReturnsOkObjectResult()
        {
            string userId = Guid.NewGuid().ToString();
            var user = new User
            {
                Id = userId,
                UserName = "ExistingUser",
                Email = "user@example.com",
                PasswordHash = "hashedPassword",
                Name = "John",
                Surname = "Doe",
                LockoutEnd = null,
                LockoutEnabled = true
            };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

            var result = await _authController.BlockUser(userId);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOfType(okResult.Value, typeof(UserDto));
            var userDto = okResult.Value as UserDto;
            Assert.AreEqual(userId, userDto.Id);
            Assert.AreEqual(DateTimeOffset.MaxValue, userDto.LockoutEnd);
        }
        [TestMethod]
        public async Task BlockUser_WithValidUserId_ReturnsOkResult()
        {
            string userId = "valid-user-id";
            var user = new User { Id = userId, UserName = "testuser", Email = "testuser@example.com" };
            _userManagerMock.Setup(mgr => mgr.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(mgr => mgr.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _authController.BlockUser(userId);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(UserDto));
            var userDto = okResult.Value as UserDto;
            Assert.AreEqual(userId, userDto.Id);
            Assert.AreEqual(user.UserName, userDto.Username);
            Assert.AreEqual(user.Email, userDto.Email);
            Assert.AreEqual(user.PasswordHash, userDto.Password);
            Assert.AreEqual(user.Name, userDto.Name);
            Assert.AreEqual(user.Surname, userDto.Surname);
            Assert.AreEqual(user.LockoutEnd, userDto.LockoutEnd);
            Assert.AreEqual(user.LockoutEnabled, userDto.LockoutEnabled);

            _userManagerMock.Verify(mgr => mgr.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(mgr => mgr.UpdateAsync(user), Times.Once);
        }

        [TestMethod]
        public async Task BlockUser_WithInvalidUserId_ReturnsNotFoundResult()
        {
            string userId = "invalid-user-id";
            _userManagerMock.Setup(mgr => mgr.FindByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _authController.BlockUser(userId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _userManagerMock.Verify(mgr => mgr.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(mgr => mgr.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
        [TestMethod]
        public async Task Login_UserIsNotBlocked_ReturnsOkResult()
        {
            var loginDto = new LoginDto("testuser", "password");
            var user = new User { UserName = "testuser", LockoutEnd = null };
            _userManagerMock.Setup(mgr => mgr.FindByNameAsync(loginDto.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(mgr => mgr.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(mgr => mgr.GetRolesAsync(user)).ReturnsAsync(new List<string>());

            var accessToken = "sample-access-token";
            _jwtTokenServiceMock.Setup(service => service.CreateAccessToken(user.UserName, user.Id, It.IsAny<List<string>>())).Returns(accessToken);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(SuccessfullLoginDto));
            var successfulLoginDto = okResult.Value as SuccessfullLoginDto;
            Assert.IsNotNull(successfulLoginDto);
            Assert.AreEqual(accessToken, successfulLoginDto.AccessToken);

            _userManagerMock.Verify(mgr => mgr.FindByNameAsync(loginDto.Username), Times.Once);
            _userManagerMock.Verify(mgr => mgr.CheckPasswordAsync(user, loginDto.Password), Times.Once);
            _userManagerMock.Verify(mgr => mgr.GetRolesAsync(user), Times.Once);
            _jwtTokenServiceMock.Verify(service => service.CreateAccessToken(user.UserName, user.Id, It.IsAny<List<string>>()), Times.Once);
        }

        [TestMethod]
        public async Task Login_UserIsBlocked_ReturnsBadRequestResult()
        {
            var loginDto = new LoginDto("testuser", "password");
            var user = new User { UserName = "testuser", LockoutEnd = DateTimeOffset.MaxValue };
            _userManagerMock.Setup(mgr => mgr.FindByNameAsync(loginDto.Username)).ReturnsAsync(user);

            var result = await _authController.Login(loginDto);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Naudotojas yra uþblokuotas", badRequestResult.Value);

            _userManagerMock.Verify(mgr => mgr.FindByNameAsync(loginDto.Username), Times.Once);
            _userManagerMock.Verify(mgr => mgr.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(mgr => mgr.GetRolesAsync(It.IsAny<User>()), Times.Never);
        }
        [TestMethod]
        public async Task UnblockUser_ValidUserId_ReturnsOkResult()
        {
            var userId = "1";
            var pastDate = DateTimeOffset.Now.AddDays(-1);
            var user = new User { Id = userId, UserName = "testuser", LockoutEnd = pastDate };
            _userManagerMock.Setup(mgr => mgr.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(mgr => mgr.SetLockoutEndDateAsync(user, null)).ReturnsAsync(IdentityResult.Success);

            var result = await _authController.UnblockUser(userId);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(UserDto));
            var userDto = okResult.Value as UserDto;
            Assert.IsNotNull(userDto);
            Assert.AreEqual(userId, userDto.Id);
            Assert.AreEqual(user.UserName, userDto.Username);
            Assert.AreEqual(user.Email, userDto.Email);
            Assert.AreEqual(user.PasswordHash, userDto.Password);
            Assert.AreEqual(user.Name, userDto.Name);
            Assert.AreEqual(user.Surname, userDto.Surname);
            Assert.IsTrue(userDto.LockoutEnd == null || userDto.LockoutEnd <= DateTimeOffset.Now);
            Assert.AreEqual(user.LockoutEnabled, userDto.LockoutEnabled);

            _userManagerMock.Verify(mgr => mgr.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(mgr => mgr.SetLockoutEndDateAsync(user, null), Times.Once);
        }

        [TestMethod]
        public async Task UnblockUser_InvalidUserId_ReturnsNotFoundResult()
        {
            var userId = "1";
            _userManagerMock.Setup(mgr => mgr.FindByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _authController.UnblockUser(userId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));

            _userManagerMock.Verify(mgr => mgr.FindByIdAsync(userId), Times.Once);
            _userManagerMock.Verify(mgr => mgr.SetLockoutEndDateAsync(It.IsAny<User>(), It.IsAny<DateTimeOffset?>()), Times.Never);
        }
    }
}