using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gearbox.Indexing;
using Gearbox.Indexing.Interfaces;
using Gearbox.IO;

namespace Gearbox.Compiling
{
    public class ModLinker
    {
        private readonly IndexBase _indexBase;
        private readonly ICollection<IIndexEntry> _entriesWithoutMatches = new List<IIndexEntry>();

        private ICollection<MatchResult> _sourceEntries;
        public ModLinker(IndexBase indexBase)
        {
            _indexBase = indexBase;
        }

        public async Task LoadSources(ICollection<IIndexHeader> sources)
        {
            var sourceEntries = new List<MatchResult>();
            
            foreach (var source in sources)
            {
                sourceEntries.AddRange(source.IndexEntries.Select(x => new MatchResult()
                {
                    SourceHeader = source,
                    SourceEntry = x
                }));
            }

            _sourceEntries = sourceEntries;
        }
        
        public async Task<MatchResult?> FindBestMatches(IIndexEntry modEntry,  IIndexHeader idealParentSource = null)
        {
            if (idealParentSource != null)
            {
                _sourceEntries = _sourceEntries
                    .OrderByDescending(x => x.SourceHeader == idealParentSource)
                    .AsParallel()
                    .ToList();
            }
            
            var matchesByHash = _sourceEntries
                .Where(x => x.SourceEntry.Hash == modEntry.Hash)
                .AsParallel()
                .ToList();

            if (!matchesByHash.Any())
            {
                return null;
            }

            if (matchesByHash.Count() == 1)
            {
               
               return matchesByHash.First();
            }

            var matchesByName = matchesByHash
                .Where(x => Path.GetFileName(x.SourceEntry.RelativeFilePath) == Path.GetFileName(modEntry.RelativeFilePath))
                .ToList();

            if (matchesByName.Any() && matchesByName.Count() == 1)
            {
                return matchesByName.First();
            }

            var matchesByDistance = matchesByHash
                .OrderByDescending(x => x.SourceHeader == idealParentSource)
                .ThenBy(x => LevenshteinDistance(x.SourceEntry.RelativeFilePath, modEntry.RelativeFilePath))
                .AsParallel();

            return matchesByDistance.First();
        }

        public IIndexHeader GetIdealSource(string filePath)
        {
            return _sourceEntries.First(x => x.SourceHeader.RawPath == PathExtensions.NormalizeFilePath(filePath)).SourceHeader;
        }
        
        private int LevenshteinDistance(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];
 
            // Step 1
            if (n == 0)
            {
                return m;
            }
 
            if (m == 0)
            {
                return n;
            }
 
            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }
 
            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }
 
            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
 
                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }

    public struct  MatchResult
    {
        public IIndexHeader SourceHeader;
        public IIndexEntry SourceEntry;
    }

    enum LastMatchType
    {
        Hash,
        Name,
        Distance
    }
}