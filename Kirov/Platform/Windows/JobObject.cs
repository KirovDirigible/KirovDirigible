namespace Kirov.Platform.Windows;

using System.Runtime.InteropServices;

using static PInvoke.Kernel32;

class JobObject : IDisposable {
    readonly SafeObjectHandle handle;

    public JobObject(bool killOnClose = false, nuint? affinityMask = null) {
        this.handle = CreateJobObject(IntPtr.Zero, null);

        var limits = default(JOB_OBJECT_LIMIT_FLAGS);
        if (killOnClose) limits |= JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
        if (affinityMask is not null) limits |= JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_AFFINITY;

        var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION {
            LimitFlags = limits,
            Affinity = affinityMask ?? 0,
        };

        var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION { BasicLimitInformation = info };

        int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
        IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
        Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

        if (!SetInformationJobObject(this.handle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, extendedInfoPtr, (uint)length))
            throw new System.ComponentModel.Win32Exception();
    }

    public bool AddProcess(SafeObjectHandle processHandle) {
        return AssignProcessToJobObject(this.handle, processHandle);
    }

    #region IDisposable Members

    public void Dispose() {
        this.Close();
        GC.SuppressFinalize(this);
    }

    public void Close() => this.handle.Close();

    #endregion
}
