using OpenChat.Domain.Users;

namespace OpenChat.Tests.Common
{
    public static class UsersMother
    {
        public const string GUID_PATTERN = "[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}";

        public const string ALICE_USERNAME = "alice123";
        public const string ALICE_PASSWORD = "alki324D132!";
        public const string ALICE_USERNAME_UPPERCASE = "ALICE123";
        public const string ALICE_ABOUT = "I am Alice.";
        public const string ALICE_PASSWORD_DIFFERENT_CASING = "AlkI324d132!";

        public const string BOB_USERNAME = "bob";
        public const string BOB_PASSWORD = "bobIsTh3b3st!";
        public const string BOB_ABOUT = "I am Bob.";

        public const string JOHN_USERNAME = "john";
        public const string JOHN_PASSWORD = "john123!";
        public const string JOHN_ABOUT = "I am John.";

        public const string WRONG_USERNAME = "WrongUsername";
        public const string WRONG_PASSWORD = "WrongPassword";

        public static Guid USER_ID = Guid.NewGuid();
        public static Guid NON_EXISTING_USER_ID = Guid.NewGuid();
        public static Guid USER_ID_EMPTY = Guid.Empty;

        public static UserDto ALICE_DTO = new UserDto(
            username: ALICE_USERNAME,
            password: ALICE_PASSWORD
        );

        public static UserDto BOB_DTO = new UserDto(
            username: BOB_USERNAME,
            password: BOB_PASSWORD
        );

        public static UserDto JOHN_DTO = new UserDto(
            username: JOHN_USERNAME,
            password: JOHN_PASSWORD
        );


        public static UserDto GetAliceWith(string username)
        {
            var user = new UserDto(
                username: username,
                password: ALICE_PASSWORD,
                about: ALICE_ABOUT
            );

            return user;
        }
    }
}