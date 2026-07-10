using System;
using System.Collections.Generic;
using UnityEngine;

public class FTimerData
{
    public int Index;
    public float CostTime;
    public bool IsLoop;
    public bool IsUnscaledTime;
    public Action CallBack; 
}


public struct FTimer
{
    public double FadedTimer;
    public FTimerHandle TimerHandle;

}

public struct FTimerHandle
{
    public uint TimerHandle;

}

public class FTimerMain : MonoBehaviour
{
    private List<FTimer> TimerList = new List<FTimer>();
    private Dictionary<FTimerHandle, FTimerData> TimerDict = new Dictionary<FTimerHandle, FTimerData>();
    protected uint ID = 0;


    void Update()
    {
        if(TimerList.Count == 0) return;
        if(TimerList[0].FadedTimer <= Time.time)
        {
            FTimerHandle Handle = TimerList[0].TimerHandle;
            FTimerData Data = TimerDict[Handle];

            Data.CallBack?.Invoke();
            if (Data.IsLoop)
            {
                RemoveTimer(Handle);
                AddTimer(Data.CostTime, Data.IsLoop, Data.IsUnscaledTime, Data.CallBack);
            }
            else
            {
                RemoveTimer(Handle);
            }
        }
    }







    public bool AddTimer(float CostTime, bool IsLoop, bool IsUnscaledTime, Action CallBack)
    {

        ID++;
        FTimerHandle TimerHandle = new FTimerHandle();
        TimerHandle.TimerHandle = ID;
        FTimer Timer = new FTimer();
        Timer.FadedTimer = Time.time + CostTime;
        Timer.TimerHandle = TimerHandle;
        FTimerData TimerData = new FTimerData();
        TimerData.Index = TimerList.Count;
        TimerData.CostTime = CostTime;
        TimerData.IsLoop = IsLoop;
        TimerData.IsUnscaledTime = IsUnscaledTime;
        TimerData.CallBack = CallBack;

        TimerList.Add(Timer);
        TimerDict.Add(TimerHandle, TimerData);

        int IndexSlot = TimerList.Count - 1;

        while (IndexSlot > 0 && TimerList[IndexSlot].FadedTimer < TimerList[IndexSlot - 1].FadedTimer)
        {
            SwapIndex(IndexSlot, IndexSlot - 1);
            IndexSlot--;
        }

        return true;
    }


    public void RemoveTimer(FTimerHandle Handle)
    {
        if(!TimerDict.ContainsKey(Handle)) return;
        FTimerData Data = TimerDict[Handle];
        int Index = Data.Index;

        TimerList.RemoveAt(Index);
        for(int i = Index; i < TimerList.Count; i++)
        {
            TimerDict[TimerList[i].TimerHandle].Index = i;
        }

        TimerDict.Remove(Handle);
    }







    public void SwapIndex(int Index1, int Index2)
    {
        if(Index1 == Index2) return;
        if(Index1 < 0 || Index2 < 0 || Index1 >= TimerList.Count || Index2 >= TimerList.Count) return;

        FTimer Slot = TimerList[Index1];
        TimerList[Index1] = TimerList[Index2];
        TimerList[Index2] = Slot;


        TimerDict[TimerList[Index1].TimerHandle].Index = Index1;
        TimerDict[TimerList[Index2].TimerHandle].Index = Index2;
    }


    

}