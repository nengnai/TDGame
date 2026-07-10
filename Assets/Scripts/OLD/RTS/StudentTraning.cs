
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class StudentTraning : MonoBehaviour
{
    public GameObject Spawnpoint; //出兵点 


    public async Task<GameObject> trainingStudent(UnitData data, Image ProgressUI, StudentSaveonButton button)
    {
        float time = 0f;
        ProgressUI.fillAmount = 0f;
        while (time < data.trainingTime)
        {
            if (button.isCancelled)
            {
                ProgressUI.fillAmount = 0f;
                return null;
            }
            else if (!button.isPaused)
            {
                time += Time.deltaTime;
                ProgressUI.fillAmount = time / data.trainingTime;
            }
            await Task.Yield();
        }

        GameObject newStudent = Instantiate(data.student, Spawnpoint.transform.position, Spawnpoint.transform.rotation);



        ProgressUI.fillAmount = 0f;
        return newStudent;

    }







    




}
