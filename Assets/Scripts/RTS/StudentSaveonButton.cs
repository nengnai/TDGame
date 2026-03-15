
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StudentSaveonButton : MonoBehaviour, IPointerClickHandler
{
    [Header("单位信息")]
    public UnitData unitData;
    public StudentTraning studentTraining;
    public Image ProgressUI;
    public Button button;
    public int maxNumber;
    int currentNumber;

    [Header("外部别动")]
    public bool isTraining = false;
    public bool isPaused = false;
    public bool isCancelled = false;

    void Start()
    {
        currentNumber = unitData.currentNumber;
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(unitData.isMaxed) return;
            else if (isPaused)
            {
                isPaused = false;
            }
            else if (!isTraining)
            {
                isTraining = true;
                isCancelled = false;
                GameObject spawnedStudent = await studentTraining.trainingStudent(unitData, ProgressUI, this);

                if (spawnedStudent != null)
                {
                    currentNumber ++;

                    var theStudent = spawnedStudent.GetComponentInChildren<CharacterState>();
                    if(theStudent != null)
                    {
                        theStudent.Button = this;
                    }


                }
                isTraining = false;
            }
        }

        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (isPaused)
            {
                isCancelled = true;
                isPaused = false;
            }
            else if (isTraining)
            {
                isPaused = true;
            }
        }
    }
















    public void studentGotkrilled()
    {
        currentNumber --;
    }



    void Update()
    {
        unitData.isMaxed = currentNumber < maxNumber ? false : true;
    }








}
