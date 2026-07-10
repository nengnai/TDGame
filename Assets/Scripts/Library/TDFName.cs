// 

using System;
using UnityEngine;

namespace TDGameLibrary
{
    
    public class TDFNameManager
    {
        // 先这样吧，又不是不能用
        // 主要是用来解决哈希碰撞的管理器，不加就是稍微慢点
    }

    [Serializable]
    public struct FName : IEquatable<FName>, IComparable<FName>, ISerializationCallbackReceiver
    {
        private int ID;
        [SerializeField] 
        private string Value;

        public FName(string InValue)
        {
            this.ID = 0;
            this.Value = null;
            // 委托给统一的初始化方法
            Init(InValue);
        }

        // 隐式转换：string -> FName
        public static implicit operator FName(string InValue) => new(InValue);

        // 显式转换：FName -> string
        public static explicit operator string(FName InName) => InName.Value;
        
        
        /*  比较操作  */
        public static bool operator ==(FName A, FName B)
        {
            return A.ID == B.ID && A.Value == B.Value;
        }
        public static bool operator !=(FName A, FName B) 
            => !(A == B);
        public bool Equals(FName Other)
        {
            // 直接沿用 == 操作符的逻辑
            return this == Other;
        }
        public override bool Equals(object obj)
        {
            if (obj is FName other)
            {
                return Equals(other);
            }
            return false;
        }
        public int CompareTo(FName other)
        {
            // 先按 ID 排序，再按 Value 排序，与相等逻辑保持协调
            int IDComparison = ID.CompareTo(other.ID);
            if (IDComparison != 0) return IDComparison;
            return Value.CompareTo(other.Value);
        }
        
        public override int GetHashCode()
        {
            // 必须与 == 使用的字段一致：ID 和 Value
            return Value.GetHashCode();
        }
        
        
        
        public override string ToString() => Value ?? string.Empty;
        
        /* 序列化 */
        public void OnBeforeSerialize()
        {
            
        }
        public void OnAfterDeserialize()
        {
            Init(Value);
        }

        /* 私有初始化 */
        private void Init(string InValue)
        {
            if (string.IsNullOrEmpty(InValue))
            {
                Value = string.Empty;
                ID = 0;
            }
            else
            {
                Value = string.Intern(InValue);
                ID = Value.GetHashCode();
            }
        }
    }
    }