using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// SyncVar�� Rpc
/// </summary>
public class PlayerHealth : NetworkBehaviour
{
    // ������ ��ũ�Ǿ��ٴ°͸� ������
    private readonly SyncVar<int> health = new SyncVar<int>(10, new SyncTypeSettings(1f));
    
    // ȣ�� ����
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

        // �̺�Ʈ
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
