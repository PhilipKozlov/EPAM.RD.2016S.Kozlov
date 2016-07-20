namespace Monitor
{
    // TODO: Use Monitor (not lock) to protect this structure.
    public class MyClass
    {
        private int _value;

        private readonly object locker = new object();

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
            System.Threading.Monitor.Enter(locker);
            try
            {
                _value++;
            }
            finally
            {
                System.Threading.Monitor.Exit(locker);
            }
        }

        public void Decrease()
        {
            System.Threading.Monitor.Enter(locker);
            try
            {
                _value--;
            }
            finally
            {
                System.Threading.Monitor.Exit(locker);
            }
        }
    }
}
