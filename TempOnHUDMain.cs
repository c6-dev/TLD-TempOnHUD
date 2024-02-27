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
        private static double elapsedMinutes = 0d;
        private static Color darkRed = new Color(0.8f, 0.2f, 0.23f, 1.000f);
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
                tempObject.transform.localPosition = new Vector3(sprite.width + 17, 0, 0);

                UILabel tempLabel = tempObject.AddComponent<UILabel>();
                tempLabel.text = "0°C";
                tempLabel.color = Color.white;
                tempLabel.fontStyle = FontStyle.Normal;
                tempLabel.font = GameManager.GetFontManager().GetUIFontForCharacterSet(CharacterSet.Latin);
                tempLabel.fontSize = 32;
                tempLabel.effectStyle = UILabel.Effect.Outline;
                tempLabel.effectColor = new Color(0.125f, 0.094f, 0.094f, 0.6f);
                tempLabel.effectDistance = new Vector2(1.7f, 1.7f);

            }
            else if (GameManager.GetHighResolutionTimerManager().GetElapsedMinutes() - elapsedMinutes >= 0.1d)
            {
                // update every 0.1 ingame minutes
                UILabel tempLabel = tempObject.GetComponent<UILabel>();
                if (tempLabel != null && GameManager.GetFreezingComponent() != null)
                {

                    int temp = (int)Math.Round(GameManager.GetFreezingComponent().CalculateBodyTemperature());
                    if (temp <= 0)
                    {
                        tempLabel.color = darkRed;
                    }
                    else
                    {
                        tempLabel.color = Color.white;
                    }
                    tempLabel.text = temp.ToString() + "°C";
                    elapsedMinutes = GameManager.GetHighResolutionTimerManager().GetElapsedMinutes();
                }
            }

        }
    }
}
