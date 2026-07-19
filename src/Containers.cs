using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.Player;

namespace VacuumModifications;

public static class Containers
{
    public interface IItemContainer
    {
        int Count { get; }
        int MaxCount { get; }
        bool CanRemove { get; }
        bool CanAdd { get; }
        bool Add(int count);
        bool Remove(int count);
    }

    public class MarketplaceContainer(AmmoSlot sourceAmmoSlot, ScorePlort marketplace) : IItemContainer
    {
        public int Count => 0;
        public int MaxCount => int.MaxValue;
        public bool CanRemove => false;

        public bool CanAdd => marketplace.CanDroneDeposit(sourceAmmoSlot.Id) &&
                              sourceAmmoSlot.Id != marketplace._sprinkleType;

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            marketplace.Deposit(sourceAmmoSlot.Id, count);

            sourceAmmoSlot.Clear();
            return true;
        }

        public bool Remove(int count) => false;
    }

    public class CaretakerShop(AmmoSlot sourceAmmoSlot, ScorePlort sprinkleTrader) : IItemContainer
    {
        public int Count => 0;
        public int MaxCount => int.MaxValue;
        public bool CanRemove => false;
        public bool CanAdd => sprinkleTrader._sprinkleType == sourceAmmoSlot.Id;

        public bool Add(int count)
        {
            if (!CanAdd || sourceAmmoSlot.Id != sprinkleTrader._sprinkleType)
                return false;

            SRSingleton<SceneContext>.Instance.PlayerState.AddCurrency(
                sprinkleTrader._sprinkleCurrency.Cast<ICurrency>(), count);
            sourceAmmoSlot.Clear();
            return true;
        }

        public bool Remove(int count) => false;
    }

    public class SiloContainer(AmmoSlot sourceAmmoSlot, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly SiloStorage _silo = siloCatcher._storageSilo;

        public AmmoSlot AmmoSlot => _silo.Ammo.Slots[siloCatcher.SlotIdx]!;

        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(sourceAmmoSlot.Id));

        public bool Add(int count) => CanAdd && Utils.tryTransferMaxAmount(sourceAmmoSlot, AmmoSlot);

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class WarpDepotContainer(AmmoSlot sourceAmmoSlot, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly LinkedSiloStorage _silo = siloCatcher._storageSilo.Cast<LinkedSiloStorage>() ??
                                                   throw new InvalidOperationException();

        public AmmoSlot AmmoSlot => _silo._linkedAmmo.Slots[_silo._linkedAmmo._selectedAmmoIdx]!;

        public IdentifiableType Id => _silo.GetSlotIdentifiable(siloCatcher.SlotIdx);
        public int Count => AmmoSlot.Count;
        public int MaxCount => AmmoSlot.MaxCount;
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(sourceAmmoSlot.Id));

        public bool Add(int count) => CanAdd && Utils.tryTransferMaxAmount(sourceAmmoSlot, AmmoSlot);

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public class FeederContainer(AmmoSlot sourceAmmoSlot, SiloCatcher siloCatcher) : IItemContainer
    {
        private readonly SiloStorage _silo = siloCatcher.gameObject.transform.parent.parent.GetComponent<SiloStorage>();

        public AmmoSlot AmmoSlot => _silo.Ammo.SelectedSlot!;

        public int Count => _silo.GetSlotCount(0);
        public int MaxCount => _silo.GetSlotMaxCount(0);
        public bool CanRemove => Count > 0;
        public bool CanAdd => _silo.CanAnySlotAccept(new AmmoSlot.AmmoMetadata(sourceAmmoSlot.Id));

        public bool Add(int count) => CanAdd && Utils.tryTransferMaxAmount(sourceAmmoSlot, AmmoSlot);

        public bool Remove(int count)
        {
            if (!CanRemove)
                return false;
            AmmoSlot.Count = Math.Max(0, AmmoSlot.Count - count);
            return true;
        }
    }

    public abstract class OutputOnlyContainer(
        SiloCatcher siloCatcher,
        SiloStorage? siloStorage = null)
        : IItemContainer
    {
        private readonly SiloStorage _silo = siloStorage ?? siloCatcher._storageSilo;

        public AmmoSlot AmmoSlot => _silo.Ammo.SelectedSlot;

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

    public class PlortCollector(SiloCatcher siloCatcher, SiloStorage? siloStorage = null)
        : OutputOnlyContainer(siloCatcher, siloStorage);

    public class SprinkleCanister(SiloCatcher siloCatcher, SiloStorage? siloStorage = null)
        : OutputOnlyContainer(siloCatcher, siloStorage);

    public class RefineryContainer(AmmoSlot sourceAmmoSlot) : IItemContainer
    {
        public int Count => SRSingleton<SceneContext>.Instance.GadgetDirector.GetItemCount(sourceAmmoSlot.Id);
        public int MaxCount => GadgetDirector.REFINERY_MAX;
        public bool CanRemove => false;

        public bool CanAdd =>
            SRSingleton<SceneContext>.Instance.GadgetDirector.IsStorable(sourceAmmoSlot.Id) && Count < MaxCount;

        public bool Add(int count)
        {
            if (!CanAdd)
                return false;

            var spaceLeft = Math.Max(0, MaxCount - Count);
            var toTransfer = Math.Min(count, spaceLeft);

            if (toTransfer <= 0)
                return false;

            sourceAmmoSlot.Count -= toTransfer;
            SRSingleton<SceneContext>.Instance.GadgetDirector.AddItem(sourceAmmoSlot.Id, toTransfer);

            if (sourceAmmoSlot.Count <= 0)
                sourceAmmoSlot.Clear();

            return true;
        }

        public bool Remove(int count) => false;
    }
}