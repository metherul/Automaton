// using Gearbox.Indexing.Next.Model;
// using Gearbox.IO;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// 
// namespace Gearbox.Indexing.Next
// {
//     public class IndexWriter
//     {
//         private Index _index;
// 
//         public IndexWriter(Index index) => _ = _index;
// 
//         public async Task GenerateNew()
//         {
//             // Delete our pre-existing index.
//             if (File.Exists(_index.IndexPath))
//             {
//                 await AsyncFs.DeleteFile(_index.IndexPath);
//             }
// 
//             var indexRoot = new IndexRoot();
//             indexRoot.Mods = (await AsyncFs.GetDirectories(_index.ModsDir))
//                 .Select(x => new IndexMod()
//                 {
//                     Name = new DirectoryInfo(x).Name,
//                     Path = x,
//                 })
//                 .ToList();
// 
//             foreach (var mod in indexRoot.Mods)
//             {
//                 mod.Contents = (await AsyncFs.GetDirectoryFiles(mod.Path, "*.*", SearchOption.AllDirectories))
//                     .Select(async x =>
//                     {
//                         var fileInfo = new FileInfo(x)
//                     })
//             }
//         }
//     }
// 
//         public async Task Update()
//         {
// 
//         }
//     }
// }
