namespace RPG.DeveloperTools
{
    public class SlowLoad<T>
    {
        private T _value;
        private bool _initialized = false;
        private InitializerDelegate _initializer;

        public delegate T InitializerDelegate();

        public SlowLoad(InitializerDelegate initializer)
        {
            _initializer = initializer;
        }
        public T value
        {
            get
            {
                Initialize();
                return _value;
            }
            set
            {
                _initialized = true;
                _value = value;
            }
        }
        public void Initialize()
        {
            if (!_initialized)
            {
                _value = _initializer();
                _initialized = true;
            }
        }
    }
}