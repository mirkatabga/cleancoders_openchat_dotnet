using FluentAssertions;
using OpenChat.Infrastructure.Persistence;

namespace OpenChat.Domain.Tests.Common
{
    public abstract class DomainTests
    {
        protected void AssertUnitOfWork(InMemoryUnitOfWork unitOfWork)
        {
            unitOfWork.SaveChangesInvocations
                .Should()
                .Be(1);
        }
    }
}