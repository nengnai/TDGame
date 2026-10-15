using System;
using System.Collections;
using System.Collections.Generic;
using TDGameLibrary;
using UnityEngine;

internal class GameTagNode
{
    public FName InFullName; // 完整路径，例如 Gameplay.技能.技能A
    public FName InName; // 绝对名，例如 技能A
    public GameTagNode InParent; // 父节点(根节点为 null)
    public readonly List<GameTagNode> Child = new(); // 子节点

    public GameTagNode(FName InFullName, FName InName, GameTagNode InParent)
    {
        this.InFullName = InFullName;
        this.InName = InName;
        this.InParent = InParent;
    }

    public bool IsRoot => InParent == null;

    public override string ToString() => InFullName.ToString();
}


// 全局标签注册表：所有节点及节点关系的唯一持有者
// 纯静态，编辑期(资产 / prefab 反序列化)与运行期共用同一份数据
internal static class GameTagRegistry
{
    private static readonly object RegistryLock = new(); // 线程锁
    private static readonly Dictionary<string, GameTagNode> AllNodes = new();

    public static readonly GameTagNode RootNode = new(new FName("Root"), new FName("Root"), null);

    static GameTagRegistry()
    {
        AllNodes["Root"] = RootNode;
    }
    
    // 查找路径对应的节点，没有时创建(连同沿途缺失的父节点)
    public static GameTagNode GetNode(string FullName)
    {
        if (string.IsNullOrEmpty(FullName)) return null;

        lock (RegistryLock)
        {
            // 快路径：已注册的路径直接命中，不再 Split
            // 构造 Tag 是热路径(索敌里每个候选每帧都在 new FGameTag)，Split 会每次分配数组与子串
            if (AllNodes.TryGetValue(FullName, out GameTagNode Existing)) return Existing;

            string[] Parts = FullName.Split('.');
            GameTagNode CurrentParent = RootNode;
            string CurrentPath = null;

            foreach (string Part in Parts)
            {
                CurrentPath = string.IsNullOrEmpty(CurrentPath) ? Part : CurrentPath + "." + Part;

                if (!AllNodes.TryGetValue(CurrentPath, out GameTagNode CurrentNode))
                {
                    CurrentNode = new GameTagNode(new FName(CurrentPath), new FName(Part), CurrentParent);
                    AllNodes.Add(CurrentPath, CurrentNode);
                    CurrentParent.Child.Add(CurrentNode);
                }

                CurrentParent = CurrentNode;
            }

            return CurrentParent;
        }
    }

    /// 只查不建
    public static bool TryGetNode(string FullName, out GameTagNode Node)
    {
        Node = null;
        if (string.IsNullOrEmpty(FullName)) return false;

        lock (RegistryLock)
        {
            return AllNodes.TryGetValue(FullName, out Node);
        }
    }

    public static int NodeCount
    {
        get { lock (RegistryLock) { return AllNodes.Count; } }
    }

    /// 拷贝所有已注册节点(供编辑器面板 / 调试工具使用)
    public static void CopyAllNodes(List<GameTagNode> Out)
    {
        if (Out == null) return;

        lock (RegistryLock)
        {
            Out.Clear();
            Out.AddRange(AllNodes.Values);
        }
    }
}



/// 单个标签
[Serializable]
public struct FGameTag : IEquatable<FGameTag>, ISerializationCallbackReceiver
{
    // 序列化载荷 只在(反)序列化瞬间有值，运行期一律为 null。
    [SerializeField]
    private string Name;

    // 运行时实际使用
    [NonSerialized]
    internal GameTagNode Node;


    public FGameTag(string Value)
    {
        Node = GameTagRegistry.GetNode(Value);

        // 解析失败(空串)时保留原文便于排查；解析成功就丢掉字符串，运行期只留节点
        Name = Node != null ? null : (Value ?? string.Empty);
    }

    // 由已有节点直接构造(容器做集合操作时避免再走一次字符串查询)
    internal FGameTag(GameTagNode InNode)
    {
        Node = InNode;
        Name = null;
    }


    /* 序列化 */
    public void OnBeforeSerialize()
    {
        string NodeName = Node != null ? Node.InFullName.ToString() : null;

        // 字符串非空且与运行期节点不一致 => 外部(Inspector / 其它序列化器)刚改过，以字符串为准
        if (!string.IsNullOrEmpty(Name) && !string.Equals(Name, NodeName, StringComparison.Ordinal))
        {
            Node = GameTagRegistry.GetNode(Name);
            NodeName = Node != null ? Node.InFullName.ToString() : null;
        }

        Name = NodeName ?? string.Empty;
    }

