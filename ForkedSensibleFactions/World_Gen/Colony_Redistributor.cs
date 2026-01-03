using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Boots_SensibleFactions.World_Gen
{
    public static class Colony_Redistributor
    {
        public static void ColonyDistributor()
        {
            List<Settlement> allSettlements = Find.WorldObjects.Settlements
                .Where(s => !s.Faction.IsPlayer)
                .ToList();

            PlanetLayerDef surfacePlanetLayerDef = DefDatabase<PlanetLayerDef>.GetNamedSilentFail("Surface");

            List<Faction> factions = Find.FactionManager.AllFactions
                .Where(f => !f.IsPlayer
                         && !f.def.hidden
                         && f.def.settlementGenerationWeight > 0f
                         && (surfacePlanetLayerDef == null || f.def.neutralArrivalLayerBlacklist == null || !f.def.neutralArrivalLayerBlacklist.Contains(surfacePlanetLayerDef)))
                .ToList();

            var quotas = factions.ToDictionary(
                f => f,
                f => allSettlements.Count(s => s.Faction == f)
            );

            allSettlements = allSettlements
                .Where(s => factions.Contains(s.Faction))
                .ToList();

            if (quotas.Values.Sum() != allSettlements.Count || quotas.Count == 0)
            {

            }
            if (quotas.Count == 0)
            {

                return;
            }

            var unassigned = new HashSet<Settlement>(allSettlements);
            var assignment = new Dictionary<Settlement, Faction>();

            var orderedFactions = quotas
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var faction in orderedFactions)
            {
                int need = quotas[faction];
                if (need == 0)
                    continue;

                Vector3 seed;
                var ownedOld = allSettlements.Where(s => s.Faction == faction).ToList();
                if (ownedOld.Count > 0)
                {
                    seed = Vector3.zero;
                    ownedOld.ForEach(s => seed += Find.WorldGrid.GetTileCenter(s.Tile));
                    seed /= ownedOld.Count;
                }
                else
                {
                    if (unassigned.Any())
                    {
                        seed = Find.WorldGrid.GetTileCenter(unassigned.RandomElement().Tile);
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int i = 0; i < need; i++)
                {
                    Settlement best = null;
                    float bestDist = float.MaxValue;
                    foreach (var s in unassigned)
                    {
                        float d = Vector3.Distance(seed, Find.WorldGrid.GetTileCenter(s.Tile));
                        if (d < bestDist)
                        {
                            bestDist = d;
                            best = s;
                        }
                    }

                    if (best == null)
                    {
                        break;
                    }

                    assignment[best] = faction;
                    unassigned.Remove(best);

                    Vector3 sum = Vector3.zero;
                    int count = 0;
                    foreach (var kv in assignment)
                        if (kv.Value == faction)
                        {
                            sum += Find.WorldGrid.GetTileCenter(kv.Key.Tile);
                            count++;
                        }

                    if (count > 0)
                        seed = sum / count;
                }
            }

            foreach (var kv in assignment)
                kv.Key.SetFaction(kv.Value);

            Find.World.renderer.RegenerateAllLayersNow();
        }
    }
}