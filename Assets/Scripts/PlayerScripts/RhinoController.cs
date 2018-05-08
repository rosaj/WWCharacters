using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class RhinoController : MonoBehaviour {


   
   
    public float AllowedDistance = 2;


    protected int speedFloat;                     

    private Animator animator;
    private GameObject Target;

    private NavMeshAgent navMeshAgent;
	void Start () {
        animator = GetComponent<Animator>();
        speedFloat = Animator.StringToHash("Speed");
        navMeshAgent = GetComponent<NavMeshAgent>();

        Target = PlayerManager.instance.player;

        StartCoroutine(trackPlayer());
    }
	
    IEnumerator trackPlayer()
    {
        while (true)
        {
            var distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            float speed = 0;
            if (distanceToTarget > AllowedDistance)
            {
             
                navMeshAgent.SetDestination(Target.transform.position);

                speed = distanceToTarget / AllowedDistance *2;

                navMeshAgent.speed = speed;
                animator.SetFloat(speedFloat, speed/2);

            }
            else
            {
                
                animator.SetFloat(speedFloat, 0);
            }
            yield return new WaitForSeconds(.5f);
        }
    }

	
}
