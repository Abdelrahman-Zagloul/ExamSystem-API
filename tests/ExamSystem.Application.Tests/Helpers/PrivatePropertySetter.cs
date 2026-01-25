using System.Reflection;

namespace ExamSystem.Application.Tests.Helpers
{
    public static class PrivatePropertySetter
    {
        public static void Set<TTarget, TValue>(TTarget target, string propertyName, TValue value) where TTarget : class
        {
            var property = typeof(TTarget)
                .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (property == null)
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(TTarget).Name}'.");

            property.SetValue(target, value);
        }
    }
}