using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraControll : MonoBehaviour
{
    public float PositionX = 0;
    public float PositionZ = 0;
    public int RotationY = 0;
    public float x = 0;
    public float y = 0;
    public float moveSpeed = 40f;
    public float duration = 0.3f;
                        
    
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    private bool canRotate = true;

    private float slowMode = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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

        Vector3 newPosition = transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            newPosition += transform.forward * moveSpeed * slowMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newPosition -= transform.forward * moveSpeed * slowMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newPosition -= transform.right * moveSpeed * slowMode * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newPosition += transform.right * moveSpeed * slowMode * Time.deltaTime;
        }

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;



        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(SmoothRotate(-45f));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SmoothRotate(45f));
        }
    
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
        //QE转向平滑动画
    }
}

    

