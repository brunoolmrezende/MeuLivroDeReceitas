using Sqids;

namespace CommonTestUtilities.IdEncrypter
{
    public class IdEncrypterBuilder
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = "mOz2WoUvQVSgCPjYNdl5KcuAs86yJ0neIaZwhDxXBHbr437q1fkTMFREGpLti9"
            });
        }
    }
}