    public void OnAfterDeserialize()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Node = null;
            return;
        }

        Node = GameTagRegistry.GetNode(Name);

        // 解析成功后立刻丢掉字符串
        if (Node != null) Name = null;
    }


    /*  比较操作  */
    public static bool operator ==(FGameTag A, FGameTag B)
    {
        return A.Equals(B);
    }
    public static bool operator !=(FGameTag A, FGameTag B) => !(A == B);

    
    public bool Equals(FGameTag Other)
    {
        return ReferenceEquals(Node, Other.Node);
    }
    public override bool Equals(object Obj)
    {
        return Obj is FGameTag Other && Equals(Other);
    }

    public override int GetHashCode()
    {
        return (Node != null ? Node.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return Node != null ? Node.InFullName.ToString() : string.Empty;
    }

    public bool IsValid()
    {
        return Node != null;
    }
}




// 标签容器
[Serializable]
public class GameTagContainer : IEnumerable<FGameTag>, ISerializationCallbackReceiver
{
    // 只在 写出读入 的瞬间存在，运行期一律为 null
    [SerializeField]
    private List<FGameTag> SerializedTags;

    [NonSerialized]
    private HashSet<FGameTag> ExactTag = new();

    // 记录自身或子项被拥有的次数，用于父级匹配
    [NonSerialized]
    private Dictionary<GameTagNode, int> ImplicitTag = new();

    // 运行期索引是否已经和 SerializedTags 同步
    [NonSerialized]
    private bool IndexReady;


    public bool IsEmpty
    {
        get { EnsureIndex(); return ExactTag.Count == 0; }
    }

    public int Count
    {
        get { EnsureIndex(); return ExactTag.Count; }
    }


    /* ---- 序列化 ---- */
    public void OnBeforeSerialize()
    {
        if (SerializedTags == null)
        {
            EnsureIndex();

            SerializedTags ??= new List<FGameTag>();
            SerializedTags.Clear();

            foreach (FGameTag Tag in ExactTag)
            {
                // 在副本上补全字符串：这样即使序列化器不回调元素，写出的也是完整路径
                FGameTag SerializableTag = Tag;
                SerializableTag.OnBeforeSerialize();
                SerializedTags.Add(SerializableTag);
            }
        }

        // 让序列化顺序稳定
        SerializedTags.Sort(CompareTagPath);
    }

    public void OnAfterDeserialize()
    {
        IndexReady = false;
        EnsureIndex();
    }

    private static int CompareTagPath(FGameTag A, FGameTag B)
        => string.CompareOrdinal(A.ToString(), B.ToString());


    /* ---- 运行期索引 ---- */
    private void EnsureIndex()
    {
        if (IndexReady)
        {
            SerializedTags = null;
            return;
        }

        RebuildIndex();
    }

    private void RebuildIndex()
    {
        // AddTag 内部还会调 EnsureIndex，避免递归
        IndexReady = true;
        
        ExactTag ??= new HashSet<FGameTag>();
        ImplicitTag ??= new Dictionary<GameTagNode, int>();
        ExactTag.Clear();
        ImplicitTag.Clear();

        // 先摘下载荷再清空字段: AddTag 内部会调 EnsureIndex(它会把字段置 null)，若直接遍历字段本身，会在遍历途中被置空而炸掉
        List<FGameTag> Payload = SerializedTags;
        //SerializedTags = null;

        if (Payload != null)
        {
            for (int i = 0; i < Payload.Count; i++)
            {
                FGameTag Tag = Payload[i];

                // 结构体元素可能没收到反序列化回调，这里显式补一次
                if (!Tag.IsValid())
                {
                    Tag.OnAfterDeserialize();
                    Payload[i] = Tag;
                }

                if (Tag.IsValid()) AddTag(Tag);
            }
        }
    }



    /* ---- 增删查 ---- */
    public void Clear()
    {
        EnsureIndex();

        ExactTag.Clear();
        ImplicitTag.Clear();
    }


