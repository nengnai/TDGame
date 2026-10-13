// 用于解决字符串快速比较，并减少多重相同字符串内存占用

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TDGameLibrary;
using UnityEngine;

namespace TDGameLibrary
{
    /// <summary>
    /// 全局唯一的 Name 池(纯静态，不依赖 Unity 生命周期)。
    ///
    /// 为什么必须是静态的：Unity 在编辑器里同样会调用反序列化回调(OnAfterDeserialize)，
    /// 如果池子只挂在 Subsystem 上，反序列化任何资产/prefab 都会走 GetSubsystem() -> new GameObject()，
    /// 凭空产生对象并污染当前场景。做成静态类后，编辑期与运行期共用同一份池，行为一致。
    /// </summary>
    internal static class FNamePool
    {
        private static readonly Dictionary<string, FNameEntry> NamePool = new Dictionary<string, FNameEntry>(StringComparer.Ordinal);
        private static readonly object PoolLock = new object();

        // 添加新 Name(已存在则直接返回同一个 Entry)
        public static FNameEntry AddNewName(string InName)
        {
            InName ??= string.Empty;

            lock (PoolLock)
            {
                if (NamePool.TryGetValue(InName, out FNameEntry Name))
                {
                    return Name;
                }

                // Intern 保证"同一内容只有一份 string 实例"，池内所有引用共享它
                string InternedName = string.Intern(InName);
                FNameEntry NewNameEntry = new FNameEntry(InternedName);
                NamePool[InternedName] = NewNameEntry;
                return NewNameEntry;
            }
        }

        public static int Count
        {
            get { lock (PoolLock) { return NamePool.Count; } }
        }
    }


    /// <summary>
    /// 全局子系统：对外提供 Unity 侧的访问点，真正的存储是静态的 FNamePool。
    /// </summary>
    internal class FNameSubsystem : GameInstanceSubsystem<FNameSubsystem>
    {
        static FNameSubsystem()
        {
            AutoCreateChecker = () => true;
        }

        public FNameEntry AddNewName(string InName) => FNamePool.AddNewName(InName);

        public int NameCount => FNamePool.Count;
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
    /* 序列化载荷：Inspector / 资产里保存的就是这个字符串 */
    [SerializeField]
    private string Value;

    /* 运行期身份：指向全局池中的唯一条目，比较与哈希都基于它 */
    [NonSerialized]
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
        Value ??= string.Empty;

        // 字符串和运行期条目不一致 => 说明是外部(Inspector / 其它序列化器)刚写进来的新值，以字符串为准
        if (Value.Length > 0 &&
            (NameEntry == null || !string.Equals(NameEntry.BaseString, Value, StringComparison.Ordinal)))
        {
            NameEntry = FNamePool.AddNewName(Value);
        }

        // 条目有效时写回规范字符串(default 实例会写出空串)
        Value = NameEntry != null ? NameEntry.BaseString : string.Empty;
    }

    public void OnAfterDeserialize()
    {
        Init(Value);
    }

    // 读取时的兜底：某些序列化路径不会回调结构体，此时直接按字符串解析，
    // 保证"序列化出来的字符串"永远能变回可比较的条目。
    private FNameEntry EnsureEntry()
    {
        if (NameEntry != null) return NameEntry;
        if (string.IsNullOrEmpty(Value)) return null;
        return FNamePool.AddNewName(Value);
    }

    // 隐式转换：string -> FName
    //public static implicit operator FName(string InValue) => new(InValue);

    // 显式转换：FName -> string
    public static explicit operator string(FName InName)
    {
        FNameEntry Entry = InName.EnsureEntry();
        return Entry != null ? Entry.BaseString : string.Empty;
    }


    /*  比较操作  */
    public static bool operator ==(FName A, FName B)
    {
        return ReferenceEquals(A.EnsureEntry(), B.EnsureEntry());
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
        FNameEntry Entry = EnsureEntry();
        return Entry != null ? RuntimeHelpers.GetHashCode(Entry) : 0;
    }


    /* 工具 */
    public override string ToString()
    {
        FNameEntry Entry = EnsureEntry();
        if (Entry != null) return Entry.BaseString;
        return Value ?? string.Empty;
    }

    public bool IsValid()
    {
        return EnsureEntry() != null;
    }



    /* 私有初始化 */
    private void Init(string InValue)
    {
        NameEntry = string.IsNullOrEmpty(InValue) ? null : FNamePool.AddNewName(InValue);

        // Value 指向池里 Intern 过的唯一字符串：同一内容全进程只有一份字符串，
        // 每个 FName 只多付一个引用(8 字节)，不会因为 Unity 反序列化出的副本而重复占内存。
        // 注意：这里不能置 null —— 字段留着才能在"序列化器不回调 OnBeforeSerialize"时依然写出完整数据。
        Value = NameEntry != null ? NameEntry.BaseString : string.Empty;
    }
}
