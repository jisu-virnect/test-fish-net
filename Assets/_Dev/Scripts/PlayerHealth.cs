using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// SyncVar와 Rpc
/// </summary>
public class PlayerHealth : NetworkBehaviour
{
    // 서버로 싱크되었다는것만 보내줌
    private readonly SyncVar<int> health = new SyncVar<int>(10, new SyncTypeSettings(1f));
    
    // 호출 날림
    [ServerRpc] public void UpdateHealth(PlayerHealth script, int amountToChange) => script.health.Value += amountToChange;


    private TMP_Text tmp_HP;
    private void Start()
    {
        tmp_HP = GameObject.FindGameObjectWithTag("HP").GetComponent<TMP_Text>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner) 
        {
            GetComponent<PlayerHealth>().enabled = false;
        }
        else
        {
            //tmp_HP.text = health.Value.ToString();
        }

        // 이벤트
        health.OnChange += OnChange;
    }

    private void OnChange(int prev, int next, bool asServer)
    {
        hp = next;
        tmp_HP.text = hp.ToString();
    }

    public int hp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateHealth(this, -1);
        }
    }



}
