using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    private void Awake()
    {
        if (Instance)
            Destroy(Instance.gameObject);
        Instance = this;
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            // Need to save the allocation to get the join code.
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            // Generate a join code.
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // Set the relay data.
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return "Error";
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            // Join allocation by join code.
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

}
