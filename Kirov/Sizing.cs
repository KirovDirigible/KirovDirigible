namespace Kirov {
    class Sizing {
        static readonly int[] commonSizes = new HashSet<int>{
            720, 768, 1280, 1024, 800, 1366, 1200, 900, 1440,
            960, 1050, 1600, 1680, 1920, 2048, 1152, 1536,
            2280, 2340, 2160, 2880, 2960, 2560, 2800, 2400,
            3200, 3000, 3840, 5096, 5120, 4480,
            6016,
        }.ToArray();

        public static (int w, int h) RandomSize(Random random) {
            int w = commonSizes.Sample(random);
            while (true) {
                int h = commonSizes.Sample(random);
                if (h > w/3 && h < w * 3) {
                    int rw = RandomizeDimension(w, random);
                    int rh = RandomizeDimension(h, random);
                    return (rw, rh);
                }
            }
        }

        static int RandomizeDimension(int dimension, Random random) {
            if ((random.Next() & 3) != 0) return dimension;

            int shrink = random.Next(maxValue: (dimension / 25) & ~1);
            return dimension - shrink;
        }
    }
}
