using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerCancel : MonoBehaviour
{
    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (toggle.isOn)
            {
                toggle.isOn = false;

            }
        }
    }

}
