using OpenChat.Domain.Exceptions;

namespace OpenChat.Domain.Common
{
    public abstract class Result<TValue, TEnum> : Result<TEnum> where TEnum : Enum
    {
        protected Result(TValue value)
            : base()
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected Result(TEnum errorReason)
            : base(errorReason)
        {
        }

        public TValue? Value { get; }
    }

    public abstract class Result<TEnum> where TEnum : Enum
    {
        protected Result()
        {
            IsSuccess = true;
        }

        protected Result(TEnum errorReason)
        {
            var notDefined = !Enum.IsDefined(typeof(TEnum), errorReason);

            if (notDefined)
            {
                throw new UnexpectedEnumValueException<TEnum>(errorReason);
            }

            Reason = errorReason;
            IsFailed = true;
        }

        public bool IsSuccess { get; }

        public bool IsFailed { get; }

        public TEnum? Reason { get; }
    }
}