// 

using System;
using UnityEngine;

namespace TDGameLibrary
{
    public abstract class WorldSubsystem<T> : MonoBehaviour
        where T : WorldSubsystem<T>
    {
        protected static T _Subsystem;


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
            if (_Subsystem != null)
            {
                return _Subsystem;
            }

            // @todo:这里也许以后会加一个开关？
            if (true)
            {
                GameObject NewObject = new GameObject();
                _Subsystem = NewObject.AddComponent<T>();
                return _Subsystem;
            }

            return null;
        }
    }


    public abstract class GameInstanceSubsystem<T> : WorldSubsystem<T>
        where T : GameInstanceSubsystem<T>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(_Subsystem);
        }
    }
}