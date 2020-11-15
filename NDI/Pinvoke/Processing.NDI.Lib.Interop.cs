using System;
using System.Runtime.InteropServices;
using System.Security;
namespace NewTek {
#if AllowUpdate
    [SuppressUnmanagedCodeSecurity]
    public static partial class NDIlib {
        public static UInt32 NDILIB_CPP_DEFAULT_CONSTRUCTORS = 0;

        // This is not actually required, but will start and end the libraries which might get
        // you slightly better performance in some cases. In general it is more "correct" to
        // call these although it is not required. There is no way to call these that would have
        // an adverse impact on anything (even calling destroy before you've deleted all your
        // objects). This will return false if the CPU is not sufficiently capable to run NDILib
        // currently NDILib requires SSE4.2 instructions (see documentation). You can verify
        // a specific CPU against the library with a call to NDIlib_is_supported_CPU()
        public static bool initialize() {
            if (IntPtr.Size == 8)
                return UnsafeNativeMethods.initialize_64();
            else
                return UnsafeNativeMethods.initialize_32();
        }

        public static void destroy() {
            if (IntPtr.Size == 8)
                UnsafeNativeMethods.destroy_64();
            else
                UnsafeNativeMethods.destroy_32();
        }

        public static IntPtr version() {
            if (IntPtr.Size == 8)
                return UnsafeNativeMethods.version_64();
            else
                return UnsafeNativeMethods.version_32();
        }

        // Recover whether the current CPU in the system is capable of running NDILib.
        public static bool is_supported_CPU() {
            if (IntPtr.Size == 8)
                return UnsafeNativeMethods.is_supported_CPU_64();
            else
                return UnsafeNativeMethods.is_supported_CPU_32();
        }

        [SuppressUnmanagedCodeSecurity]
        internal static partial class UnsafeNativeMethods {
            // initialize 
            [DllImport("Processing.NDI.Lib.x64.dll", EntryPoint = "NDIlib_initialize", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAsAttribute(UnmanagedType.U1)]
            internal static extern bool initialize_64();
            [DllImport("Processing.NDI.Lib.x86.dll", EntryPoint = "NDIlib_initialize", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAsAttribute(UnmanagedType.U1)]
            internal static extern bool initialize_32();

            // destroy 
            [DllImport("Processing.NDI.Lib.x64.dll", EntryPoint = "NDIlib_destroy", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void destroy_64();
            [DllImport("Processing.NDI.Lib.x86.dll", EntryPoint = "NDIlib_destroy", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern void destroy_32();

            // version 
            [DllImport("Processing.NDI.Lib.x64.dll", EntryPoint = "NDIlib_version", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr version_64();
            [DllImport("Processing.NDI.Lib.x86.dll", EntryPoint = "NDIlib_version", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr version_32();

            // is_supported_CPU 
            [DllImport("Processing.NDI.Lib.x64.dll", EntryPoint = "NDIlib_is_supported_CPU", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAsAttribute(UnmanagedType.U1)]
            internal static extern bool is_supported_CPU_64();
            [DllImport("Processing.NDI.Lib.x86.dll", EntryPoint = "NDIlib_is_supported_CPU", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAsAttribute(UnmanagedType.U1)]
            internal static extern bool is_supported_CPU_32();
        } // UnsafeNativeMethods
    } // class NDIlib
#endif
} // namespace NewTek