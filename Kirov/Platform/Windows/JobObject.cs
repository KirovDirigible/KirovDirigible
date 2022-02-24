namespace Kirov.Platform.Windows {
    using System.Runtime.InteropServices;

    using static PInvoke.Kernel32;

    class JobObject : IDisposable {
        readonly SafeObjectHandle handle;

        public JobObject(JOB_OBJECT_LIMIT_FLAGS limits = default) {
            this.handle = CreateJobObject(IntPtr.Zero, null);

            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION {
                LimitFlags = limits,
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
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Dispose(bool disposing) => this.Close();

        public void Close() => this.handle.Close();

        #endregion
    }
}
