using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player;

namespace VacuumModifications;

public class Containers
{
    public interface IItemContainer
    {
        IdentifiableType Id { get; }
        int Count { get; }
        int MaxCount { get; }
        bool CanRemove { get; }
        bool CanAdd { get; }
        bool Add(int count);
        bool Remove(int count);
    }

    public class MarketplaceContainer : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly ScorePlort _marketplace;

        public MarketplaceContainer(IdentifiableType id, ScorePlort marketplace)
        {
            _id = id;
            _marketplace = marketplace;
        }

        public IdentifiableType Id => null!;
        public int Count => 0;
        public int MaxCount => int.MaxValue;
        public bool CanRemove => false;
        public bool CanAdd => _marketplace.CanDroneDeposit(_id);

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;
            var player = Mod.Player;
            if (player == null)
                return false;

            _marketplace.Deposit(_id, count);

            player.Ammo.SelectedSlot.Count = 0;
            player.Ammo.SelectedSlot.Id = null;
            return true;
        }

        public bool Remove(int count) => false;
    }

    public class SiloContainer : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly SiloStorage _silo;
        private readonly SiloCatcher _siloCatcher;

        public SiloContainer(IdentifiableType id, SiloCatcher siloCatcher)
        {
            _id = id;
            _siloCatcher = siloCatcher;
            _silo = siloCatcher._storageSilo;
        }

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[_siloCatcher.SlotIdx];

        public IdentifiableType Id => AmmoSlot.Id;
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAccept(_id);

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            var player = Mod.Player;
            return player != null && Utils.tryTransferMaxAmount(player.Ammo.SelectedSlot, AmmoSlot);
        }

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Min(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class WarpDepotContainer : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly LinkedSiloStorage _silo;
        private readonly SiloCatcher _siloCatcher;

        public WarpDepotContainer(IdentifiableType id, SiloCatcher siloCatcher)
        {
            _id = id;
            _siloCatcher = siloCatcher;
            _silo = siloCatcher._storageSilo.Cast<LinkedSiloStorage>() ?? throw new InvalidOperationException();
        }

        public AmmoSlot AmmoSlot => _silo._linkedAmmo.Slots[_silo._linkedAmmo._selectedAmmoIdx];

        public IdentifiableType Id => _silo.GetSlotIdentifiable(_siloCatcher.SlotIdx);
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAccept(_id);

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            var player = Mod.Player;
            return player != null && Utils.tryTransferMaxAmount(player.Ammo.SelectedSlot, AmmoSlot);
        }

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Min(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class FeederContainer : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly SiloStorage _silo;
        private readonly SiloCatcher _siloCatcher;

        public FeederContainer(IdentifiableType id, SiloCatcher siloCatcher)
        {
            _id = id;
            _siloCatcher = siloCatcher;
            _silo = siloCatcher.gameObject.transform.parent.parent.GetComponent<SiloStorage>();
        }

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[0];

        public IdentifiableType Id => _silo.GetSlotIdentifiable(0);
        public int Count => _silo.GetSlotCount(0);
        public int MaxCount => _silo.GetSlotMaxCount(0);
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAccept(_id);

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            var player = Mod.Player;
            return player != null && Utils.tryTransferMaxAmount(player.Ammo.SelectedSlot, AmmoSlot);
        }

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Min(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class PlortCollector : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly SiloStorage _silo;
        private readonly SiloCatcher _siloCatcher;

        public PlortCollector(
            IdentifiableType id,
            SiloCatcher siloCatcher,
            SiloStorage? siloStorage = null
        )
        {
            _id = id;
            _siloCatcher = siloCatcher;
            _silo = siloStorage ?? siloCatcher._storageSilo;
        }

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[_siloCatcher.SlotIdx];

        public IdentifiableType Id => AmmoSlot.Id;
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => false;

        public bool Add(int count) => false;

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Min(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class RefineryContainer : IItemContainer
    {
        private readonly IdentifiableType _id;
        private readonly SiloCatcher _siloCatcher;

        public RefineryContainer(IdentifiableType id, SiloCatcher siloCatcher)
        {
            _id = id;
            _siloCatcher = siloCatcher;
        }

        public IdentifiableType Id => null!;
        public int Count => SRSingleton<SceneContext>.Instance.GadgetDirector.GetItemCount(_id);
        public int MaxCount => GadgetDirector.REFINERY_MAX;
        public bool CanRemove => false;

        public bool CanAdd =>
            SRSingleton<SceneContext>.Instance.GadgetDirector.IsStorable(_id) && Count < MaxCount;

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            var player = Mod.Player;
            if (player == null)
                return false;

            var spaceLeft = Math.Max(0, MaxCount - Count);
            var toTransfer = Math.Min(count, spaceLeft);

            if (toTransfer <= 0)
                return false;

            player.Ammo.SelectedSlot.Count -= toTransfer;
            SRSingleton<SceneContext>.Instance.GadgetDirector.AddItem(_id, toTransfer);

            if (player.Ammo.SelectedSlot.Count <= 0)
                player.Ammo.SelectedSlot.Id = null;

            return true;
        }

        public bool Remove(int count) => false;
    }
}
