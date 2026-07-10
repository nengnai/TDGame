using UnityEngine;
[CreateAssetMenu(fileName = "NewUnit", menuName = "建造系统/新建学生档案")]
public class UnitData : ScriptableObject 
{
    public string studentName;   
    public float trainingTime;   
    public GameObject student; 
    public int currentNumber;
    public bool isMaxed = false;

    
    





}