using LeonMapper.Exception;

namespace LeonMapper.Common
{
    public class Assert
    {
        public static void IsNotNull<T>(T target)
        {
            if (target == null)
            {
                throw new AssertFailedException($"{nameof(target)} is not null!");
            }
        }
    }
}
