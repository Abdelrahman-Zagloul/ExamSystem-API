using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExamSystem.Application.Tests.Helpers
{
    internal static class MockHelper
    {

        public static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : IdentityUser
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
