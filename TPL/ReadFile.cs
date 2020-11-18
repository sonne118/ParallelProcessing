using System.IO;
using System.Threading.Tasks;

namespace TPL
{
    public class ReadFile
    {
        public async Task<string> GetData()
        {
            string readText = default;

            if (File.Exists(GetPath()))
            {
                readText = await File.ReadAllTextAsync(GetPath());
            }
            return readText;
        }

        private string GetPath()
        {
            return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "file.txt");
        }
    }
}
