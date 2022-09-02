using OpenChat.Api.Tests.Abstractions;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Api.Tests
{
    public class AccountsControllerTests : ControllerTests
    {
        [Fact]
        public void Register_ValidUser_ReturnsOkWithCorrectUserData()
        {
            var accountController = new AccountsController(_usersService);
            var registerRequest = GetAliceRegisterRequest();
            var expected = GetUserResponseFromRequest(registerRequest);

            var actionResult = accountController.Register(registerRequest);

            var userResponse = AssertObjectResult<OkObjectResult, UserResponse>(actionResult);

            userResponse
                .Should()
                .BeEquivalentTo(expected, opt => opt.Excluding(resp => resp.Id));

            userResponse.Id
                .Should()
                .NotBeEmpty();

            userResponse.Id.ToString()
                .Should()
                .MatchRegex(GUID_PATTERN);
        }

        [Fact]
        public void Register_DuplicateUsername_ReturnsBadRequestWithErrorMessage()
        {
            var accountController = new AccountsController(_usersService);
            var registerRequest = GetAliceRegisterRequest();
            accountController.Register(registerRequest);

            var actionResult = accountController.Register(registerRequest);

            var errorMessage = AssertObjectResult<BadRequestObjectResult, string>(actionResult);

            errorMessage
                .Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(ALICE_USERNAME, ALICE_PASSWORD)]
        [InlineData(ALICE_USERNAME_UPPERCASE, ALICE_PASSWORD)]
        public void Login_CorrectCredentials_ReturnsOkWithCorrectUserData(string username, string password)
        {
            var accountController = new AccountsController(_usersService);
            var registerRequest = GetAliceRegisterRequest();
            var expected = GetUserResponseFromRequest(registerRequest);
            var loginRequest = GetLoginRequest(username, password);
            accountController.Register(registerRequest);

            var actionResult = accountController.Login(loginRequest);

            var userResponse = AssertObjectResult<OkObjectResult, UserResponse>(actionResult);

            userResponse
                .Should()
                .BeEquivalentTo(expected, opt => opt.Excluding(resp => resp.Id));
        }

        [Fact]
        public void Login_WrongCredentials_ReturnsBadRequestWithErrorMessage()
        {
            var accountController = new AccountsController(_usersService);
            var registerRequest = GetAliceRegisterRequest();
            var loginRequest = GetLoginRequest(ALICE_USERNAME, "WrongPassword");
            accountController.Register(registerRequest);

            var actionResult = accountController.Login(loginRequest);

            var errorMessage = AssertObjectResult<BadRequestObjectResult, string>(actionResult);

            errorMessage
                .Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetAll_Users_ReturnsAllUsers()
        {
            var accountsController = new AccountsController(_usersService);
            var alice = RegisterUser(GetAliceRegisterRequest());
            var bob = RegisterUser(GetBobRegisterRequest());
            var actionResult = accountsController.GetAll();

            var users = AssertObjectResult<OkObjectResult, ICollection<UserResponse>>(
                actionResult, assertObjectType: false);

            users
                .Should()
                .ContainEquivalentOf(alice)
                .And
                .ContainEquivalentOf(bob);
        }

        private static LoginRequest GetLoginRequest(string username, string password)
        {
            return new LoginRequest
            {
                Username = username,
                Password = password
            };
        }

        private static UserResponse GetUserResponseFromRequest(RegisterRequest request)
        {
            return new UserResponse(
                id: USER_ID,
                username: request.Username!, 
                about: request.About!);
        }      
    }
}

