using LeonMapper.Exception;

namespace LeonMapper.Common
{
    public static class Assert
    {
        public static void IsNotNull<T>(T target)
        {
            if (target == null)
            {
                throw new AssertFailedException($"{nameof(target)} is not null!");
            }
        }

        public static void AreEqual<T>(T source, T target)
        {
            if (!Equals(source,target))
            {
                throw new AssertFailedException("objects are not equal!");
            }
        }
    }
}
