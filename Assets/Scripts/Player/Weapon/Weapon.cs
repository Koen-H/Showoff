using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    internal WeaponData weaponData;

    //private bool canAttack = true;
    internal PlayerCharacterController playerController;
    internal GameObject weaponObj;//The object that exists

    protected Animator weaponAnimation;
    protected BoxCollider weaponCollider; 

    protected WeaponState weaponState = WeaponState.READY;

    protected void Awake()
    {
        playerController = GetComponent<PlayerCharacterController>();
        playerController.gunForward.OnValueChanged += (Vector3 prevValue, Vector3 newValue) => { weaponObj.transform.forward = newValue; };

    }

    /// <summary>
    /// When the player starts with the input
    /// </summary>
    public virtual void OnAttackInputStart()
    {
        if (weaponState != WeaponState.READY) return;
        playerController.AttackStartServerRpc();
        Aim();
    }
    /// <summary>
    /// While the player is holding the input
    /// </summary>
    public virtual void OnAttackInputHold()
    {
        Aim();
    }
    /// <summary>
    /// When the player lets go of the input
    /// </summary>
    public virtual void OnAttackInputStop()
    {
        Aim();
    }

    /// <summary>
    /// For when the enemy stuns/cancels the player during the attack.
    /// </summary>
    public virtual void CancelAttack()
    {

    }


    protected virtual void AfterAttack()
    {
        playerController.ToggleMovement(true);
    }

    /// <summary>
    /// Aim rotates the player towards where the mouse clicks.
    /// </summary>
    internal void Aim(bool isBow = false)
    {
        if (!playerController.IsOwner) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~(LayerMask.GetMask("Decal") | LayerMask.GetMask("UI") | LayerMask.GetMask("Area") | LayerMask.GetMask("Debris"))))
        {
            Vector3 clickPoint = hit.point;
            Transform playerObj = playerController.playerObj.transform;
            playerObj.LookAt(new Vector3(clickPoint.x, playerObj.position.y, clickPoint.z));
            if (isBow)
            {
                weaponObj.transform.LookAt(clickPoint);
                playerController.gunForward.Value = weaponObj.transform.forward;
            }
        }
    }

    public virtual void AttackStart()
    {

    }

    public virtual void Attack()
    {
        //canAttack = false;
        weaponState= WeaponState.COOLDOWN;
        StartCoroutine(Cooldown(weaponData.cooldown));
    }

    IEnumerator Cooldown(float time)
    {
        yield return new WaitForSeconds(time);
        //canAttack = true;
        weaponState = WeaponState.READY;
    }

    protected Transform FindDeepChild(Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = FindDeepChild(child, aName);
            if (result != null)
                return result;
        }
        return null;
    }


}

public enum WeaponType { UNSET, SWORD, BOW, STAFF }
public enum WeaponState { COOLDOWN, READY, START, HOLD, STOP, DISABLED}