namespace Kirov.Platform;
interface IPlatform {
    void Initialize();
}

interface ISupportAffinity {
    nuint? AffinityMask { get; set; }
}