    public void AddTag(FGameTag Tag)
    {
        EnsureIndex();

        GameTagNode Node = Tag.Node;
        if (Node == null) return;

        // 已拥有时不能再累加父项计数
        if (!ExactTag.Add(Tag)) return;

        GameTagNode CurrentNode = Node;
        while (CurrentNode.InParent != null)
        {
            ImplicitTag.TryGetValue(CurrentNode, out int Count);
            ImplicitTag[CurrentNode] = Count + 1;
            CurrentNode = CurrentNode.InParent;
        }
    }

    public void RemoveTag(FGameTag Tag)
    {
        EnsureIndex();

        GameTagNode Node = Tag.Node;
        if (Node == null) return;
        
        if (!ExactTag.Remove(Tag)) return;
        
        GameTagNode CurrentNode = Node;
        while (CurrentNode.InParent != null)
        {
            if (ImplicitTag.TryGetValue(CurrentNode, out int Count))
            {
                if (Count - 1 <= 0) ImplicitTag.Remove(CurrentNode);
                else ImplicitTag[CurrentNode] = Count - 1;
            }

            CurrentNode = CurrentNode.InParent;
        }
    }


    // 包含匹配：拥有子标签时，父标签同样为真(标签继承)
    public bool HasTag(FGameTag Tag)
    {
        EnsureIndex();
        
        GameTagNode Node = Tag.Node;
        if (Node == null) return false;
        return ImplicitTag.ContainsKey(Node);
    }


    // 精确匹配：只考虑是否拥有该标签本身，忽略父级
    public bool HasTagExact(FGameTag Tag)
    {
        EnsureIndex();
        
        if (Tag.Node == null) return false;
        return ExactTag.Contains(Tag);
    }




    public bool HasAnyTag(List<FGameTag> Tags)
    {
        if (Tags == null) return false;
        
        foreach (FGameTag Tag in Tags)
        {
            if (HasTag(Tag)) return true;
        }
        
        return false;
    }

    public bool HasAllTags(List<FGameTag> Tags)
    {
        if (Tags == null) return false;
        
        foreach (FGameTag Tag in Tags)
        {
            if (!HasTag(Tag)) return false;
        }
        
        return true;
    }

    public bool HasAnyTagExact(List<FGameTag> Tags)
    {
        if (Tags == null) return false;
        
        foreach (FGameTag Tag in Tags)
        {
            if (HasTagExact(Tag)) return true;
        }
        
        return false;
    }
    
    public bool HasAllTagsExact(List<FGameTag> Tags)
    {
        if (Tags == null) return false;
        
        foreach (FGameTag Tag in Tags)
        {
            if (!HasTagExact(Tag)) return false;
        }
        
        return true;
    }
    
    
    
    
    /// 全部匹配(包含继承)：自身拥有 Container 中的每一个 Tag；空容器视为满足
    public bool MatchAllContainer(GameTagContainer Container)
    {
        EnsureIndex();
        
        if (Container == null) return true;
        
        Container.EnsureIndex();
        if (Container.ExactTag.Count == 0) return true;
        
        foreach (FGameTag Tag in Container.ExactTag)
        {
            if (!HasTag(Tag)) return false;
        }
        
        return true;
    }
    
    
    /// 任意匹配(包含继承)：自身至少拥有 Container 中的某一个 Tag
    public bool MatchAnyContainer(GameTagContainer Container)
    {
        EnsureIndex();
        
        if (Container == null) return false;
        
        Container.EnsureIndex();
        if (Container.ExactTag.Count == 0) return false;
        
        foreach (FGameTag Tag in Container.ExactTag)
        {
            if (HasTag(Tag)) return true;
        }
        
        return false;
    }
    
    
    
    // 将 Container 的 Tag 全部加入
    public void AppendTags(GameTagContainer Other)
    {
        if (Other == null || ReferenceEquals(Other, this)) return;
        
        Other.EnsureIndex();
        
        foreach (FGameTag Tag in Other.ExactTag)
        {
            AddTag(Tag);
        }
    }
    
    // 将 Container 的 Tag 全部移除
    public void RemoveTags(GameTagContainer Other)
    {
        if (Other == null || ReferenceEquals(Other, this)) return;
        
        Other.EnsureIndex();
        
        foreach (FGameTag Tag in Other.ExactTag)
        {
            RemoveTag(Tag);
        }
    }
    
    public IEnumerator<FGameTag> GetEnumerator()
    {
        EnsureIndex();
        return ExactTag.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}