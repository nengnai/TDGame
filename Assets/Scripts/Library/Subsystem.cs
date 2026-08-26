// 

using System;
using UnityEngine;

namespace TDGameLibrary
{
    public abstract class WorldSubsystem<T> : MonoBehaviour
        where T : WorldSubsystem<T>
    {
        protected static T _Subsystem;
        
        // 是否需要无效时自动创建
        protected static Func<bool> AutoCreateChecker = () => true;


        protected virtual void Awake()
        {
            if (_Subsystem != null && _Subsystem != this)
            {
                Destroy(gameObject); // 防止重复实例
                return;
            }

            // Subsystem本身是抽象的，所以直接强转
            _Subsystem = this as T;
        }

        public static T GetSubsystem()
        {
            if (_Subsystem)
            {
                return _Subsystem;
            }
            
            if (AutoCreateChecker())
            {
                GameObject NewObject = new GameObject();
                _Subsystem = NewObject.AddComponent<T>();
                return _Subsystem;
            }

            return null;
        }

        /*[RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if (_Subsystem) { return; }
            
            GameObject NewObject = new();
            _Subsystem = NewObject.AddComponent<T>();
        }*/
    }


    public abstract class GameInstanceSubsystem<T> : WorldSubsystem<T>
        where T : GameInstanceSubsystem<T>
    {
        protected override void Awake()
        {
            if (_Subsystem == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            base.Awake();
        }
    }
}