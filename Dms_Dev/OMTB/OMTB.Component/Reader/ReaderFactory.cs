namespace OMTB.Component.Reader
{
    public class ReaderFactory
    {
        public static IReader CreateInstance(ReaderType? readerType)
        {
            if (readerType == ReaderType.Sql)
                return new SqlReader();
            
            return null;
        }
    }
}