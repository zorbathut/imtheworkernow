using CommunityCoreLibrary_ImTheWorkerNow;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ImTheWorkerNow
{
    class Bootstrap : Def
    {
        static Bootstrap()
        {
            try
            {
                {
                    MethodInfo method1 = typeof(RimWorld.Building_CommsConsole).GetMethod("GetFloatMenuOptions", BindingFlags.Instance | BindingFlags.Public);
                    MethodInfo method2 = typeof(Building_CommsConsole_Detour).GetMethod("GetFloatMenuOptions", BindingFlags.Static | BindingFlags.Public);
                    if (!Detours.TryDetourFromTo(method1, method2))
                    {
                        Log.Error("EVERYTHING IS BROKEN 1");
                        return;
                    }
                }

                {
                    MethodInfo method1 = typeof(RimWorld.FloatMenuMakerMap).GetMethod("AddDraftedOrders", BindingFlags.Static | BindingFlags.NonPublic);
                    MethodInfo method2 = typeof(FloatMenuMakerMap_Detour).GetMethod("AddDraftedOrders", BindingFlags.Static | BindingFlags.Public);
                    if (!Detours.TryDetourFromTo(method1, method2))
                    {
                        Log.Error("EVERYTHING IS BROKEN 2");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("something is seriously wrong");
            }
        }
    }
}
