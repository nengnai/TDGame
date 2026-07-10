using System;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace TestScript
{
    public class TimerTest : MonoBehaviour
    {
        protected void Awake()
        {
            Debug.Log("开始定时");
            FTimerHandle TimerHandle;
            TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                0.8f,
                true,
                false,
                () =>
                {
                    Debug.Log("循环0.8秒,当前时间:" + Time.time);
                }
            );
            
            TimerSubsystem.GetSubsystem().AddTimer(
                5.0f,
                false,
                false,
                () =>
                {
                    Debug.Log("5秒到了,移除");
                    TimerSubsystem.GetSubsystem().RemoveTimer(TimerHandle);
                }
            );
            
        }

    }
}