using System.IO;

namespace Soccer.Common.Helpers
{
    public class FilesHelper : IFilesHelper
    {
        //recibe un stream y devuelve un arrays de bytes
        public byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
