using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerCancel : MonoBehaviour
{

    public TowerPre1 塔投影管理器;
    public GameObject 塔预制体;
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(触发器变化);
    }

    void 触发器变化(bool isOn)
    {
        塔投影管理器.选中塔(toggle, 塔预制体);
    }

}
