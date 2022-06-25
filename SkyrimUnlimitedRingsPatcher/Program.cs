using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace SkyrimUnlimitedRingsPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var RingIconKwd = state.PatchMod.Keywords.AddNew();
            RingIconKwd.EditorID = "UnlimitedRing";
            var AmuletIconKwd = state.PatchMod.Keywords.AddNew();
            AmuletIconKwd.EditorID = "UnlimitedAmulet";

            foreach (var armorGetter in state.LoadOrder.PriorityOrder.Armor().WinningOverrides())
            {
                if (armorGetter.FormKey == Dragonborn.Armor.DLC2DummyHelmet.FormKey) continue;

                var getterFlags = armorGetter.BodyTemplate?.FirstPersonFlags;

                if (getterFlags != null)
                {
                    if ((getterFlags & BipedObjectFlag.Ring) == BipedObjectFlag.Ring)
                    {
                        var ring = state.PatchMod.Armors.GetOrAddAsOverride(armorGetter);
                        if (ring.BodyTemplate != null)
                        {
                            ring.BodyTemplate.FirstPersonFlags = ring.BodyTemplate.FirstPersonFlags & ~BipedObjectFlag.Ring;
                        }
                        ring.Keywords?.Add(RingIconKwd);
                    }
                    if ((getterFlags & BipedObjectFlag.Amulet) == BipedObjectFlag.Amulet)
                    {
                        var amulet = state.PatchMod.Armors.GetOrAddAsOverride(armorGetter);
                        if (amulet.BodyTemplate != null)
                        {
                            amulet.BodyTemplate.FirstPersonFlags = amulet.BodyTemplate.FirstPersonFlags & ~BipedObjectFlag.Amulet;
                        }
                        amulet.Keywords?.Add(AmuletIconKwd);
                    }
                }

            }
        }
    }
}
