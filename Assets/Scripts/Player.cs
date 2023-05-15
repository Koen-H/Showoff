using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private float health; 


    public void TakeDamage(float damage)
    {
        if (damage > 0) health -= damage;
        if (health <= 0) Debug.Log("Player Died");
    }
    void Update()
    {
<<<<<<< HEAD:Assets/Scripts/Player.cs
        // Check if this is the local player.
        if (IsLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
=======
        if (!IsOwner)
            return;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
>>>>>>> character-select-with-sync:Assets/Player.cs

            Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
    }
}
