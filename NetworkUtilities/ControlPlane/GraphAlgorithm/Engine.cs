using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane.GraphAlgorithm {
    public static class Engine {
        public static LinkedList<Path<T>> CalculateShortestPathBetween<T>(T source, T destination,
            IEnumerable<Path<T>> paths) {
            return CalculateFrom(source, paths)[destination];
        }

        public static Dictionary<T, LinkedList<Path<T>>> CalculateShortestFrom<T>(T source, IEnumerable<Path<T>> paths) {
            return CalculateFrom(source, paths);
        }

        private static Dictionary<T, LinkedList<Path<T>>> CalculateFrom<T>(T source, IEnumerable<Path<T>> paths) {
            // validate the paths
            if (paths.Any(p => p.Source.Equals(p.Destination)))
                throw new ArgumentException("No path can have the same source and destination");


            // keep track of the shortest paths identified thus far
            var shortestPaths =
                new Dictionary<T, KeyValuePair<int, LinkedList<Path<T>>>>();
            // keep track of the locations which have been completely processed
            var locationsProcessed = new List<T>();

            // include all possible steps, with Int.MaxValue cost
            paths.SelectMany(p => new[] {p.Source, p.Destination}) // union source and destinations
                .Distinct() // remove duplicates
                .ToList() // ToList exposes ForEach
                .ForEach(s => shortestPaths.Set(s, int.MaxValue, null)); // add to ShortestPaths with MaxValue cost

            // update cost for self-to-self as 0; no path
            shortestPaths.Set(source, 0, null);

            // keep this cached
            var locationCount = shortestPaths.Keys.Count;

            while (locationsProcessed.Count < locationCount) {
                var locationToProcess = default(T);

                //Search for the nearest location that isn't handled
                foreach (var location in shortestPaths.OrderBy(p => p.Value.Key).Select(p => p.Key).ToList())
                    if (!locationsProcessed.Contains(location)) {
                        if (shortestPaths[location].Key == int.MaxValue)
                            return shortestPaths.ToDictionary(k => k.Key, v => v.Value.Value);
                        //ShortestPaths[destination].Value;

                        locationToProcess = location;
                        break;
                    }

                var selectedPaths = paths.Where(p => p.Source.Equals(locationToProcess));
                foreach (var path in selectedPaths)
                    if (shortestPaths[path.Destination].Key > path.Cost + shortestPaths[path.Source].Key)
                        shortestPaths.Set(
                            path.Destination,
                            path.Cost + shortestPaths[path.Source].Key,
                            shortestPaths[path.Source].Value.Union(new[] {path}).ToArray());

                //Add the location to the list of processed locations
                locationsProcessed.Add(locationToProcess);
            } // while

            return shortestPaths.ToDictionary(k => k.Key, v => v.Value.Value);
            //return ShortestPaths[destination].Value;
        }
    }


    public static class ExtensionMethods {
        public static void Set<T>(this Dictionary<T, KeyValuePair<int, LinkedList<Path<T>>>> dictionary, T destination,
            int cost, params Path<T>[] paths) {
            var completePath = paths == null ? new LinkedList<Path<T>>() : new LinkedList<Path<T>>(paths);
            dictionary[destination] = new KeyValuePair<int, LinkedList<Path<T>>>(cost, completePath);
        }
    }
}