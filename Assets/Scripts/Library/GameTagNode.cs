using System;
using System.Collections.Generic;
using TDGameLibrary;

public class GameTagNode
{
    public FName InFullName;
    public FName InName;
    public GameTagNode InParent;
    public readonly List<GameTagNode> Child = new();
    
    public GameTagNode(FName InFullName, FName InName, GameTagNode InParent)
    {
        this.InFullName = InFullName;
        this.InName = InName;
        this.InParent = InParent;
    }
}


public class GameTagManager : GameInstanceSubsystem<GameTagManager>
{
    public GameTagNode RootNode;
    private Dictionary<string, GameTagNode> AllNodes = new();
    
    protected override void Awake()
    {
        base.Awake();
        RootNode = new GameTagNode(new FName("Root"), new FName("Root"), null);
    }
    
    public GameTagNode GetNode (string Fullname)
    {
        string [] Parts = Fullname.Split('.');
        string CurrentPath = null;
        GameTagNode CurrentNode = null;
        GameTagNode CurrentParent = RootNode;
        
        foreach (var T in Parts)
        {
            if (string.IsNullOrEmpty(CurrentPath))
            {
                CurrentPath = T;
            }
            else
            {
                CurrentPath = CurrentPath + "." + T;
            }

            if(AllNodes.TryGetValue(CurrentPath, out GameTagNode Node))
            {
                CurrentNode = Node;
            }
            else
            {
                GameTagNode NewNode = new(new FName(CurrentPath), new FName (T), CurrentParent);
                AllNodes.Add(CurrentPath, NewNode);
                CurrentParent.Child.Add(NewNode);
                CurrentNode = NewNode;
            }
            CurrentParent = CurrentNode;
        }

        return CurrentNode;
    }

}



public struct FGameTag : IEquatable<FGameTag> //, ISerializationCallbackReceiver
{
    public GameTagNode Node
    {
        get;
        private set;
    }
    
    
    public FGameTag(string Value)
    {
        Node = GameTagManager.GetSubsystem().GetNode(Value);
    }
    
    /* 序列化 */
    //@todo:添加序列化支持
    /*public void OnBeforeSerialize()
    {
        Value = NameEntry?.BaseString;
    }
    public void OnAfterDeserialize()
    {
        Value ??= "";
        Init(Value);
        Value = null;
    }*/
    
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
        //@todo:这块没仔细看，回头再仔细看下？？
        return (Node != null ? Node.GetHashCode() : 0); 
    }

    public override string ToString()
    {
        return Node.InFullName.ToString();
    }

    public bool IsValid()
    {
        return Node != null;
    }
}




public class GameTagContainer
{
    private HashSet<GameTagNode> ExactTag = new();
    private Dictionary<GameTagNode, int> ImplicitTag = new();
    public void AddTag(FGameTag Tag)
    {
        GameTagNode Node = Tag.Node;
        ExactTag.Add(Node);

        GameTagNode CurrentNode = Node;
        while(CurrentNode.InParent != null)
        {
            if(ImplicitTag.TryGetValue(CurrentNode, out int Count))
            {
                ImplicitTag[CurrentNode]++;
            }
            else
            {
                ImplicitTag.Add(CurrentNode, 1);
            }

            CurrentNode = CurrentNode.InParent;
        }
    }

    public void RemoveTag(FGameTag Tag)
    {
        GameTagNode Node = Tag.Node;
        ExactTag.Remove(Node);

        GameTagNode CurrentNode = Node;
        while(CurrentNode.InParent != null)
        {
            if(ImplicitTag.TryGetValue(CurrentNode, out int Count))
            {
                if(Count - 1 <= 0)
                {
                    ImplicitTag.Remove(CurrentNode);
                }
                else
                {
                    ImplicitTag[CurrentNode]--;
                }
            }

            CurrentNode = CurrentNode.InParent;
        }
    }


    public bool HasTag(FGameTag Tag)
    {
        return ImplicitTag.ContainsKey(Tag.Node);
    }


    public bool HasTagExact(FGameTag Tag)
    {
        return ExactTag.Contains(Tag.Node);
    }







    public bool HasAnyTag(List<FGameTag> Tags)
    {
        foreach(FGameTag tag in Tags)
        {
            if(HasTag(tag))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAllTags(List<FGameTag> Tags)
    {
        foreach(FGameTag tag in Tags)
        {
            if (!HasTag(tag))
            {
                return false;
            }
        }

        return true;
    }

    public bool HasAnyTagExact(List<FGameTag> Tags)
    {
        foreach(FGameTag tag in Tags)
        {
            if (HasTagExact(tag))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAllTagsExact(List<FGameTag> Tags)
    {
        foreach(FGameTag tag in Tags)
        {
            if (!HasTagExact(tag))
            {
                return false;
            }
        }

        return true;
    }


}