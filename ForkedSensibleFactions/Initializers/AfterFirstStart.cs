using Boots_SensibleFactions.World_Gen;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using Verse;

namespace Boots_SensibleFactions
{
    [StaticConstructorOnStartup]
    public static class Boots_SensibleFactions_Init
    {
        static Boots_SensibleFactions_Init()
        {
            var harmony = new Harmony("Boots.SensibleFactions");
            harmony.Patch(
                original: AccessTools.Method(typeof(WorldGenerator), nameof(WorldGenerator.GenerateWorld)),
                postfix: new HarmonyMethod(typeof(Boots_SensibleFactions_Init), nameof(AfterWorldGenerated))
            );
        }

        public static void AfterWorldGenerated(World __result)
        {
            if (__result == null)
                return;

            LongEventHandler.ExecuteWhenFinished(() =>
            {
                Colony_Redistributor.ColonyDistributor();
            });
        }
    }
}
