using UnityEngine;
using UnityEngine.InputSystem;



public enum KeyState
{
    Pressed,             //按下
    Released,          //松开
    Held               //按住

}

public struct InputData
{
    public InputControl Key;
    public KeyState State;
}



public delegate bool InputHandle(InputData Data);


public class InputHandleContext
{
    public int Priority;
    public InputHandle Handle;
    
}





public class InputSubsystem
{
    private static InputSubsystem _InputSystem;

    public static InputSubsystem InputSystem
    {
        get
        {
            if(_InputSystem == null) _InputSystem = new InputSubsystem();
            return _InputSystem;
        }
    }

    private InputSubsystem() {}


}


