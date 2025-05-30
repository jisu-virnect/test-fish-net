using System;
using Dissonance.Integrations.FishNet.Utils;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dissonance.Integrations.FishNet
{
    // A Player object wrapper for Dissonance Voice
    public sealed class DissonanceFishNetPlayer : NetworkBehaviour, IDissonancePlayer
    {
        [Tooltip("This transform will be used in positional voice processing. If unset, then GameObject's transform will be used.")]
        [SerializeField] private Transform trackingTransform;
        
        // SyncVar ensures that all observers know player ID, even late joiners
        private readonly SyncVar<string> _syncedPlayerName = new (settings: new SyncTypeSettings(WritePermission.ServerOnly, ReadPermission.Observers));

        // Captured DissonanceComms instance
        public DissonanceComms Comms { get; private set; }
        
        
        public string PlayerId => _syncedPlayerName.Value;
        public Vector3 Position => trackingTransform.position;
        public Quaternion Rotation => trackingTransform.rotation;
        public NetworkPlayerType Type => IsOwner ? NetworkPlayerType.Local : NetworkPlayerType.Remote;

        public bool IsTracking { get; private set; }


        private void Awake()
        {
            if (trackingTransform == null) trackingTransform = transform;
        }

        private void OnEnable()
        {
            ManageTrackingState(true);
        }
        
        private void OnDisable()
        { 
            ManageTrackingState(false);
        }

        // Called by FishNet when object is spawned on client with authority
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            if (prevOwner == null || !IsOwner) return;
            
            DissonanceFishNetComms fishNetComms = DissonanceFishNetComms.Instance;
            if (fishNetComms == null)
            {
                LoggingHelper.Logger.Error("Could not find any DissonanceFishNetComms instance! This DissonancePlayer instance will not work!");
                return;
            }

            // Configure Player name
            fishNetComms.Comms.LocalPlayerNameChanged += SetPlayerName;
            if (fishNetComms.Comms.LocalPlayerName == null)
            {
                string randomGuid = Guid.NewGuid().ToString();
                fishNetComms.Comms.LocalPlayerName = randomGuid;
            }
            else
            {
                SetPlayerName(fishNetComms.Comms.LocalPlayerName);
            }
        }

        private void SetPlayerName(string playerName)
        {
            // Disable tracking before name change
            if (IsTracking) ManageTrackingState(false);
            
            // Update name & re-enable tracking
            _syncedPlayerName.Value = playerName;
            ManageTrackingState(true);
            
            // And if owner, sync name over network
            if(IsOwner) ServerRpcSetPlayerName(playerName);
        }
        
        [ServerRpc(RequireOwnership = true)]
        private void ServerRpcSetPlayerName(string playerName)
        {
            _syncedPlayerName.Value = playerName;
        }

        private void OnSyncedPlayerNameUpdated(string _, string updatedName, bool __)
        {
            if(!IsOwner) SetPlayerName(updatedName);
        }

        private void ManageTrackingState(bool track)
        {
            // Check if you should change tracking state
            if (IsTracking == track) return;
            if (DissonanceFishNetComms.Instance == null) return;
            if (track && !DissonanceFishNetComms.Instance.IsInitialized) return;

            // And update it
            DissonanceComms comms = DissonanceFishNetComms.Instance.Comms;
            if (track) comms.TrackPlayerPosition(this);
            else comms.StopTracking(this);

            IsTracking = track;
        }
    }
}