namespace Kirov {
    static class Randomization {
        public static T Sample<T>(this IReadOnlyList<T> list, Random random)
            => list[random.Next(list.Count)];
    }
}
