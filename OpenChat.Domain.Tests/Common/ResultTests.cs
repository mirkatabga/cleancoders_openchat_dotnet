namespace OpenChat.Domain.Tests.Common
{
    using FluentAssertions;
    using OpenChat.Domain.Common;
    using OpenChat.Domain.Exceptions;

    public partial class ResultsTests
    {
        [Fact]
        public void Instantiating_ResultWithValue_ShouldSetSuccessState()
        {
            var foo = new Foo();
            var result = CreateFooResult.Success(foo);

            result.IsSuccess
                .Should()
                .BeTrue();

            result.IsFailed
                .Should()
                .BeFalse();

            result.Value
                .Should()
                .Be(foo);
        }

        [Fact]
        public void Instantiating_ResultWithNullValue_ShouldThrow()
        {
            var createInstanceHandler = () =>
                CreateFooResult.Success(null!);

            createInstanceHandler
                .Invoking(handler => handler())
                .Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Instantiating_ResultWithError_ShouldSetFailedState()
        {
            var errorReason = CreateFooError.FooAlreadyExists;
            var result = CreateFooResult.Error(errorReason);

            result.IsFailed
                .Should()
                .BeTrue();

            result.IsSuccess
                .Should()
                .BeFalse();

            result.Reason
                .Should()
                .Be(errorReason);
        }

        [Fact]
        public void Instantiating_ResultWithNotDefinedError_ShouldThrow()
        {
            var createInstanceHandler = () =>
                CreateFooResult.Error((CreateFooError)0);

            createInstanceHandler
                .Invoking(handler => handler())
                .Should()
                .ThrowExactly<UnexpectedEnumValueException<CreateFooError>>();
        }

        private class Foo { }

        private enum CreateFooError
        {
            FooAlreadyExists = 1
        }

        private class CreateFooResult : Result<Foo, CreateFooError>
        {
            private CreateFooResult(Foo value) : base(value)
            {
            }

            private CreateFooResult(CreateFooError errorReason) : base(errorReason)
            {
            }

            public static CreateFooResult Success(Foo foo)
            {
                return new CreateFooResult(foo);
            }

            public static CreateFooResult Error(CreateFooError reason)
            {
                return new CreateFooResult(reason);
            }
        }
    }
}