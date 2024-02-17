using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vacuum_Modifications;

public abstract class Utils
{
    public static T Get<T>(string name) where T : Object =>
        Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((Func<T, bool>)(x => x.name == name))!;
}