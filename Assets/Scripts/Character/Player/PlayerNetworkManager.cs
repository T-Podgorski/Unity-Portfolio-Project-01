using Unity.Collections;
using Unity.Netcode;

public class PlayerNetworkManager : CharacterNetworkManager
{
    public NetworkVariable<FixedString64Bytes> playerName =
        new NetworkVariable<FixedString64Bytes>( "Default Name", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner );
}
