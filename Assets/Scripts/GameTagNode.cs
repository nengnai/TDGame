using System.Collections.Generic;
using UnityEngine;

public class GameTagNode
{
    public int ID;
    public FName FullName;
    public FName Name;
    public GameTagNode Parent;
    public List<GameTagNode> Child = new List<GameTagNode>();
    

    public GameTagNode(FName fullName, FName name, GameTagNode parent)
    {
        ID = GameTagManager.NextID;
        GameTagManager.NextID++;
        FullName = fullName;
        Name = name;
        Parent = parent;
    }

}


public static class GameTagManager
{
    public static GameTagNode RootNode;
    public static Dictionary<string, GameTagNode> AllNodes;
    public static int NextID;




    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        NextID = 1;

        RootNode = new GameTagNode(new FName("Root"), new FName("Root"), null);

        AllNodes = new Dictionary<string, GameTagNode>();
    }

}





public static class TDTools
{
    public static GameTagNode GetNode (string Fullname)
    {


        
        string [] Parts = Fullname.Split('.');
        string CurrentPath = null;
        GameTagNode CurrentNode = null;
        GameTagNode CurrentParent = GameTagManager.RootNode;


        for (int i = 0; i < Parts.Length; i++)
        {
            if (string.IsNullOrEmpty(CurrentPath))
            {
                CurrentPath = Parts[i];
            }
            else
            {
                CurrentPath = CurrentPath + "." + Parts[i];
            }

            if(GameTagManager.AllNodes.ContainsKey(CurrentPath))
            {
                CurrentNode = GameTagManager.AllNodes[CurrentPath];
            }
            else
            {
                GameTagNode NewNode = new GameTagNode(new FName(CurrentPath), new FName (Parts[i]), CurrentParent);
                GameTagManager.AllNodes.Add(CurrentPath, NewNode);
                CurrentParent.Child.Add(NewNode);
                CurrentNode = NewNode;
            }

            CurrentParent = CurrentNode;


        }

        return CurrentNode;
    }
}


public struct GameTag
{
    public GameTagNode Node;

    public static bool NodeMatch(GameTag A, GameTag B)
    {
        return A.Node.ID == B.Node.ID;
    }

    public static GameTag CreateTag(string Path)
    {
        GameTag Tag;
        Tag.Node = TDTools.GetNode(Path);
        return Tag;
    }
}