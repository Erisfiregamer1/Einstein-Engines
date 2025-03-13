using Content.Shared.DragDrop;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace Content.Shared._Arcadis.Bitrunning.Netpod
{
    public sealed class NetpodSystem : EntitySystem
    {

        [Dependency] private readonly SharedContainerSystem _containerSystem = default!;

        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

        [Dependency] private readonly IGameTiming _gameTiming = default!;

    [Dependency] private readonly SharedPowerReceiverSystem _power = default!;
        private TimeSpan _openAnimDelay = TimeSpan.FromSeconds(1);
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<NetpodComponent, EntInsertedIntoContainerMessage>(OnInsertedIntoContainer);
            SubscribeLocalEvent<NetpodComponent, EntRemovedFromContainerMessage>(OnRemovedFromContainer);
            //SubscribeLocalEvent<NetpodComponent, CanDropTargetEvent>(OnCanDropTarget);
        }

        private void OnInsertedIntoContainer(EntityUid uid, NetpodComponent component, EntInsertedIntoContainerMessage args)
        {
            if (args.Container != _containerSystem.GetContainer(uid, component.NetpodContainer))
                return;
            // Set current status to closing and set the delay for the animation
            component.Status = NetpodComponent.NetpodStatus.Closing;
            _openAnimDelay = _gameTiming.CurTime + TimeSpan.FromSeconds(1.8);
        }

        private void OnRemovedFromContainer(EntityUid uid, NetpodComponent component, EntRemovedFromContainerMessage args)
        {
            if (args.Container != _containerSystem.GetContainer(uid, component.NetpodContainer))
                return;
            // Set current status to opening and set the delay for the animation
            component.Status = NetpodComponent.NetpodStatus.Opening;
            _openAnimDelay = _gameTiming.CurTime + TimeSpan.FromSeconds(1.8);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var entity in EntityManager.GetEntities())
            {
                if (EntityManager.TryGetComponent(entity, out NetpodComponent? netpod))
                {
                    netpod.Status = (netpod.Status, _power.IsPowered(entity)) switch
                    {
                        (NetpodComponent.NetpodStatus.OpenPowered, false) => NetpodComponent.NetpodStatus.Open,
                        (NetpodComponent.NetpodStatus.ClosedPowered, false) => NetpodComponent.NetpodStatus.Closed,
                        (NetpodComponent.NetpodStatus.Open, true) => NetpodComponent.NetpodStatus.OpenPowered,
                        (NetpodComponent.NetpodStatus.Closed, true) => NetpodComponent.NetpodStatus.ClosedPowered,
                        _ => netpod.Status
                    };

                    netpod.Status = (netpod.Status, _openAnimDelay > _gameTiming.CurTime) switch
                    {
                        (NetpodComponent.NetpodStatus.Opening, false) => NetpodComponent.NetpodStatus.Open,
                        (NetpodComponent.NetpodStatus.Closing, false) => NetpodComponent.NetpodStatus.Closed,
                        _ => netpod.Status
                    };


                    _appearance.SetData(entity, NetpodComponent.NetpodVisuals.Status, netpod.Status);
                }
            }
        }


    }
}
