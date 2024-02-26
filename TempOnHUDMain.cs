using Il2Cpp;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace TempOnHUD;

internal sealed class TempOnHUDMain : MelonMod

{

    [HarmonyPatch(typeof(StatusBar), "Update")]
    private static class TempMeter
    {
        private static void Postfix(StatusBar __instance)
        {
            if (__instance.m_StatusBarType != StatusBar.StatusBarType.Hunger) return;
            if (!__instance.m_IsOnHUD) return;

            UISprite sprite = __instance.m_OuterBoxSprite.GetComponent<UISprite>();
            GameObject spriteObject = sprite.gameObject;
            GameObject tempObject = spriteObject.transform.parent.FindChild("temperature")?.gameObject;
            if (tempObject == null)
            {
                // init
                tempObject = new GameObject("temperature");
                tempObject.transform.SetParent(spriteObject.transform.parent);
                tempObject.transform.localScale = spriteObject.transform.localScale;
                tempObject.transform.localPosition = new Vector3(sprite.width + 15, 0, 0);

                UILabel tempLabel = tempObject.AddComponent<UILabel>();
                tempLabel.text = "0°C";
                tempLabel.color = Color.white;
                tempLabel.fontStyle = FontStyle.Normal;
                tempLabel.font = GameManager.GetFontManager().GetUIFontForCharacterSet(CharacterSet.Latin);
                tempLabel.fontSize = 24;
                tempLabel.width = sprite.width;
                tempLabel.height = sprite.height;
            } else
            {
                // update
                UILabel tempLabel = tempObject.GetComponent<UILabel>();
                if (tempLabel)
                {

                    int temp = (int)Math.Round(GameManager.GetFreezingComponent().CalculateBodyTemperature());
                    tempLabel.text = temp.ToString() + "°C"; ;
                }
            }

        }
    }
}
