using Il2Cpp;
using Il2CppSystem;

namespace Vacuum_Modifications.containers;

class MarketContainer : IItemContainer
{
    readonly ScorePlort scorePlort;

    public MarketContainer(ScorePlort scorePlort)
    {
        this.scorePlort = scorePlort;
    }

    public int Count => 0;
    public int MaxCount => int.MaxValue;

    public bool CanAdd(IdentifiableType id) => scorePlort.CanDeposit(id);
    public bool CanRemove => false;

    public bool add(IdentifiableType id, int count)
    {
        var player = VacuumModifications.Player;
        var deposits = scorePlort.Deposit(id, count, new Nullable<PlayerState.CoinsType>(PlayerState.CoinsType.NORM), true)?.Deposits ?? 0;
        player?.Ammo.DecrementAmmo(player.Ammo.Slots[player.Ammo.SelectedAmmoIndex], deposits);
        return deposits > 0;
    }

    public bool remove(int _) => false;
}