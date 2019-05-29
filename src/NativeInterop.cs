﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChipmunkBinding
{
    internal static class NativeInterop
    {
        public static IntPtr RegisterHandle(object obj)
        {
            var gcHandle = GCHandle.Alloc(obj);
            return GCHandle.ToIntPtr(gcHandle);
        }

        public static T FromIntPtr<T>(IntPtr pointer)
        {
            Debug.Assert(pointer != IntPtr.Zero, "IntPtr parameter should never be Zero");

            var handle = GCHandle.FromIntPtr(pointer);

            Debug.Assert(handle.IsAllocated, "GCHandle not allocated.");
            Debug.Assert(handle.Target is T, "Target is not of type T.");

            return (T)handle.Target;
        }

        public static void ReleaseHandle(IntPtr pointer)
        {
            Debug.Assert(pointer != IntPtr.Zero, "IntPtr parameter should never be Zero");

            var handle = GCHandle.FromIntPtr(pointer);

            Debug.Assert(handle.IsAllocated, "GCHandle not allocated.");

            handle.Free();
        }

        public static int SizeOf<T>()
        {
#if NETFRAMEWORK
            return Marshal.SizeOf(typeof(T));
#else
            return Marshal.SizeOf<T>();
#endif
        }

        public static IntPtr AllocStructure<T>()
        {
            int size = SizeOf<T>();
            return Marshal.AllocHGlobal(size);
        }

        public static void FreeStructure(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        public static T PtrToStructure<T>(IntPtr intPtr)
        {
#if NETFRAMEWORK
            return (T)Marshal.PtrToStructure(intPtr, typeof(T));
#else
            return Marshal.PtrToStructure<T>(intPtr);
#endif
        }

        public static T[] PtrToStructureArray<T>(IntPtr intPtr, int count)
        {
            var items = new T[count];
            var size = SizeOf<T>();

            for (var i = 0; i < count; i++)
            {
                var newPtr = new IntPtr(intPtr.ToInt64() + (i * size));
                items[i] = PtrToStructure<T>(newPtr);
            }
            return items;
        }

        public static T PtrToStructure<T>(IntPtr intPtr, int index)
        {
            var size = SizeOf<T>();
            var newPtr = new IntPtr(intPtr.ToInt64() + (index * size));
            return PtrToStructure<T>(newPtr);
        }
    }
}