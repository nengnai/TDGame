// 用于事件分发与触发

using System;
using System.Collections.Generic;

namespace TDGameLibrary
{
    public class EventSystem<T>
    {
        private Dictionary<FGameTag, HashSet<Action<T>>> ActionMap = new();

        public void ListenAction(FGameTag TargetTag, Action<T> Action)
        {
            if (Action == null) return;

            if (!ActionMap.TryGetValue(TargetTag, out var Actions))
            {
                Actions = new HashSet<Action<T>>();
                ActionMap[TargetTag] = Actions;
            }

            Actions.Add(Action);
        }

        public void TriggerAction(FGameTag TargetTag, T Value)
        {
            if (!ActionMap.TryGetValue(TargetTag, out var Actions)) { return; }
            foreach (var TargetAction in Actions)
            {
                TargetAction.Invoke(Value);
            }
        }
    }
}