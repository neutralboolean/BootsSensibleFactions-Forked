using Boots_SensibleFactions.World_Gen;
using LudeonTK;
using RimWorld;
using Verse;

namespace Boots_SensibleFactions.DebugActions
{
    [StaticConstructorOnStartup]
    public static class DebugAction_ColonyRedistributor
    {
        [DebugAction("Sensible Factions", "Re-run Colony Redistribution", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        public static void RedistributeColonies()
        {
            if (Find.World == null || Find.WorldObjects == null)
            {
                Messages.Message("World data not available.", MessageTypeDefOf.RejectInput, false);
                return;
            }

            Colony_Redistributor.ColonyDistributor();
            Messages.Message("Colony redistribution completed.", MessageTypeDefOf.TaskCompletion, false);
        }
    }
}
