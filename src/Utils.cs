using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VacuumModifications;

public class Utils
{
    public static T Get<T>(string name)
        where T : Object =>
        Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((Func<T, bool>)(x => x.name == name))!;

    public static GameObject? tryGetPointedObject(VacuumItem vacpack, float distance = 20f)
    {
        var transform = vacpack.VacOrigin.transform;
        Physics.Raycast(
            new Ray(transform.position, transform.up),
            out var hitInfo,
            distance,
            LayerMask.GetMask("Interactable"),
            QueryTriggerInteraction.Collide
        );
        return hitInfo.collider?.gameObject;
    }


    /// <summary>
    ///     Transfers as much as possible from <paramref name="source" /> into <paramref name="target" />,
    ///     limited by the target's remaining space and the source's available count.
    /// </summary>
    /// <param name="source">The <see cref="AmmoSlot" /> to transfer items from.</param>
    /// <param name="target">
    ///     The <see cref="AmmoSlot" /> to transfer items into. Must be empty or hold the same item type as
    ///     <paramref name="source" />.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if any amount was transferred, <see langword="false" /> if the slots ids don't match, the
    ///     slot is null or nothing was transferred.
    /// </returns>
    public static bool tryTransferMaxAmount(AmmoSlot? source, AmmoSlot? target)
    {
        if (
            source == null
            || source.Id == null
            || target == null
            || (source.Id != target.Id && target.Id != null)
        )
            return false;

        var spaceAvailableInTarget = Math.Max(0, target.MaxCount - target.Count);
        var amountToTransferToTarget = Math.Min(source.Count, spaceAvailableInTarget);

        if (amountToTransferToTarget <= 0)
            return false;

        if (target.Id == null)
            target.Id = source.Id;

        target.Count += amountToTransferToTarget;
        source.Count -= amountToTransferToTarget;

        if (source.Count <= 0)
            source.Id = null;

        return true;
    }


    /// <summary>
    ///     Checks whether the given SiloCatcher belongs is an Automatic Feeder.
    /// </summary>
    /// <param name="siloCatcher">
    ///     <see cref="SiloCatcher" />
    /// </param>
    /// <returns>
    ///     <see langword="true" /> If the SiloCatcher is an Automatic Feeder and <see langword="false" /> if it's not an
    ///     Automatic Feeder
    /// </returns>
    private static bool isSlimeFeeder(SiloCatcher siloCatcher)
    {
        var slimeFeeder = siloCatcher.gameObject.transform.parent.parent;
        return slimeFeeder.GetComponent<SiloStorage>() && slimeFeeder.name.Contains("SlimeFeeder");
    }

    public static Containers.IItemContainer? tryGetContainer(
        GameObject go,
        IdentifiableType id = null!
    )
    {
        if (go.TryGetComponent<SiloCatcher>(out var siloCatcher))
            switch (siloCatcher.Type)
            {
                case SiloCatcherType.SILO_DEFAULT:
                {
                    if (isSlimeFeeder(siloCatcher))
                        return new Containers.FeederContainer(id, siloCatcher);

                    if (siloCatcher._storageSilo && siloCatcher._storageSilo.name.Contains("Silo"))
                        return new Containers.SiloContainer(id, siloCatcher);

                    if (
                        siloCatcher._storageSilo.Cast<LinkedSiloStorage>() != null
                        && siloCatcher._storageSilo.name.Contains("WarpDepot")
                    )
                        return new Containers.WarpDepotContainer(id, siloCatcher);
                    break;
                }

                case SiloCatcherType.SILO_OUTPUT_ONLY:
                {
                    // PlortCollector and SprinkleCanister are practically the same, but keeping it separate here in case of a future update.
                    if (siloCatcher._storageSilo.name.Contains("PlortCollector"))
                        return new Containers.PlortCollector(id, siloCatcher);
                    if (siloCatcher._storageSilo.name.Contains("SprinkleCanister"))
                        return new Containers.SprinkleCanister(id, siloCatcher);
                    break;
                }

                case SiloCatcherType.REFINERY:
                {
                    return new Containers.RefineryContainer(id, siloCatcher);
                }

                case SiloCatcherType.DECORIZER:
                case SiloCatcherType.SILO_INPUT_ONLY:
                    break;

                default:
                    throw new InvalidOperationException($"Unhandled SiloCatcherType: {siloCatcher.Type}");
            }

        if (go.TryGetComponent<ScorePlort>(out var scorePlort))
        {
            if (scorePlort._isCaretakerShop) return new Containers.CaretakerShop(id, scorePlort);

            return new Containers.MarketplaceContainer(id, scorePlort);
        }


        MelonDebug.Msg($"Unknown Container! {go.name}");

        return null;
    }


    /// <summary>Determines the target ammo slot for a given item id in the player's inventory.</summary>
    /// <param name="id">The item id to find a slot for.</param>
    /// <returns>
    ///     The <see cref="AmmoSlot" /> to use, selected in this order of priority:
    ///     <list type="number">
    ///         <item>
    ///             <description>A slot already containing the same <paramref name="id" />, if one exists.</description>
    ///         </item>
    ///         <item>
    ///             <description>The currently selected slot, if it can accept this <paramref name="id" />.</description>
    ///         </item>
    ///         <item>
    ///             <description>The slot found by <see cref="AmmoSlotManager.TryFindSlot" />, if any.</description>
    ///         </item>
    ///     </list>
    ///     Returns <see langword="null" /> if none of the above apply.
    /// </returns>
    public static AmmoSlot? FindTargetSlot(IdentifiableType id)
    {
        var playerAmmo = Mod.Player!.Ammo;
        var ammoMetadata = new AmmoSlot.AmmoMetadata(id);
        var matchingSlot = playerAmmo.Slots.FirstOrDefault(slot => slot.Id == id);

        playerAmmo.TryFindSlot(ammoMetadata, out var ammoSlotIndex);
        return matchingSlot ?? (playerAmmo.CouldAddToSelectedSlot(ammoMetadata)
            ? playerAmmo.Slots[playerAmmo._selectedAmmoIdx]
            : playerAmmo.Slots[ammoSlotIndex] ?? null);
    }
}