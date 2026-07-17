using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Economy;
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

    public class MarketplaceContainer(IdentifiableType id, ScorePlort marketplace) : IItemContainer
    {
        public IdentifiableType Id => null!;
        public int Count => 0;
        public int MaxCount => int.MaxValue;
        public bool CanRemove => false;
        public bool CanAdd => marketplace.CanDroneDeposit(id);

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;
            var player = Mod.Player;
            if (player == null)
                return false;

            marketplace.Deposit(id, count);

            player.Ammo.SelectedSlot.Count = 0;
            player.Ammo.SelectedSlot.Id = null;
            return true;
        }

        public bool Remove(int count) => false;
    }

    public class CaretakerShop(IdentifiableType id, ScorePlort sprinkleTrader) : IItemContainer
    {
        public IdentifiableType Id => null!;
        public int Count => 0;
        public int MaxCount => int.MaxValue;
        public bool CanRemove => false;
        public bool CanAdd => sprinkleTrader._sprinkleType == id;

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;
            var player = Mod.Player;
            if (player == null)
                return false;

            SRSingleton<SceneContext>.Instance.PlayerState.AddCurrency(
                sprinkleTrader._sprinkleCurrency.Cast<ICurrency>(), count);
            player.Ammo.SelectedSlot.Id = null;
            return true;
        }

        public bool Remove(int count) => false;
    }

    public class SiloContainer(IdentifiableType id, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly SiloStorage _silo = siloCatcher._storageSilo;

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[siloCatcher.SlotIdx];

        public IdentifiableType Id => AmmoSlot.Id;
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(id));

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
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class WarpDepotContainer(IdentifiableType id, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly LinkedSiloStorage _silo = siloCatcher._storageSilo.Cast<LinkedSiloStorage>() ??
                                                   throw new InvalidOperationException();

        public AmmoSlot AmmoSlot => _silo._linkedAmmo.Slots[_silo._linkedAmmo._selectedAmmoIdx];

        public IdentifiableType Id => _silo.GetSlotIdentifiable(siloCatcher.SlotIdx);
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(id));

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
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class FeederContainer(IdentifiableType id, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly SiloStorage _silo = siloCatcher.gameObject.transform.parent.parent.GetComponent<SiloStorage>();
        private readonly SiloCatcher _siloCatcher = siloCatcher;

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[0];

        public IdentifiableType Id => _silo.GetSlotIdentifiable(0);
        public int Count => _silo.GetSlotCount(0);
        public int MaxCount => _silo.GetSlotMaxCount(0);
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(id));

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
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public abstract class OutputOnlyContainer(
        IdentifiableType id,
        SiloCatcher siloCatcher,
        SiloStorage? siloStorage = null)
        : IItemContainer
    {
        protected readonly IdentifiableType _id = id;
        protected readonly SiloStorage _silo = siloStorage ?? siloCatcher._storageSilo;
        protected readonly SiloCatcher _siloCatcher = siloCatcher;

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
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class PlortCollector(IdentifiableType id, SiloCatcher siloCatcher, SiloStorage? siloStorage = null)
        : OutputOnlyContainer(id, siloCatcher, siloStorage);

    public class SprinkleCanister(IdentifiableType id, SiloCatcher siloCatcher, SiloStorage? siloStorage = null)
        : OutputOnlyContainer(id, siloCatcher, siloStorage);

    public class RefineryContainer(IdentifiableType id, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly SiloCatcher _siloCatcher = siloCatcher;

        public IdentifiableType Id => null!;
        public int Count => SRSingleton<SceneContext>.Instance.GadgetDirector.GetItemCount(id);
        public int MaxCount => GadgetDirector.REFINERY_MAX;
        public bool CanRemove => false;

        public bool CanAdd =>
            SRSingleton<SceneContext>.Instance.GadgetDirector.IsStorable(id) && Count < MaxCount;

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
            SRSingleton<SceneContext>.Instance.GadgetDirector.AddItem(id, toTransfer);

            if (player.Ammo.SelectedSlot.Count <= 0)
                player.Ammo.SelectedSlot.Id = null;

            return true;
        }

        public bool Remove(int count) => false;
    }
}