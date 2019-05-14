using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace KontrlRefl2
{
    class Comment
    {
        public readonly int postId;
        public readonly int id;
        public readonly string name;
        public readonly string email;
        public readonly string body;
    }

    class ResultItem
    {
        public readonly int CommentId;
        public readonly int BodyLetterAmount;

        public ResultItem(int commentId, int bodyLetterAmount)
        {
            CommentId = commentId;
            BodyLetterAmount = bodyLetterAmount;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<ResultItem> result = new List<ResultItem>();

            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://jsonplaceholder.typicode.com/comments");
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            Parallel.ForEach(((IEnumerable<Comment>)JsonConvert.DeserializeObject(content)) //десериализация
                .Where(comment => comment.id % 2 == 0), //чётные комментарии
                comment => result.Add(new ResultItem(comment.id,
                comment.body.Where(character => char.IsLetter(character)).Count()))); //подсчёт букв
        }
    }
}
