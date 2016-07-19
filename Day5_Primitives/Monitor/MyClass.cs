namespace Monitor
{
    // TODO: Use Monitor (not lock) to protect this structure.
    public class MyClass
    {
        private int _value;

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
            _value++;
        }

        public void Decrease()
        {
            _value--;
        }
    }
}
