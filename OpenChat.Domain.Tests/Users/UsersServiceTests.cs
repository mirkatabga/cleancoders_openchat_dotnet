using FluentAssertions;
using OpenChat.Domain.Tests.Common;
using OpenChat.Domain.Users;
using OpenChat.Domain.Users.Results;
using OpenChat.Infrastructure.Persistence;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Domain.Tests.Users
{
    public class UsersServiceTests : DomainTests
    {
        private readonly InMemoryUnitOfWork _uow;
        private readonly IUsersService _usersService;

        public UsersServiceTests()
        {
            _uow = new InMemoryUnitOfWork(new InMemoryDataContext());
            _usersService = new UsersService(_uow);
        }

        [Fact]
        public void Create_ValidUser_CreatesUser()
        {
            var result = _usersService.Create(ALICE_DTO);

            result.IsSuccess
                .Should()
                .BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(ALICE_DTO);

            result.Value!.Id
                .Should()
                .NotBeEmpty();

            result.Value!.Id
                .Should()
                .NotBeNull();

            AssertUnitOfWork(_uow);
        }

        [Theory]
        [InlineData(ALICE_USERNAME, ALICE_USERNAME)]
        [InlineData(ALICE_USERNAME_UPPERCASE, ALICE_USERNAME)]
        [InlineData(ALICE_USERNAME, ALICE_USERNAME_UPPERCASE)]
        public void Create_UserWithDuplicateUsername_ReturnsFailedResult(string firstUsername, string secondUsername)
        {
            var userDto = GetAliceWith(firstUsername);
            _usersService.Create(userDto);

            var secondUserDto = GetAliceWith(secondUsername);
            var result = _usersService.Create(secondUserDto);

            result.IsFailed
                .Should()
                .BeTrue();

            result.Reason
                .Should()
                .Be(CreateUserError.UserAlreadyExists);
        }

        [Theory]
        [InlineData(ALICE_USERNAME, ALICE_PASSWORD)]
        [InlineData(ALICE_USERNAME_UPPERCASE, ALICE_PASSWORD)]
        public void Get_CorrectUsernameAndPassword_ReturnsUser(string username, string password)
        {
            _usersService.Create(ALICE_DTO);

            var result = _usersService.GetByCredentials(username, password);

            result.IsSuccess
                .Should()
                .BeTrue();

            result.Value!.Username
                .Should()
                .Be(ALICE_DTO.Username);

            result.Value!.Password
                .Should()
                .Be(ALICE_DTO.Password);
        }

        [Theory]
        [InlineData(ALICE_USERNAME, WRONG_PASSWORD)]
        [InlineData(ALICE_USERNAME, ALICE_PASSWORD_DIFFERENT_CASING)]
        [InlineData(WRONG_USERNAME, ALICE_PASSWORD)]
        [InlineData(WRONG_USERNAME, WRONG_PASSWORD)]
        public void Get_WrongUsernameOrPassword_ReturnsFailedResult(string username, string password)
        {
            _usersService.Create(ALICE_DTO);

            var result = _usersService.GetByCredentials(username, password);

            result.IsFailed
                .Should()
                .BeTrue();

            result.Reason
                .Should()
                .Be(GetUserError.NotFound);
        }

        [Fact]
        public void GetAll_ReturnsAllUsers()
        {
            var alice = _usersService.Create(ALICE_DTO).Value;
            var bob = _usersService.Create(BOB_DTO).Value;

            var users = _usersService.GetAll();

            users
                .Should()
                .ContainEquivalentOf(alice)
                .And
                .ContainEquivalentOf(bob);
        }
    }
}