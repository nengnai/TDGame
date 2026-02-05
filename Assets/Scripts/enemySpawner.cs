using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public static enemySpawner Instance{get;private set;}
    public Transform startPoint;

    public List<Wave> waveList;

    private int enemyCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
         foreach(Wave wave in waveList)
        {
            for(int i = 0; i < wave.count; i++)
            {
                GameObject.Instantiate(wave.enemyPrefab,startPoint.position,Quaternion.identity);
                enemyCount++;
                if (i != wave.count - 1)
                {
                    yield return new WaitForSeconds(wave.rate); 
                }
                
            }
            //生成完一个波次

            while (enemyCount > 0)
            {
                yield return 0;
            }

            

        }
        yield return null;
    }

    public void DecreateEnemyCount()
    {
        if(enemyCount > 0)
        {
            enemyCount--;
        }
    }
}
