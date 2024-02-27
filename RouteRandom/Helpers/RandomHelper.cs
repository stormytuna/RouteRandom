using System;
using System.Collections.Generic;

namespace RouteRandom.Helpers;

public static class RandomHelper
{
    public static TSource NextFromCollection<TSource>(this Random rand, List<TSource> collection) {
        int randomIndex = rand.Next(collection.Count);
        return collection[randomIndex];
    }
}
