using Fastenshtein;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gearbox.SDK.Indexers
{
    public static class MatchResultReducer
    {
        /// <summary>
        /// Attempts to select the entry which is *most likely* the matching pair for the modFileEntry.
        /// This algorithm follow through several steps, aiming to reduce or select the most plausable 
        /// selections each time.
        /// 1. Reduce by non-matching file sizes. 
        /// 2. Reduce by non-matching file names.
        /// 3a. Sort possible matches via a string distance algorithm, computed against each's name.
        /// 3b. Try to select the first instance with a matching file extension in the sorted list.
        /// 4. No matches were found, so return the most similar entry.
        /// </summary>
        /// <param name="matchResults">The list of <see cref="MatchResult"/> to reduce.</param>
        /// <param name="modEntry">The source mod entry.</param>
        /// <param name="modFileEntry">The mod file entry to reduce against.</param>
        /// <param name="reduceOptions">Additional reduce options.</param>
        /// <returns>A singular <see cref="MatchResult"/> which is the most similar to <paramref name="modFileEntry"/>.</returns>
        public static MatchResult Reduce(this List<MatchResult> matchResults, ModEntry modEntry, FileEntry modFileEntry, ReduceOptions reduceOptions = null)
        {
            if (matchResults.Count == 1)
            {
                return matchResults[0];
            }

            if (matchResults.Count == 0)
            {
                return null;
            }

            if (reduceOptions == null)
            {
                reduceOptions = new ReduceOptions();
            }

            var preferredArchive = reduceOptions.PreferredArchiveName;

            // If there is a valid preferredArchiveName, presort the possible source entries.
            if (!string.IsNullOrEmpty(preferredArchive))
            {
                matchResults = matchResults.OrderByDescending(x => x.SourceArchive.Name == preferredArchive).ToList();
            }

            // Attempt to reduce by removing all non-matching file sizes.
            var reducedBySize = matchResults.Where(x => x.FileEntry.Length == modFileEntry.Length).ToList();

            // We have found our most likely match. 
            if (reducedBySize.Count == 1)
            {
                return reducedBySize[0];
            }

            // We found no matches, something broke. Set reducedBySize back to matchResults.
            else if (reducedBySize.Count == 0)
            {
                reducedBySize = matchResults;
            }

            // Next, filter files out that do not have the same filename.
            var reducedFileName = reducedBySize.Where(x => x.FileEntry.Name == modFileEntry.Name).ToList();

            // We have found our most likely match (size and name matches).
            if (reducedFileName.Count == 1)
            {
                return reducedFileName[0];
            }

            // No matches, so reset reducedFileName back to reducedBySize.
            else if (reducedFileName.Count == 0)
            {
                reducedFileName = reducedBySize;
            }

            // Use a fast string distance algorithm to sort each remaining archive file entry by its name and full path.
            // This algorithm is expanded upon when the preferredArchive is not null. Since we already know the preferredArchive,
            // we negate the negativity of the second distance call.
            var sortedDistanceByName = reducedFileName.OrderBy(x =>
                Levenshtein.Distance(x.FileEntry.Name, modFileEntry.Name) +
                (Levenshtein.Distance(Path.GetFileNameWithoutExtension(x.SourceArchive.Name), modEntry.Name) * 
                (preferredArchive == x.SourceArchive.Name ? 0 : 1)))
                .ToList();

            // Try to select the first value which has a matching file extension.
            var firstWithExt = sortedDistanceByName.FirstOrDefault(x => Path.GetExtension(x.FileEntry.Name) == Path.GetExtension(modFileEntry.Name));

            // The most similar looking file name with the matching extension.
            // We can attempt to improve the accuracy by only searching for a matching extension up to 
            // a value of n, but it's not worth the trouble right now.
            if (firstWithExt != null)
            {
                return firstWithExt;
            }

            // We couldn't find a matching extension, so pick the entry with the most similar name.
            return sortedDistanceByName[0];
        }
    }
}
