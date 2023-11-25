using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteRandom.Helpers
{
    public static class RandomHelper
    {
        public static TSource NextFromCollection<TSource>(this Random rand, IList<TSource> collection) {
            int randomIndex = rand.Next(collection.Count());
            return collection[randomIndex];
        }
    }
}
