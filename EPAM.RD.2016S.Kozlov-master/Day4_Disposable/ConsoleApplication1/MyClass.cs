using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    // TODO: The code below contains a lot of issues. Please, fix all of them.
    // Use as a guidelines:
    // https://msdn.microsoft.com/en-us/library/b1yfkh5e(v=vs.110).aspx
    // https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.100%29.aspx?f=255&MSPPError=-2147217396
    // https://msdn.microsoft.com/en-us/library/fs2xkftw(v=vs.110).aspx
    public class MyClass : IDisposable
    {
        private IntPtr _buffer;       // unmanaged resource
        private SafeHandle _resource; // managed resource
        private bool _disposed = false;

        public MyClass()
        {
            _buffer = Helper.AllocateBuffer();
            _resource = Helper.GetResource();
        }

        ~MyClass()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // TODO: Add your implementations here.
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose of managed object
                    if (_resource != null) _resource.Dispose();
                }

                // dispose of unmanaged object
                if (_buffer != null) Helper.DeallocateBuffer(_buffer);

                _disposed = true;
            }
        }

        public void DoSomething()
        {
            // NOTE: Manipulation with _buffer and _resource in this line.
            if (_disposed) throw new ObjectDisposedException(nameof(_resource));

            // resoursce manipulations
            Marshal.WriteInt32(_buffer,123);
            Console.WriteLine(Marshal.ReadInt32(_buffer));
            Console.WriteLine(_resource.IsClosed);

        }
    }
}
