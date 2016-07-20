using System.Threading;

namespace Monitor
{
    // TODO: Use SpinLock to protect this structure.
    public class AnotherClass
    {
        private int _value;

        private SpinLock sl = new SpinLock();

        public int Counter
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public void Increase()
        {
            bool islockTaken = false;
            try
            {
                sl.Enter(ref islockTaken);
                _value++;
            }
            finally
            {
                if (islockTaken) sl.Exit(true);
                islockTaken = false;
            }
            
        }

        public void Decrease()
        {
            bool islockTaken = false;
            try
            {
                sl.Enter(ref islockTaken);
                _value--;
            }
            finally
            {
                if (islockTaken) sl.Exit(true);
                islockTaken = false;

            }
        }
    }
}
