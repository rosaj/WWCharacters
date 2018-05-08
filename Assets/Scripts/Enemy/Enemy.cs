using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Interactable {

    public float attackSpeed = 0.2f;

    private EnemyController controller;
    private NavMeshAgent navAgent;

    private GameObject onDeathParticle;

    private bool interacting = false;
    private float attackCooldown = 0f;

    private EnemyStats enemyStats;
    private ProgressBar healthBar;
    public override void Start()
    {
        base.Start();
        controller = GetComponent<EnemyController>();
        navAgent = GetComponent<NavMeshAgent>();
        healthBar = GameManager.GetEnemyHealthBarCopy(gameObject);
        enemyStats = GetComponent<EnemyStats>();
     //   navAgent.isStopped = true;
    }
    private void Update()
    {
        if (interacting)
        {
            if (controller.CanAttack())
            {
                attackCooldown -= Time.deltaTime*10;

                if (GetPlayerDistance() < navAgent.stoppingDistance)
                {
                    if (attackCooldown <= 0)
                    {
                        Attack();
                        attackCooldown = 1f / attackSpeed;
                    }
                    else FaceTarget();
                }
                else
                {
                    navAgent.SetDestination(player.transform.position);
                }
            }
            controller.SetSpeed(navAgent.velocity.magnitude);
        }
    }
    public override void Interact()
    {
        base.Interact();
        interacting = true;
        navAgent.isStopped = false;
        //  Attack();

    }
    public override void StopInteract()
    {
        base.StopInteract();
        interacting = false;
        if(navAgent.enabled) navAgent.isStopped = true;
        controller.SetSpeed(0);
    }

    public void Attack()
    {
        FaceTarget();
        controller.Attack();
    }
    void FaceTarget()
    {
        //  Vector3 direction = (player.transform.position - transform.position).normalized;
        //   Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));
        //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime );
        // transform.rotation.SetLookRotation(player.transform.position);
        //navAgent.SetDestination(player.transform.position);
        //   transform.LookAt(player.transform);
        // transform.rotation.SetLookRotation(player.transform.position);

        Vector3 targetDir = player.transform.position - transform.position;
        // The step size is equal to speed times frame time.
        float step = 3   * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
      //  Debug.DrawRay(transform.position, newDir, Color.red);
        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }
    public void Died()
    {
        controller.Die();
        healthBar.SetProgress(0);

        StopInteract();
        navAgent.enabled = false;
        var colider = GetComponent<BoxCollider>();
        colider.size = new Vector3(colider.size.x, colider.size.x/2, colider.size.z);

        Invoke("RemoveEnemy", 3);
        
        Invoke("DeathParticle", 2);
        //TODO: zvuk umiranja
    }
    void DeathParticle()
    {
        onDeathParticle = Instantiate(GameManager.instance.OnEnemyDeathParticle, null);
        onDeathParticle.transform.position = transform.position;
        onDeathParticle.SetActive(true);
        Invoke("RemoveOnDeathParticle",3);

    }
    void RemoveOnDeathParticle()
    {

        Destroy(onDeathParticle);
        Destroy(gameObject);

    }
    void RemoveEnemy()
    {
        gameObject.SetActive(false);
        GameManager.EnemyDied(this);
    }
    public void Knocback()
    {
        controller.KnockBack();
    }
    public void TakeHit()
    {
        if (!interacting) Interact();

        controller.Hit();
        healthBar.SetProgress(enemyStats.CurrentHealth);
    }

    public EnemyController EnemyController
    {
        get { return controller; }
    }
    public bool IsAttacking()
    {
        return controller.IsAttacking();
    }
}
