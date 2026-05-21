
using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV.Extensions;

public static class YieldHelper
{

    public static IEnumerable<T> Yield<T>(this T obj)
    {
        yield return obj;
    }

}
