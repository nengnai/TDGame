
using TDGameLibrary;
using UnityEngine.InputSystem;

public class InputSubsystem : WorldSubsystem<InputSubsystem>
{
    public enum InputState : uint
    {
        Ingame,
        UI,
        Unknow= 255
    }
    

    private InputActionAsset IngameInputGroup;
    private InputActionAsset UIInputGroup;

    private InputState CurrentInputState;


    #region 公开函数
    public void ChangeCurrentInputState(InputState NewState)
    {
        if(NewState == CurrentInputState) return;

        var LastState = CurrentInputState;
        CurrentInputState = NewState;
        UpdateInputAsset(LastState, NewState);

    }


    public void SwitchIngameAsset(InputActionAsset NewAsset)
    {
        if(CurrentInputState == InputState.Ingame)
        {
            IngameInputGroup.Disable();
            NewAsset.Enable();
        } 
        
        IngameInputGroup = NewAsset;

    }


    #endregion

    #region 工具

    private void UpdateInputAsset(InputState LastState, InputState NewState)
    {
        switch (LastState)
        {
            case InputState.Ingame:
            IngameInputGroup.Disable();
            break;

            case InputState.UI:
            UIInputGroup.Disable();
            break;

            case InputState.Unknow:
            IngameInputGroup.Disable();
            UIInputGroup.Disable();
            break;
        }

        switch (NewState)
        {
            case InputState.Ingame:
            IngameInputGroup.Enable();
            break;

            case InputState.UI:
            UIInputGroup.Enable();
            break;

            case InputState.Unknow:
            break;
        }
    }

    #endregion
}