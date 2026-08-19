// 用于解决字符串快速比较，并减少多重相同字符串内存占用

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TDGameLibrary;
using UnityEngine;

namespace TDGameLibrary
{
    internal class FNameSubsystem : GameInstanceSubsystem<FNameSubsystem>
    {
        static FNameSubsystem()
        {
            AutoCreateChecker = () => true;
        }
        
        private readonly Dictionary<string, FNameEntry> NamePool = new Dictionary<string, FNameEntry>();
        private readonly object PoolLock = new object();

        // 添加新 Name
        public FNameEntry AddNewName(string InName)
        {
            lock (PoolLock)
            {
                if (NamePool.TryGetValue(InName, out FNameEntry Name))
                {
                    return Name;
                }

                string InternedName = string.Intern(InName);
                FNameEntry NewNameEntry = new FNameEntry(InternedName);
                NamePool[InternedName] = NewNameEntry;
                return NewNameEntry;
            }
        }
    }

    internal class FNameEntry
    {
        public readonly string BaseString;
        
        public FNameEntry(string InBaseString)
        {
            BaseString = InBaseString;
        }
    }
}


[Serializable]
public struct FName : IEquatable<FName>, ISerializationCallbackReceiver
{
    [SerializeField] 
    private string Value; // 序列化 和 Inspector 专用！！
    private FNameEntry NameEntry;

    public FName(string InValue)
    {
        Value = null;
        NameEntry = null;
        Init(InValue);
    }
    
    /* 序列化 */
    public void OnBeforeSerialize()
    {
        Value = NameEntry?.BaseString;
    }
    public void OnAfterDeserialize()
    {
        Value ??= "";
        Init(Value);
        Value = null;
    }

    // 隐式转换：string -> FName
    //public static implicit operator FName(string InValue) => new(InValue);

    // 显式转换：FName -> string
    public static explicit operator string(FName InName) => InName.NameEntry.BaseString;
    
    
    /*  比较操作  */
    public static bool operator ==(FName A, FName B)
    {
        return ReferenceEquals(A.NameEntry, B.NameEntry);
    }
    public static bool operator !=(FName A, FName B) => !(A == B);
    public bool Equals(FName Other)
    {
        // 直接沿用 == 操作符的逻辑
        return this == Other;
    }
    public override bool Equals(object Obj)
    {
        if (Obj is FName OtherObj)
        {
            return Equals(OtherObj);
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(NameEntry);
    }
    
    
    /* 工具 */
    public override string ToString()
    {
        return NameEntry.BaseString;
    }
    
    public bool IsValid()
    {
        return NameEntry != null;
    }
    
    

    /* 私有初始化 */
    private void Init(string InValue)
    {
        NameEntry = FNameSubsystem.GetSubsystem().AddNewName(InValue);
    }
}