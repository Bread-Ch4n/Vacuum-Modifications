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

    public static bool tryTransferMaxAmount(AmmoSlot? source, AmmoSlot? target)
    {
        if (
            source == null
            || source.Id == null
            || target == null
            || (source.Id != target.Id && target.Id != null)
        )
            return false;

        var val2 = Math.Max(0, target.MaxCount - target.Count);
        var count = Math.Min(source.Count, val2);

        if (target.Id == null)
        {
            target.Id = source.Id;
            target.Count = count;
        }
        else
        {
            if (count <= 0)
                return false;
            target.Count += count;
        }

        source.Count -= count;

        if (source.Count <= 0)
            source.Id = null;
        return true;
    }

    private static bool isSlimeFeeder(SiloCatcher siloCatcher)
    {
        var slimeFeeder = siloCatcher.gameObject.transform.parent.parent;
        var siloStorage = slimeFeeder.GetComponent<SiloStorage>();
        return siloStorage && slimeFeeder.name.Contains("SlimeFeeder");
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
                    if (siloCatcher._storageSilo.name.Contains("PlortCollector"))
                        return new Containers.PlortCollector(id, siloCatcher);
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
                    throw new ArgumentOutOfRangeException();
            }

        if (go.TryGetComponent<ScorePlort>(out var marketplace))
            return new Containers.MarketplaceContainer(id, marketplace);

        MelonLogger.Msg($"Unknown Container! {go.name}");

        return null;
    }

    public static AmmoSlot? FindTargetSlot(IdentifiableType id)
    {
        var playerAmmo = Mod.Player!.Ammo;
        var selected = playerAmmo.SelectedSlot;

        var matchingSlot = playerAmmo.Slots.FirstOrDefault(slot => slot.Id == id);
        if (matchingSlot != null)
            return matchingSlot;

        if (selected.Id == id || selected.Id == null)
            return selected;

        return playerAmmo.Slots.FirstOrDefault(slot => slot.Id == null && slot.IsUnlocked);
    }
}
