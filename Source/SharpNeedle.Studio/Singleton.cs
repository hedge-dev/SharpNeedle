namespace SharpNeedle.Studio
{
    public struct Singleton<T>
    {
        public static SingletonInstanceChangedDelegate<T> InstanceChanged;
        private static T StaticInstance { get; set; }
        public T Instance => StaticInstance;

        public Singleton(T instance)
        {
            SetInstance(instance);
        }

        public static void SetInstance(T instance)
        {
            var oldInstance = StaticInstance;
            StaticInstance = instance;

            RaiseInstanceChanged(oldInstance, StaticInstance);
        }

        public static T GetInstance()
        {
            return StaticInstance;
        }

        public static bool HasInstance()
        {
            return StaticInstance is not null;
        }

        private static void RaiseInstanceChanged(T oldInstance, T newInstance)
        {
            InstanceChanged?.Invoke(oldInstance, newInstance);
        }

        public static implicit operator T(Singleton<T> singleton) => GetInstance();
    }

    public struct Singleton
    {
        public static void AddInstanceChangedHandler<T>(SingletonInstanceChangedDelegate<T> handler) => Singleton<T>.InstanceChanged += handler;
        public static T GetInstance<T>() => Singleton<T>.GetInstance();
        public static void SetInstance<T>(T instance) => Singleton<T>.SetInstance(instance);
        public static bool HasInstance<T>() => Singleton<T>.HasInstance();
    }


    public delegate void SingletonInstanceChangedDelegate<in T>(T oldInstance, T newInstance);
}
