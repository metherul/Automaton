using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Automaton.Winforms
{
    public static class Extensions
    {
        public static IEnumerable<TOut> PMap<TIn, TOut>(this IEnumerable<TIn> coll, WorkQueue q, Func<TIn, TOut> f)
        {
            var promises = coll.Select(i => q.QueueTask(f, i)).ToList();
            return promises.Select(p =>
            {
                p.Wait();
                return p.Result;
            }).ToList();
        }

        public static HttpResponseMessage GetSync(this HttpClient client, string url)
        {
            var result = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            result.Wait();
            return result.Result;
        }
        public static string GetStringSync(this HttpClient client, string url)
        {
            var result = client.GetStringAsync(url);
            result.Wait();
            return result.Result;
        }

        public static Stream GetStreamSync(this HttpClient client, string url)
        {
            var result = client.GetStreamAsync(url);
            result.Wait();
            return result.Result;
        }
    }
}
