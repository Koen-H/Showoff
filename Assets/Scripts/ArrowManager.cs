using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private bool attached = false;
    private Transform attachedObject;
    public float damage = 0;
    public PlayerCharacterController playerController;
    [SerializeField] AudioSource arrowHitSFX;
    

    private void OnTriggerEnter(Collider other)
    {
        if (!playerController) return;
        if (attached) return;
        if(other.tag == "Enemy")
        {
            if(playerController.IsOwner)
            other.transform.parent.GetComponent<Enemy>().TakeDamage(damage, playerController.damageType);
        }
        arrowHitSFX.Play();
        attached = true;
        attachedObject = other.gameObject.transform;
        this.transform.parent = attachedObject;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(GetComponent<Collider>());
        StartCoroutine(KillAfterDuration(10)); 
    }

    IEnumerator KillAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject); 
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision! As OnCollisionEnter");
    }
    private void Update()
    {
        if (attached)
        {
            if (attachedObject == null)//Destroy if the object is gone.
            {
                Destroy(gameObject);
                return;
            }
        }
    }

}
