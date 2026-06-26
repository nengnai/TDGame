using UnityEngine;
using TDGameLibrary;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("范围")]
    public Transform RangeTransform;
    public Vector3 Range =  new Vector3(30.0f, 30.0f, 30.0f);
    
    public float MaxDistance = 500.0f;
    public float MinDistance = 100.0f;
    public float MaxAngle = 70.0f;
    public float MinAngle = 30.0f;
    
    [Header("速度")]
    public float MoveSpeed = 50.0f;
    public float ShiftSpeedRate = 0.1f;
    
    public float DistanceSpeed = 10.0f;
    
    [Header("控制")]
    public Transform SpringArm;
    public bool CanMove = true;
    public bool CanZoom = true;
    
    [Header("运行时")]
    protected float MoveSpeedRate = 1.0f;
    protected float TargetDistance;
    protected float CurrentDistance;
    
    
    
    void Start()
    {
        TargetDistance = MaxDistance;
        CurrentDistance = MaxDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanZoom)
        {
            TargetDistance = Mathf.Clamp(
                TargetDistance - Input.mouseScrollDelta.y * DistanceSpeed,
                MinDistance,
                MaxDistance);
            
            CurrentDistance = TDUtility.FInterpTo(
                CurrentDistance, 
                TargetDistance,
                Time.deltaTime, 
                10.0f);

            SetSpringArmData(TDUtility.Remap(
                    CurrentDistance,
                    MinDistance,
                    MaxDistance,
                    MinAngle,
                    MaxAngle
                ),
                CurrentDistance);
        }
        if (CanMove)
        {
            MoveSpeedRate = TDUtility.FInterpTo(
                MoveSpeedRate, 
                Mathf.Lerp(1.0f, ShiftSpeedRate, TDUtility.BoolToFloat(Input.GetKey(KeyCode.LeftShift))),
                Time.deltaTime, 
                10.0f);
            
            var HorizontalInput = new Vector2(
                MoveSpeed * MoveSpeedRate *(
                    TDUtility.BoolToFloat(Input.GetKey(KeyCode.D))
                    -TDUtility.BoolToFloat(Input.GetKey(KeyCode.A))
                ), 
                MoveSpeed * MoveSpeedRate *(
                    TDUtility.BoolToFloat(Input.GetKey(KeyCode.W))
                    -TDUtility.BoolToFloat(Input.GetKey(KeyCode.S))
                )
            );
            AddMoveInput(HorizontalInput);
        }
    }


    public void SetSpringArmData(float Angle, float Distance)
    {
        SpringArm.transform.localEulerAngles = new Vector3(Angle, 0.0f, 0.0f);
        SpringArm.transform.localPosition = SpringArm.transform.localRotation *
                                            Vector3.forward *
                                            (-Distance);
    }
    
    public void AddMoveInput(Vector2 Input)
    {
        // 注意：Unity是Y轴朝上
        transform.position += transform.TransformDirection(new Vector3(Input.x, 0.0f, Input.y)) * Time.deltaTime;
    }
    
    
    
    /* 工具函数 */
    protected Vector3 ClampInRange(ref Vector3 WorldPosition)
    {
        // 世界空间 到 局部空间
        Vector3 LocalPos = RangeTransform.InverseTransformPoint(WorldPosition);
        
        Vector3 ClampedLocalPos = new Vector3(
            Mathf.Clamp(LocalPos.x, -Range.x, Range.x),
            Mathf.Clamp(LocalPos.y, -Range.y, Range.y),
            Mathf.Clamp(LocalPos.z, -Range.z, Range.z)
        );

        // 局部空间 到 世界空间 
        return RangeTransform.TransformPoint(ClampedLocalPos);
    }

}
