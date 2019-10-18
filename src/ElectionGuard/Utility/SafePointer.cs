using System;

namespace ElectionGuard.SDK.Utility
{
    public class SafePointer
    {
        protected T Protect<T>(UIntPtr pointer, Func<T> func)
        {
            if (pointer == UIntPtr.Zero)
            {
                throw new NullReferenceException();
            }
            return func();
        }

        protected T Protect<T>(IntPtr pointer, Func<T> func)
        {
            if (pointer == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }
            return func();
        }

        protected void ProtectVoid(UIntPtr pointer, Action func)
        {
            if (pointer == UIntPtr.Zero)
            {
                return;
            }
            func();
        }

        protected void ProtectVoid(IntPtr pointer, Action func)
        {
            if (pointer == IntPtr.Zero)
            {
                return;
            }
            func();
        }
    }
}