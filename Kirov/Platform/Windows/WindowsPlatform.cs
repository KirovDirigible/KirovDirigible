namespace Kirov.Platform.Windows;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

using static PInvoke.Kernel32;

using ProcessPriorityClass = System.Diagnostics.ProcessPriorityClass;

[SupportedOSPlatform("windows")]
class WindowsPlatform : IPlatform, ISupportAffinity {
    static JobObject? jobObject;

    public nuint? AffinityMask { get; set; }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Initialize() {
        if (this.AffinityMask is { } affinity)
            Process.GetCurrentProcess().ProcessorAffinity = (nint)affinity;

        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Idle;

        // ensures when the process dies or stopped from IDE, all launched browsers are killed too
        jobObject = new JobObject(killOnClose: true, affinityMask: this.AffinityMask);
        var selfHandle = new SafeObjectHandle(Process.GetCurrentProcess().Handle, ownsHandle: false);
        jobObject.AddProcess(selfHandle);
    }
}
