using Unity.VisualScripting;
using UnityEngine;

public class Selected : MonoBehaviour
{

    public GameObject selectionCirclePrefab;
    public LayerMask clickableLayer;
    private GameObject selectionCircle;
    private bool isSelected = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.transform == transform)
                {
                    ToggleSelection();
                }
                else
                {
                    if(isSelected && ((1 << hit.collider.gameObject.layer) & clickableLayer) != 0)
                    {
                        MoveTo(hit.point);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (isSelected)
            {
                ToggleSelection(false);
            }
        }


        if(isSelected && selectionCircle != null)
        {
            Collider col = GetComponent<Collider>();

            if(col != null)
            {
                Vector3 bottom = col.bounds.min;
                bottom.y += 0.01f;
                selectionCircle.transform.position = new Vector3(transform.position.x, bottom.y, transform.position.z);
            }
            

        }



    }

    void MoveTo(Vector3 target)
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if(agent != null) agent.SetDestination(target);
    }

    void ToggleSelection(bool? state = null)
    {
        isSelected = state ?? !isSelected;

        if (isSelected)
        {
            if(selectionCircle == null)
            {
                selectionCircle = Instantiate(selectionCirclePrefab);
            }
        }
        else
        {
            if(selectionCircle != null)
            {
                Destroy(selectionCircle);
            }
        }
    }


}
