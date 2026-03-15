using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimControl : MonoBehaviour
{
    
    



    Animator animator;
    NavMeshAgent agent;



    void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }







    void Update()
    {
        float CurrentMovespeed = agent.velocity.magnitude;
        animator.SetBool("isWalking", CurrentMovespeed> 0.1f);

        float Multiple = CurrentMovespeed / agent.speed;

        
        animator.SetFloat("RunAnimSpeed", Multiple);
    }


}
