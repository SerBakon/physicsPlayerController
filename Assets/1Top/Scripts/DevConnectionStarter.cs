using PurrLobby;
using PurrNet;
using PurrNet.Logging;
using PurrNet.Transports;
using System.Collections;
using UnityEngine;

public class DevConnectionStarter : MonoBehaviour
{

    private NetworkManager _networkManager;
    private LobbyDataHolder _lobbyDataHolder;

    private bool isFromlobby;

    private void Awake() {
        if (!TryGetComponent(out _networkManager)) {
            PurrLogger.LogError($"Failed to get {nameof(NetworkManager)} component.", this);
        }

        _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        if (_lobbyDataHolder)
            isFromlobby = true;
        else 
            isFromlobby = false;
    }

    private void Start() {
        if (!_networkManager) {
            PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
            return;
        }
        if (isFromlobby) {
            startFromLobby();
        } else {
            //startNormal();
        }

        
    }

    //private void startNormal() {
    //    _networkManager.transport = transform.GetComponent<UDPTransport>();

    //    if(!ParrelSync.ClonesManager.IsClone())
    //        _networkManager.StartServer();
    //    _networkManager.StartClient();
    //}

    private void startFromLobby() {
        _networkManager.transport = transform.GetComponent<PurrTransport>();
        if (!_lobbyDataHolder) {
            PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
            return;
        }

        if (!_lobbyDataHolder.CurrentLobby.IsValid) {
            PurrLogger.LogError($"Failed to start connection. Lobby is invalid!", this);
            return;
        }

        if (_networkManager.transport is PurrTransport) {
            (_networkManager.transport as PurrTransport).roomName = _lobbyDataHolder.CurrentLobby.LobbyId;
        }

#if UTP_LOBBYRELAY
            else if(_networkManager.transport is UTPTransport) {
                if(_lobbyDataHolder.CurrentLobby.IsOwner) {
                    (_networkManager.transport as UTPTransport).InitializeRelayServer((Allocation)_lobbyDataHolder.CurrentLobby.ServerObject);
                }
                (_networkManager.transport as UTPTransport).InitializeRelayClient(_lobbyDataHolder.CurrentLobby.Properties["JoinCode"]);
            }
#else
        //P2P Connection, receive IP/Port from server
#endif

        if (_lobbyDataHolder.CurrentLobby.IsOwner)
            _networkManager.StartServer();
        StartCoroutine(StartClient());
    }

    private IEnumerator StartClient() {
        yield return new WaitForSeconds(1f);
        _networkManager.StartClient();
    }
}
