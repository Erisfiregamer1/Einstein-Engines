using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Arcadis.Bitrunning.Netpod
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class NetpodComponent : Component
    {
        [DataField]
        public string NetpodContainer = "netpod_bodyContainer";

        [Serializable, NetSerializable]
        public enum NetpodVisuals : byte
        {
            Status
        }

        [Serializable, NetSerializable]
        public enum NetpodStatus : byte
        {
            Opening, // Person is exiting netpod
            Open, // Netpod is open, unpowered
            OpenPowered, // Netpod is open, powered
            Closing, // Person is entering netpod
            Closed, // Netpod is closed, unpowered
            ClosedPowered // Netpod is closed, powered
        }
        public NetpodStatus Status { get; set; } = NetpodStatus.Open;
    }
}
