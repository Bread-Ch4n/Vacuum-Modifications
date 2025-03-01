using Il2Cpp;

namespace Vacuum_Modifications;

public interface IItemContainer
{

    int Count { get; }
    int MaxCount { get; }

    bool CanAdd(IdentifiableType id);
    bool CanRemove { get; }

    bool add(IdentifiableType id, int count);
    bool remove(int count);
}