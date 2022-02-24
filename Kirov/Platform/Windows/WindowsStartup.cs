namespace Kirov.Platform.Windows {
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using static PInvoke.Kernel32;

    class WindowsStartup {
        static JobObject? jobObject;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Startup() {
            // ensures when the process dies or stopped from IDE, all launched browsers are killed too
            jobObject = new JobObject(JOB_OBJECT_LIMIT_FLAGS.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE);
            var selfHandle = new SafeObjectHandle(Process.GetCurrentProcess().Handle, ownsHandle: false);
            jobObject.AddProcess(selfHandle);
        }
    }
}
