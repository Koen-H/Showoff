using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Contains all the data of a player.
/// </summary>
public class PlayerData : NetworkBehaviour
{
    //Networkvariables
    [HideInInspector] public NetworkVariable<PlayerRole> playerRole = new(PlayerRole.UNSET, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);//What role is the player?
    [HideInInspector] public NetworkVariable<int> avatarId = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);//What avatar from that role is the player using?
    [HideInInspector] public NetworkVariable<int> weaponId = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);//What weapon from that role is the player using?
    //Local
    public PlayerRoleData playerRoleData = null;//The SO object that has the related data/models

    public override void OnNetworkSpawn()
    {
        playerRole.OnValueChanged += SelectPlayerRoleData;
        if (playerRole.Value != PlayerRole.UNSET) SelectPlayerRoleData();
    }
    public override void OnNetworkDespawn()
    {
        playerRole.OnValueChanged -= SelectPlayerRoleData;
    }

    /// <summary>
    /// When the role changes, so does the related SO.
    /// </summary>
    void SelectPlayerRoleData(PlayerRole oldRole = PlayerRole.UNSET, PlayerRole newRole = PlayerRole.UNSET)
    {
        switch (newRole)
        {
            case PlayerRole.ARTIST:
                playerRoleData = GameData.Instance.artistData;
                break;

            case PlayerRole.DESIGNER:
                playerRoleData = GameData.Instance.designerData;
                break;

            case PlayerRole.ENGINEER:
                playerRoleData = GameData.Instance.engineerData;
                break;

            default:
                break;
        }
        Debug.Log($"{newRole} And avatar id {avatarId.Value}");
    }

    /// <summary>
    /// Set the player Data
    /// </summary>
    public void SetData(PlayerRole newPlayerRole, int newAvatarId, int newWeaponId)
    {
        avatarId.Value = newAvatarId;
        playerRole.Value = newPlayerRole;
        weaponId.Value = newWeaponId;
    }

}
public enum PlayerRole {UNSET, ARTIST, DESIGNER, ENGINEER}