using System.IO;

namespace Soccer.Common.Helpers
{
    public interface IFilesHelper
    {
        //lee un array de bytes
        byte[] ReadFully(Stream input);
    }
}
