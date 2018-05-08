using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveBehaviour))]
[RequireComponent(typeof(BasicBehaviour))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Player : MonoBehaviour {
    public enum PlayerType
    {
        Character = 0,
        Sword = 1,
        Magic = 2,
        Bow = 3
    }
    public enum AttackType
    {
        Default = 0,
        SwordAttack1 = 0,
        SwordAttack2 = 1,
        SwordAttack3 = 2,
        SwordSlash1 = 3,
        SwordSlash2 = 4,
        SwordBackHand = 5,
        SwordCombo1 = 6,
        SwordCombo2 = 7,
        SwordCombo3 = 8,
        SwordHorizontal = 9,
        SwordJump = 10,
        FireBall = 0,
        FireBolt = 1,
        LightningBolt = 2,
        Spark = 3,
        Bow = 0,
        BowKnockback = 1,
        BowPiercing = 2
    }

    public int ShootingInertion = 3000;

    public Texture2D crosshair;

    public int defaultSwordIndex = 0;
    public GameObject[] swords;
    public GameObject bow;
    public GameObject arrow;
    public GameObject arrowPiercingParticle;
    public GameObject arrowKnockbackParticle;
    public GameObject magicBlue;
    public GameObject fireBall;
    public GameObject fireBolt;
    public GameObject lightningBolt;

    //   private GameObject arrowHolder;

    private PlayerType type = PlayerType.Character;
    private AttackType attackType = AttackType.Spark;

    private MoveBehaviour moveBehaviour;
    private Camera playerCamera;



    private Dictionary<PlayerType, List<GameObject>> weapons = new Dictionary<PlayerType, List<GameObject>>();

    private int floatPlayerType = Animator.StringToHash("PlayerType");
   

    // Use this for initialization
    void Start () {
        SetWeaponsForPlayer(PlayerType.Character, null);
        SetWeaponsForPlayer(PlayerType.Sword, swords);
        SetWeaponsForPlayer(PlayerType.Magic, magicBlue);
        SetWeaponsForPlayer(PlayerType.Bow, bow);
        moveBehaviour = GetComponent<MoveBehaviour>();
        playerCamera = GetComponent<BasicBehaviour>().playerCamera.GetComponent<Camera>();

        foreach(var sword in swords)
        {
            sword.GetComponentInChildren<CombatWeapon>().OnDamage += (GameObject source, GameObject enemy ) =>{
                 if(attackType == AttackType.SwordJump) enemy.GetComponent<Enemy>().Knocback();
            };
        }

    }
    void SetWeaponsForPlayer(PlayerType playerType,params GameObject[] objects)
    {
        List<GameObject> list = new List<GameObject>();
        if(objects != null) list.AddRange(objects);
        weapons.Add(playerType, list);
    }
        

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("1"))
        {
            Type = PlayerType.Character;
        }

        else if (Input.GetKeyDown("2"))
        {
            Type = PlayerType.Sword;
        }
        else if (Input.GetKeyDown("3"))
        {
            if (AttackType.Spark == attackType) attackType = AttackType.LightningBolt;
            else attackType = AttackType.Spark;
            Type = PlayerType.Magic;
        }
        else if (Input.GetKeyDown("4"))
        {
            Type = PlayerType.Bow;
        }
        
     }

    public PlayerType Type
    {
        get
        {
            return type;
        }

        set
        {
            weapons[type].ForEach((weapon) => weapon.SetActive(false));
            type = value;
            // weapons[type].ForEach((weapon) => weapon.SetActive(true));
            int index = 0;
            if (type == PlayerType.Sword) index = defaultSwordIndex;

            GameObject currentWeapon = null;
            if(weapons[type].Count> index) currentWeapon = weapons[type][index]; 
            if (currentWeapon != null) currentWeapon.SetActive(true);
            moveBehaviour.GetAnimator.SetFloat(floatPlayerType, (int)type);
        }
    }
    public AttackType Attack
    {
        get { return attackType; }
        set
        {
            attackType = value;
            //TODO: nesto tu mozda kad se promijeni nacin napada?

        }
    }
    public int AttackIndex
    {
        get { return (int)attackType; }
        set
        {
            if (type == PlayerType.Magic)
            {
                attackType = AttackType.FireBall;
                switch (value)
                {
                    case 0: attackType = AttackType.FireBall; break;
                    case 1: attackType = AttackType.FireBolt; break;
                    case 2: attackType = AttackType.LightningBolt; break;
                    case 3: attackType = AttackType.Spark; break;
                }

            }
            else if (type == PlayerType.Bow)
            {
                attackType = AttackType.Bow;
                switch (value)
                {
                    case 0: attackType = AttackType.Bow; break;
                    case 1: attackType = AttackType.BowKnockback; break;
                    case 2: attackType = AttackType.BowPiercing; break;
                }
            }
            else if (type == PlayerType.Sword)
            {
                attackType = AttackType.SwordAttack1;
                if(value == (int)AttackType.SwordJump)
                {
                    attackType = AttackType.SwordJump;
                }
            }
            else attackType = AttackType.Default;
        }
    }

    public void SetActiveSword(int index)
    {
        if(index < swords.Length)
        {
            swords[defaultSwordIndex].SetActive(false);
            defaultSwordIndex = index;
            swords[defaultSwordIndex].SetActive(true);
        }
    }

    // Ovu metodu poziva event animacije kada lik otpušta strijelu
    public void OnArrowReleased()
    {
        // kreiranje noveg projektila
        var currentArrow = CloneProjectile(arrow);
        // sakrij strelicu
        arrow.SetActive(false);
        var combatWeapon = currentArrow.GetComponent<CombatWeapon>();
        if (combatWeapon)
        {
            if (attackType == AttackType.Bow)
                combatWeapon.OnDamage += (source, enemy) => Destroy(source);
            else if (attackType == AttackType.BowKnockback)
                combatWeapon.OnDamage += (GameObject source, GameObject enemy) =>
                {
                    Destroy(source);
                    var enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent) enemyComponent.Knocback();
                };
        }
       
        var rb = currentArrow.GetComponent<Rigidbody>();

        rb.AddForce(currentArrow.transform.forward * ShootingInertion);
        Destroy(currentArrow, 5f);
    }
    // Ovu metodu poziva event animacije kada lik dohvača strijelu
    public void OnArrowPulled()
    {
        //prikazi strelicu
        arrow.SetActive(true);
        arrowKnockbackParticle.SetActive(false);
        arrowPiercingParticle.SetActive(false);

        if (attackType == AttackType.BowKnockback) arrowKnockbackParticle.SetActive(true);
        else if(attackType == AttackType.BowPiercing) arrowPiercingParticle.SetActive(true);

    }
    // Ovu metodu poziva event animacije koja baca magiju
    void OnMagicThrowed()
    {
        switch (attackType)
        {
            case AttackType.FireBall:
                ThrowBall(fireBall);
                break;
            case AttackType.FireBolt:
                ThrowBall(fireBolt);
                break;
            case AttackType.LightningBolt:
                ThrowLightning(LightningBolt.Type.Time);
                break;
            case AttackType.Spark:
                ThrowLightning(LightningBolt.Type.Continuous);
                break;
        }
        
    }


    void ThrowLightning(LightningBolt.Type type)
    {
      
        LightningBolt.LightningType = type;
        lightningBolt.SetActive(true);
        
    }

    void ThrowBall(GameObject ball)
    {

        var fb = CloneProjectile(ball);
        fb.SetActive(true);
        var rb = fb.GetComponent<Rigidbody>();
        rb.AddForce(fb.transform.forward * ShootingInertion);
        Destroy(fb, 5);
        PlayerManager.ConsumeMana();
    }

    // ovo služi samo za radnom napade mačen
    public int GetAttackType
    {
        get {if(attackType == AttackType.SwordAttack1) return Random.Range((int)AttackType.SwordAttack1, (int)AttackType.SwordJump);
            return (int)attackType;
        }

    }



    GameObject CloneProjectile(GameObject template)
    {
        var go = Instantiate(template);
        go.transform.position = template.transform.position;
        go.transform.rotation = transform.rotation;

        if (moveBehaviour.IsAiming)  go.transform.rotation = playerCamera.transform.rotation;
        
        // ako puca iz luka onda treba rotirat putanje strijele za 90 stupnjeva uljevo
        else if(Type == PlayerType.Bow)
        {
            go.transform.Rotate(Vector3.up*-90);
        }

        go.transform.SetParent(this.transform.parent);
        go.AddComponent<Rigidbody>();
        var combatWeapon = go.GetComponent<CombatWeapon>();
        combatWeapon.Stats = PlayerManager.PlayerStats;
      //  combatWeapon.MoveBehaviour = moveBehaviour;

        return go;
    }
    public bool IsAiming
    {
        get { return moveBehaviour.IsAiming; }
    }
    public bool IsAttacking
    {
        get { return moveBehaviour.GetAttacks; }
    }
    public void DoAttack()
    {
        moveBehaviour.Attack();
    }



    public Camera PlayerCamera
    {
        get { return playerCamera; }
    }
    public MoveBehaviour MoveBehaviour
    {
        get { return moveBehaviour; }
    }
}
