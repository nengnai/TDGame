// 

using System;
using System.Collections.Generic;
using TDGameLibrary;
using UnityEngine;


public struct FTimerHandle
{
    private readonly uint _TimerHandle;
    
    public FTimerHandle(uint InTimerHandle)
    {
        _TimerHandle = InTimerHandle;
    }
    
    public override int GetHashCode()
    {
        return (int)_TimerHandle;
    }
}

public class TimerSubsystem : WorldSubsystem<TimerSubsystem>
{
    private class FTimerData
    {
        public int Index;
        public readonly float CostTime;
        public readonly bool IsLoop;
        public readonly bool IsUnscaledTime;
        public readonly Action CallBack;

        public FTimerData(
            int InIndex,
            float InCostTime,
            bool InIsLoop,
            bool InIsUnscaledTime,
            Action InCallBack)
        {
            Index = InIndex;
            CostTime = InCostTime;
            IsLoop = InIsLoop;
            IsUnscaledTime = InIsUnscaledTime;
            CallBack = InCallBack;
        }
    }

    private class FTimer
    {
        public double FadedTimer;
        public readonly FTimerHandle TimerHandle;

        public FTimer(float InFadedTimer, FTimerHandle InTimerHandle)
        {
            FadedTimer = InFadedTimer;
            TimerHandle = InTimerHandle;
        }
    }
    
    
    private List<FTimer> TimerList = new List<FTimer>();
    private Dictionary<FTimerHandle, FTimerData> TimerDict = new Dictionary<FTimerHandle, FTimerData>();
    private uint ID = 0;
    
    
    void Update()
    {
        while (TimerList.Count > 0)
        {
            FTimer Timer = TimerList[^1];

            if (Timer.FadedTimer > Time.time)
                break;

            FTimerData Data = TimerDict[Timer.TimerHandle];

            if (Data.IsLoop)
            {
                Timer.FadedTimer += Data.CostTime;
                FastSortEnd(TimerList);
            }
            else
            {
                RemoveTimer(Timer.TimerHandle);
            }

            Data.CallBack?.Invoke();
        }
    }
    
    
    public FTimerHandle AddTimer(float CostTime, bool IsLoop, bool IsUnscaledTime, Action CallBack)
    {
        // 创建新句柄
        FTimerHandle TimerHandle = new FTimerHandle(ID++);
        
        // 创建计时用元素
        FTimer Timer = new FTimer(
            Time.time + CostTime,
            TimerHandle);
        
        // 创建数据保存元素
        FTimerData TimerData = new FTimerData(
            TimerList.Count,
            CostTime, 
            IsLoop,
            IsUnscaledTime,
            CallBack);
        
        // 添加到素组和字典
        TimerList.Add(Timer);
        TimerDict.Add(TimerHandle, TimerData);
        
        // 进行排序归为
        FastSortEnd(TimerList);

        return TimerHandle;
    }
    
    
    public void RemoveTimer(FTimerHandle Handle)
    {
        if (!TimerDict.TryGetValue(Handle, out FTimerData Data))
        {
            return;
        }

        int Index = Data.Index;

        TimerList.RemoveAt(Index);
        for(int i = Index; i < TimerList.Count; i++)
        {
            TimerDict[TimerList[i].TimerHandle].Index = i;
        }

        TimerDict.Remove(Handle);
    }
    
    
    /* 工具函数 */
    
    private void SwapIndex(int Index1, int Index2)
    {
        if(Index1 == Index2) return;
        if(Index1 < 0 || Index2 < 0 || Index1 >= TimerList.Count || Index2 >= TimerList.Count) return;
        
        // 通过析构交换
        (TimerList[Index1], TimerList[Index2]) = (TimerList[Index2], TimerList[Index1]);
        
        TimerDict[TimerList[Index1].TimerHandle].Index = Index1;
        TimerDict[TimerList[Index2].TimerHandle].Index = Index2;
    }
    
    
    /*
     * 对定时器列表进行降序插入排序
     * 排序规则：按 FadedTimer 从大到小排列
     * 即过期时间最大的在列表头部，最小的在尾部
     *
     * @param SortList 需要进行排序的定时器列表
     *
     * 注意：
     *   本函数只能对最后一个数进行排序，其他部分需要有序
     */
    private void FastSortEnd(List<FTimer> SortList)
    {
        int Index = TimerList.Count - 1;
        
        while (Index > 0 &&
               TimerList[Index].FadedTimer > TimerList[Index - 1].FadedTimer)
        {
            SwapIndex(Index, Index - 1);
            Index--;
        }
    }
    
}