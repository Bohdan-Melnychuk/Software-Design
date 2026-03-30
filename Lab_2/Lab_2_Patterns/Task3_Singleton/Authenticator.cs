using System;
using System.Threading;

namespace Lab2_Patterns.Task3_Singleton
{
    public sealed class Authenticator
    {
        private static Authenticator _instance;
        
        private static readonly object _lock = new object();
        
        private readonly Guid _instanceId;
        
        private int _accessCount;
        
        private Authenticator()
        {
            _instanceId = Guid.NewGuid();
            _accessCount = 0;
            Console.WriteLine($"[СТВОРЕНО] Новий екземпляр Authenticator з ID: {_instanceId}");
        }
        
        public static Authenticator GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Authenticator();
                    }
                }
            }
            return _instance;
        }
        
        public Guid InstanceId => _instanceId;
        
        public bool Authenticate(string username, string password)
        {
            _accessCount++;
            Console.WriteLine($"[АУТЕНТИФІКАЦІЯ] Користувач: {username}, " + $"Екземпляр ID: {_instanceId}, " + $"Звернень: {_accessCount}");
            
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }
        
        public static void ResetInstance()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}