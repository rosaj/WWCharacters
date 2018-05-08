using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{

    [Tooltip("Udaljenost na kojoj će biti druga točka munje kada igrač mjeri u prazno")]
    public int freeShootingDistance = 50;

    [Tooltip("Vrijeme koliko će se munja prikazivat")]
    public float attackTime = 0.5f;

    public Gradient lightningColor;
    public Gradient sparkColor;

    public GameObject sparkParticle;

    public enum Type
    {
        Time,
        Continuous
    }


    private static Type type = Type.Continuous;

    private LightningBoltScript lbScript;
    private GameObject lightningEnd;
    private Player player;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = PlayerManager.PlayerScript;
        lbScript = GetComponent<LightningBoltScript>();
        lightningEnd = lbScript.EndObject;

    }
    private void Update()
    {
     
        // samo za kontinuirani bolt provjeravamo dali igrač još uvijek puca
       if(type == Type.Continuous)
        {   // ako trenutno nije pretisnut fire button ugasi lightning bolt
            var stop = !Input.GetButton(player.MoveBehaviour.attackButton);
            if (stop)
            {
                TurnOffLightningBolt();

                player.MoveBehaviour.GetAnimator.speed = 1;
            }
        }
        
    }

    void ShootLightningBolt()
    {
        
        RaycastHit hit;
      
        Ray ray;
        if (player.IsAiming)
        {
            ray = player.PlayerCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            ray.origin += ray.direction;
        }
        else
        {
            Vector3 fwd = player.transform.TransformDirection(Vector3.forward);
            var from = player.transform.position + player.transform.forward * 2;
            from.y += 1.5f;
            ray = new Ray(from, fwd);
        }
        if (Physics.Raycast(ray, out hit))
        {
            lbScript.EndObject = hit.transform.gameObject;
            var combatWeapon = GetComponent<CombatWeapon>();
            combatWeapon.Start();
            combatWeapon.OnTriggerEnter(lbScript.EndObject.GetComponent<Collider>());
            if (type == Type.Continuous)
            {
                sparkParticle.SetActive(true);
                sparkParticle.transform.parent = hit.transform;
                sparkParticle.transform.localPosition = Vector3.up;
            }
        }
        else
        {
            lbScript.EndPosition = ray.direction * freeShootingDistance;
        }

        if(type == Type.Time) Invoke("TurnOffLightningBolt", attackTime);
        PlayerManager.ConsumeMana();
    }
    void TurnOffLightningBolt()
    {
        sparkParticle.transform.parent = transform;
        sparkParticle.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);

    }
    private void OnDisable()
    {
        StopAllCoroutines();
        ResetLightningTargets();
    }
    void ResetLightningTargets()
    {

        sparkParticle.SetActive(false);
        lbScript.EndObject = lightningEnd;
        lbScript.EndPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        if(type == Type.Time)
        {
            lineRenderer.colorGradient = lightningColor;

            ShootLightningBolt();
        }
        else
        {

            lineRenderer.colorGradient = sparkColor;
            player.MoveBehaviour.GetAnimator.speed = 0;
            StartCoroutine(SparkLightning());
            
        }
       
    }
    IEnumerator SparkLightning()
    {
        while (true)
        {

            ResetLightningTargets();
            ShootLightningBolt();

            yield return new WaitForSeconds(0.5f);

        }
    }
    public static Type LightningType
    {
        get
        {
            return type;
        }

        set
        {
            type = value;          
        }
    }
}
