namespace Kirov.Platform;

using System.Globalization;

class Affinity {
    public static nuint? GetDesiredAffinity() {
        string? affinityVar = Environment.GetEnvironmentVariable("AFFINITY");
        if (affinityVar is not null) return nuint.Parse(affinityVar, CultureInfo.InvariantCulture);

        string? maxCoresVar = Environment.GetEnvironmentVariable("MAX_CORES");
        int maxCores = maxCoresVar is null
            ? Math.Max(1, Environment.ProcessorCount - 6) // leave 3 full physical cores to the user
            : int.Parse(maxCoresVar, CultureInfo.InvariantCulture);
        return AffinityFromMaxCores(maxCores);
    }

    static nuint AffinityFromMaxCores(int maxCores) {
        if (maxCores <= 0) throw new ArgumentOutOfRangeException(nameof(maxCores));

        int logicalCores = Environment.ProcessorCount;

        if (maxCores > logicalCores)
            throw new ArgumentOutOfRangeException(
                paramName: nameof(maxCores),
                message: "The system does not have the requested amount of logical cores");

        return ((nuint)1 << maxCores) - 1;
    }
}
