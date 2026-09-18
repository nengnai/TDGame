using UnityEngine;
using System.Collections.Generic;
using System;


public struct FAbilityHandle // : IEquatable<FAbilityHandle> 也许以后需要比较?
{
    private readonly uint HandleID;

    public FAbilityHandle(uint ID)
    {
        HandleID = ID;
    }

    public static FAbilityHandle Create(ref uint _abilityID)
    {
        if (_abilityID == 0) { _abilityID = 1; } // 0 是无效值,永不发出

        return new FAbilityHandle(_abilityID++);
    }

    public bool IsValid => HandleID != 0;
    
    public override int GetHashCode() => (int)HandleID;

    public override string ToString() => IsValid ? HandleID.ToString() : "Invalid";

    public static readonly FAbilityHandle Invalid = default;
}


public partial class AbilitySystemComponent : MonoBehaviour
{
    private readonly Dictionary<FAbilityHandle, GameAbilitySpec> HandleToSpec = new();
    
    private readonly List<GameAbilitySpec> EndBuffer = new();

    private bool bIsAbilityActivating; // 重入保护

    private uint AbilityID;


    /* 查询 */
    public bool TryGetSpec(FAbilityHandle Handle, out GameAbilitySpec Spec)
    {
        return HandleToSpec.TryGetValue(Handle, out Spec);
    }

    public bool IsAbilityActive(FAbilityHandle Handle)
    {
        return HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec) && Spec.IsActive;
    }

    public GameAbility GetLiveAbility(FAbilityHandle Handle)
    {
        return HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec) ? Spec.Instance : null;
    }
    
    
    public void GetActiveSpecs(List<GameAbilitySpec> Out)
    {
        if (Out == null) return;

        Out.Clear();
        foreach (GameAbilitySpec Spec in HandleToSpec.Values)
        {
            if (Spec.IsActive) Out.Add(Spec);
        }
    }
    
    
    
    public FAbilityHandle AddAbility(GameAbility Config)
    {
        if (Config == null) return FAbilityHandle.Invalid;

        FAbilityHandle Handle = FAbilityHandle.Create(ref AbilityID);
        GameAbilitySpec Spec = new(Config, Handle);
        HandleToSpec.Add(Handle, Spec);

        switch (Config.Policy)
        {
            case GameAbility.InstantiationPolicy.Static:
            {
                Spec.Instance = Config; // 资产本身就是实例:授予时不写任何字段
                break;
            }
            case GameAbility.InstantiationPolicy.OnGranted:
            {
                Spec.Instance = Instantiate(Config);
                Spec.Instance.OnGranted();
                break;
            }
            case GameAbility.InstantiationPolicy.OnActivate:
            {
                Spec.Instance = null;
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        return Handle;
    }


    public void RemoveAbility(FAbilityHandle Handle)
    {
        if (!HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec)) { return; }

        EndAbility(Handle);
        HandleToSpec.Remove(Handle);

        if (Spec.Config.Policy == GameAbility.InstantiationPolicy.OnGranted && Spec.Instance != null)
        {
            Destroy(Spec.Instance); // OnActivate 的实例已在 ShutdownAbility 里销毁
        }

        Spec.Instance = null;
    }

    
    public bool ActivateAbility(FAbilityHandle Handle)
    {
        if (bIsAbilityActivating) { return false; }

        if (!HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec)) { return false; }

        if (Spec.IsActive) { return false; } // 已激活:一次字典命中,不靠标签判断

        // 判断是否可以激活
        {
            GameAbility TempInstance = Spec.Instance ?? Spec.Config;
            BindContext(Spec, TempInstance);
            bool bCanActivate = TempInstance.CanActivate();
            UnbindContext(Spec, TempInstance);
            if (!bCanActivate) { return false; }
        }

        // 被正在激活技能的 BlockingTags 阻止?
        foreach (GameAbilitySpec CheckSpec in HandleToSpec.Values)
        {
            if (!CheckSpec.IsActive || CheckSpec.Instance == null) { continue; }
            if (CheckSpec.Instance.BlockingTags.IsEmpty) { continue; }
            if (CheckSpec.Instance.BlockingTags.MatchAnyContainer(Spec.Config.OwnTags)) { return false; }
        }

        GameAbility Live = Spec.Instance;
        if (Spec.Config.Policy == GameAbility.InstantiationPolicy.OnActivate)
        {
            Live = Instantiate(Spec.Config);
            Spec.Instance = Live;
            Live.OnGranted();
        }
        if (Live == null) { return false; }

        bIsAbilityActivating = true;
        try
        {
            Spec.IsActive = true;

            // 打断: CancelTags 命中其它激活中技能的 OwnTags (不打断自己)
            EndBuffer.Clear();
            foreach (GameAbilitySpec ForSpec in HandleToSpec.Values)
            {
                if (ForSpec == Spec) { continue; }
                if (!ForSpec.IsActive || ForSpec.Instance == null) { continue; }
                if (ForSpec.Instance.OwnTags.MatchAnyContainer(Live.CancelTags)) { EndBuffer.Add(ForSpec); }
            }
            foreach (GameAbilitySpec EndSpec in EndBuffer)
            {
                EndAbility(EndSpec.Handle);
            }

            Tags.AppendTags(Live.OwnTags); // 挂自己的标签

            BindContext(Spec, Live);
            Live.Activate();
            UnbindContext(Spec, Live);
        }
        finally
        {
            bIsAbilityActivating = false;
        }

        return true;
    }


    public bool EndAbility(FAbilityHandle Handle)
    {
        if (!HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec)) { return false; }
        if (!Spec.IsActive || Spec.Instance == null) { return false; }
        
        BindContext(Spec, Spec.Instance);
        Spec.Instance.EndAbility();
        UnbindContext(Spec, Spec.Instance);

        return true;
    }


    public void CancelAbility(FAbilityHandle Handle)
    {
        if (!HandleToSpec.TryGetValue(Handle, out GameAbilitySpec Spec)) { return; }
        if (!Spec.IsActive || Spec.Instance == null) { return; }

        BindContext(Spec, Spec.Instance);
        Spec.Instance.Cancel();
        UnbindContext(Spec, Spec.Instance);

        if (Spec.IsActive) { EndAbility(Handle); }
    }

    

    // GA 回调入口
    public void ShutdownAbility(GameAbility Live)
    {
        if (Live == null || !HandleToSpec.TryGetValue(Live.Handle, out GameAbilitySpec Spec)) { return; }

        if (!Spec.IsActive) { return; }

        Tags.RemoveTags(Live.OwnTags);
        Spec.IsActive = false;

        if (Spec.Config.Policy == GameAbility.InstantiationPolicy.OnActivate)
        {
            Destroy(Live);
            Spec.Instance = null;
        }
    }


    // 只有 CanActivate / Activate / EndAbility 调用前绑上下文(GA 内部直接读 Owner/Handle)
    private void BindContext(GameAbilitySpec Spec, GameAbility Target)
    {
        Target.Owner = this;
        Target.Handle = Spec.Handle;
    }

    private void UnbindContext(GameAbilitySpec Spec, GameAbility Target)
    {
        if (Spec.Config.Policy != GameAbility.InstantiationPolicy.Static) { return; }

        Target.Owner = null;
        Target.Handle = FAbilityHandle.Invalid;
    }
}
