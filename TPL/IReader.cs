using System.Threading.Tasks;

namespace TPL
{
    public interface IReader
    {
        Task<char> awaitNext();
        char Next();
    }
}
