using UnityEngine;

public class KillTheThings : MonoBehaviour
{
    public new GameObject gameObject;
    public void Krill()
    {
        Destroy(gameObject);
    }
}
