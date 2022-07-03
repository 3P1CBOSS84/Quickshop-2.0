using UnityEngine;
using Harmony12;
using System;
using System.Reflection;
using UnityModManagerNet;

namespace QuickShop
{
    internal static class Main
    {
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Main.mod = modEntry;
            modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
            HarmonyInstance harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Main.enabled = value;
            modEntry.Logger.Log("Starting QuickShop");
            return true;
        }
        public static bool enabled;

        public static UnityModManager.ModEntry mod;
    }

    [HarmonyPatch(typeof(GameScript))]
    [HarmonyPatch("Update")]
    public class RotationInput_Update_Patcher
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            bool keyUp = Input.GetKeyUp(KeyCode.B);
            if (keyUp)
            {
                GameScript.Get().GetSelectedPartToMount();
                string idwithTuned = GameScript.Get().GetPartMouseOver().GetIDWithTuned();
                idwithTuned.Replace("1", "2");
                bool flag = idwithTuned != null;
                if (flag)
                {
                    Main.mod.Logger.Log(idwithTuned);
                    Inventory.Get().Add(idwithTuned, 1f, Color.black, true, true);
                    new NewInventoryItem(idwithTuned, true).Condition = 1f;
                    int price = Singleton<GameInventory>.Instance.GetItemProperty(idwithTuned).Price;
                    GlobalData.AddPlayerMoney(-price);
                    UIManager.Get().ShowPopup("QuickShop Mod:", "Part cost: " + Helper.MoneyToString((float)price), PopupType.Buy);
                }
            }
            // Add a key that purchases tuned parts.
            if (Input.GetKeyUp(KeyCode.N))
            {
                GameScript.Get().GetSelectedPartToMount();
                string idwithTuned2 = "t_" + GameScript.Get().GetPartMouseOver().GetIDWithTuned();
                idwithTuned2.Replace("1", "2");
                bool flag = idwithTuned2 != null;
                if (flag)
                {
                    Main.mod.Logger.Log(idwithTuned2);
                    Inventory.Get().Add(idwithTuned2, 1f, Color.black, true, true);
                    new NewInventoryItem(idwithTuned2, true).Condition = 1f;
                    int price = Singleton<GameInventory>.Instance.GetItemProperty(idwithTuned2).Price;
                    GlobalData.AddPlayerMoney(-price);
                    UIManager.Get().ShowPopup("QuickShop Mod:", "Part cost: " + Helper.MoneyToString((float)price), PopupType.Buy);
                }
            }
        }
    }
}
