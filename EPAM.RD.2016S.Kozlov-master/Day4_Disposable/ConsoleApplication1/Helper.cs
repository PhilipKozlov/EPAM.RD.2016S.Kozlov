using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    public static class Helper
    {
        public static IntPtr AllocateBuffer() { return Marshal.AllocHGlobal(1024); }
        public static void DeallocateBuffer(IntPtr ptr) { Marshal.FreeHGlobal(ptr); }
        public static SafeHandle GetResource() { return new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true); }
    }
}
