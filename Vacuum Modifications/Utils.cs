using System;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;
using Vacuum_Modifications.containers;
using Object = UnityEngine.Object;

namespace Vacuum_Modifications;

public abstract class Utils
{
    public static T Get<T>(string name) where T : Object =>
        Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((Func<T, bool>)(x => x.name == name))!;

    public static GameObject tryGetPointedObject(VacuumItem vacpack, float distance = 10f)
    {
        Transform transform = vacpack.VacOrigin.transform;
        Physics.Raycast(new Ray(transform.position, transform.up), out var hitInfo, distance, LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Collide);
        return hitInfo.collider?.gameObject;
    }

    public static T tryGetPointedObject<T>(VacuumItem vacpack, float distance = 10f) where T : Component
    {
        GameObject pointedObject = tryGetPointedObject(vacpack, distance);
        return pointedObject == null ? null : pointedObject.GetComponent<T>();
    }

    public static void tryTransferMaxAmount(Ammo.Slot source, Ammo.Slot target)
    {
        if (source == null || target == null) return;
        if (source.Id != target.Id && target.Id !=null)
        {
            MelonLogger.Error(
                $"tryTransferMaxAmount: source and target have different ids! (source id: {source.Id}, target id: {target.Id})");
            return;
        }

        int val2 = Math.Max(0, target.MaxCount - target.Count);
        int count = Math.Min(source.Count, val2);

        if (target.Id == null)
        {
            target.Id = source.Id;
            target.Count = count;
            source.Count -= count;
        }
        else
        {
            if (count > 0)
            {
                target.Count += count;
                source.Count -= count;
            }
        }



        return;
    }
    public static IItemContainer tryGetContainer(GameObject go, IdentifiableType id = null)
    {
        if (go.TryGetComponent<ScorePlort>(out var scorePlort))
            return new MarketContainer(scorePlort);

        return null;
    }
    //
    // public IItemContainer tryGetContainer(SiloCatcher siloCatcher, IdentifiableType id = null)
    // {
    //     return siloCatcher?.Type switch
    //     {
    //         SiloCatcherType.REFINERY => new SiloContainer(siloCatcher, id),
    //         SiloCatcherType.DECORIZER => new DecorizerContainer(siloCatcher, id),
    //         SiloCatcherType.SILO_DEFAULT => new RefineryContainer(id),
    //         SiloCatcherType.SILO_OUTPUT_ONLY => new SiloOutputContainer(siloCatcher, id),
    //         SiloCatcherType.SILO_INPUT_ONLY => new ViktorContainer(siloCatcher, id),
    //         _ => null
    //     };
    // }
}