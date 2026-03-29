namespace FTK_MultiMax_Rework_v2 {
    public static class RoomHandler {
        public static bool CreateRoom(ref GameLogic __instance, string _roomName, bool _isOpen) {
            PhotonNetwork.offlineMode = false;
            PhotonNetwork.CreateRoom(_roomName,
                new RoomOptions {
                    IsOpen = _isOpen,
                    IsVisible = _isOpen,
                    MaxPlayers = __instance.m_GameMode == GameLogic.GameMode.SinglePlayer
                        ? (byte)1 : (byte)GameFlowMC.gMaxPlayers
                },
                new TypedLobby { Type = LobbyType.Default });
            return false;
        }
    }
}
