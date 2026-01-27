using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ExamSystem.Application.Tests.Helpers
{
    public static class MockHelper
    {

        public static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : IdentityUser
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        public static IMapper CreateMappingProfile<T>() where T : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<T>();
            }, NullLoggerFactory.Instance);
            return config.CreateMapper();
        }
    }
}
