using HarmonyLib;
using HaulersHelper.Util;

namespace HaulersHelper;

[HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
internal static class FetchVagonPiecesObjectDB
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(ObjectDB __instance)
    {
        if (__instance.GetItemPrefab("Hammer")?.GetComponent<ItemDrop>()?.m_itemData.m_shared.m_buildPieces is { } pieces)
        {
            Functions.GetThemWagons(pieces.m_pieces);
        }
    }
}

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
internal static class FetchVagonPiecesZNetScene
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(ZNetScene __instance)
    {
        Functions.GetThemWagons(__instance.m_prefabs);
    }
}

[HarmonyPatch(typeof(Vagon),nameof(Vagon.Awake))]
internal static class VagonAwakePatch
{
    static void Postfix(Vagon __instance)
    {
        string prefabName = Utils.GetPrefabName(__instance.transform.root.gameObject);
        if (HaulersHelperPlugin.Wagons.Contains(prefabName))
        {
            Functions.FixDuhWagons();
        }
    }
}