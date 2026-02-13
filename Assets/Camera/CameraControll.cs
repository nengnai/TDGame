using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraControll : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed;
    public float duration;
                        
    
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    private bool canRotate = true;

    private float slowMode = 1;

    [Header("远近拉伸")]
    public Transform camera1;
    public float 默认高度;
    public float 最低高度;
    public float 最高高度;
    public float 拉伸速度;
    public float 最小角度;
    public float 最大角度;
    public float 动画时间;
    private float 目标高度;
    private float 当前高度角度;


    void Start()
    {

        float defaultHeight = 500f;

        camera1.localPosition = new Vector3(camera1.localPosition.x, defaultHeight, camera1.localPosition.z
    );

        目标高度 = defaultHeight;

        当前高度角度 = 0f;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            slowMode = 0.4f;
        }
        else
        {
            slowMode = 1f;
        }
        //按shift减摄像头移动速度

        Vector3 当前位置 = transform.position;
        Vector3 方向 = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            方向 += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            方向 += transform.forward * -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            方向 += transform.right * -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            方向 += transform.right;
        }


        if(方向 != Vector3.zero)
        方向.Normalize();

        Vector3 movement = 方向 * moveSpeed * slowMode * Time.deltaTime;

        当前位置 += movement;

        //向量叠加实现摄像头移动

        当前位置.x = Mathf.Clamp(当前位置.x, minX, maxX);
        当前位置.z = Mathf.Clamp(当前位置.z, minZ, maxZ);
        //限制摄像头移动区域

        transform.position = 当前位置;
        //更新位置来达到摄像头移动的表现



        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(SmoothRotate(-45f));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SmoothRotate(45f));
        }

        //摄像头旋转按键

        HandleZoom();
        
    
    }

    IEnumerator SmoothRotate(float angle)
    {
        if (!canRotate) yield break;
                
        canRotate = false;
        float elapsed = 0f;
        Vector3 startRot = transform.eulerAngles;
        Vector3 endRot = startRot + new Vector3(0, angle, 0);
                
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = t * t * (3f - 2f * t); 
            transform.eulerAngles = Vector3.Lerp(startRot, endRot, smoothT);
            yield return null;
        }
                
        canRotate = true;
    }
    //QE转向平滑动画，看不懂先别管能跑就行

    void HandleZoom()
    {
        if(Time.frameCount < 60) return;
        float scroll = Input.mouseScrollDelta.y;

        if (scroll != 0)
        {
            目标高度 -= scroll * 拉伸速度 * Time.deltaTime;
            目标高度 = Mathf.Clamp(目标高度, 最低高度, 最高高度);
        }

        float currentHeight = camera1.localPosition.y;

        float smoothHeight = Mathf.SmoothDamp(currentHeight, 目标高度, ref 当前高度角度, 动画时间);

        camera1.localPosition = new Vector3(camera1.localPosition.x, smoothHeight, camera1.localPosition.z);

        float percent = (smoothHeight - 最低高度)/(最高高度 - 最低高度);
        float targetAngle = Mathf.Lerp(最小角度, 最大角度, percent);

        camera1.localRotation = Quaternion.Euler(targetAngle, 0f, 0f);

    }
}

    

