using Il2Cpp;

namespace Vacuum_Modifications;

public abstract class AmmoManager
{
    public static int CalculateMaxAmmoAmount(AmmoModel ammoModel, int index, IdentifiableType id)
    {
        var slot = ammoModel.slots[index].Definition;
        bool isLargo = VacuumModifications.LargoGroup!.IsMember(id) || id.referenceId.Equals("SlimeDefinition.Tarr");
        var half = isLargo && VacuumModifications.HalfForMoreVaccablesModLargos!.Value;
        
        if (slot.name.Contains("AmmoSlot") && VacuumModifications.PlayerCustomToggle!.Value)
        {
            return !half ? VacuumModifications.PlayerCustomLimit!.Value : VacuumModifications.PlayerCustomLimit!.Value / 2;
        }

        if (slot.name.Contains("PlortCollector") && VacuumModifications.PlortCollectorCustomToggle!.Value)
        {
            return VacuumModifications.PlortCollectorCustomLimit!.Value;
        }

        if (slot.name.Contains("Feeder") && VacuumModifications.FeederCustomToggle!.Value)
        {
            return VacuumModifications.FeederCustomLimit!.Value;
        }

        if (slot.name.Contains("Silo") && VacuumModifications.SiloCustomToggle!.Value)
        {
            return !half ? VacuumModifications.SiloCustomLimit!.Value : VacuumModifications.SiloCustomLimit!.Value / 2;
        }

        return !half ? slot.MaxAmount : slot.MaxAmount / 2;
    }
}