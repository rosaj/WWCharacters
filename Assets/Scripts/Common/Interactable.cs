using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float updateTime = 0.5f;
    public float interactionRadius = 3f;


    protected GameObject player;

    private bool hasInteracted = false;

    public virtual void Interact()
    {
      //  Debug.Log(gameObject.name + " interacting");
    }
    public virtual void StopInteract()
    {
        //Debug.Log(gameObject.name + " not interacting");
    }
    public virtual void Start()
    {
        player = PlayerManager.instance.player;
        StartCoroutine(CheckInteractions());
      //  Debug.Log("Interactable start");
    }

    IEnumerator CheckInteractions()
    {
        while (true)
        {
            CheckInteraction();
            yield return new WaitForSeconds(updateTime);
        }
    }

    private void CheckInteraction()
    {

        float distance = GetPlayerDistance();
        if (!hasInteracted && distance <= interactionRadius)
        {
            Interact();
            hasInteracted = true;
        }
        else if (distance >= interactionRadius)
        {
            if (hasInteracted) StopInteract();

            hasInteracted = false;
        }


    }
    public float GetPlayerDistance()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

}
