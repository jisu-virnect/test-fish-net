using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Broadcast_Chat : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject msgElement;
    public TMP_InputField input_Username, input_Message;

    private void OnEnable()
    {
        //InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnServerMessageReceived);
        //InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageReceived);
    }

    private void OnDisable()
    {
        //InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnServerMessageReceived);
        //InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageReceived);
    }

    private void OnClientMessageReceived(NetworkConnection connection, Message message, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(message);
    }

    private void OnServerMessageReceived(Message message, Channel channel)
    {
        GameObject go = Instantiate(msgElement, scrollRect.content);
        var tmps = go.GetComponentsInChildren<TMP_Text>();
        tmps[0].text = message.username;
        tmps[1].text = message.message;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    private void SendMessage()
    {
        Message message = new Message()
        {
            username = input_Username.text,
            message = input_Message.text,
        };

        if (InstanceFinder.IsServer)
        {
            InstanceFinder.ServerManager.Broadcast(message);
        }
        else if (InstanceFinder.IsClient)
        {
            InstanceFinder.ClientManager.Broadcast(message);
        }
    }
}

public struct Message : IBroadcast
{
    public string username;
    public string message;
}