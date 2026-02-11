using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraControll : MonoBehaviour
{
    
    public float moveSpeed = 40f;
    public float duration = 0.3f;
                        
    
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    private bool canRotate = true;

    private float slowMode = 1;


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
    
        System.Collections.IEnumerator SmoothRotate(float angle)
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
    }
}

    

