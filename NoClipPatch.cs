using System.Collections.Generic;
using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace NoClip {
	internal class PlayerClipData {
		public GameObject LastObject = null;
		public bool HasChanged = false;
	}


	[HarmonyPatch(typeof(PlayerView), "Update")]
	internal class NoClipPatch {
		private static bool? PrepGhostInstalled = null;
		private static SceneType LastScene = SceneType.Null;
		private static bool WasPrepTime = false;
		private static bool NoClipEnabled = false;
		private static readonly Dictionary<PlayerView, PlayerClipData> PlayerDataDict = new();
		private static bool InputConsumed = false;

		[HarmonyPrefix]
		public static void Update_CheckPrepState(bool ___IsMyPlayer, PlayerView __instance) {
			if (!___IsMyPlayer) {
				return;
			}

			PlayerClipData PlayerData = PlayerDataDict.GetValueOrDefault(__instance);

			if (PlayerData == null) {
				PlayerData = new PlayerClipData();
				PlayerDataDict[__instance] = PlayerData;
			}

			if (GameInfo.CurrentScene != LastScene) {
				//if (GameInfo.CurrentScene != SceneType.Kitchen) {
				SetEnabled(false);
				//}

				LastScene = GameInfo.CurrentScene;
			}

			if (GameInfo.IsPreparationTime != WasPrepTime) {
				//if (GameInfo.IsPreparationTime) {
				//	if (IsPrepGhostInstalled()) {
				//		SetEnabled(false);
				//	} else {
				//		//SetEnabled(true);
				//	}
				//} else {
				SetEnabled(false);
				//}

				WasPrepTime = GameInfo.IsPreparationTime;
			}

			if (__instance.gameObject != PlayerData.LastObject) {
				Debug.Log(typeof(NoClipPatch).Name + ": GameObject changed");
				PlayerData.LastObject = __instance.gameObject;
				PlayerData.HasChanged = true;
			}

			if (!ShouldIgnore()) {
				ProcessInput();
			}

			if (PlayerData.HasChanged) {
				ModifyColliders(__instance);

				PlayerData.HasChanged = false;
			}
		}

		private static bool IsPrepGhostInstalled() {
			if (PrepGhostInstalled == null) {
				PrepGhostInstalled = KitchenMods.ModPreload.Mods.Exists(mod => {
					return mod.Name == "PlateupPrepGhost";
				});
			}

			return (bool) PrepGhostInstalled;
		}

		private static bool ShouldIgnore() {
			if (IsPrepGhostInstalled()) {
				if (GameInfo.CurrentScene == SceneType.Kitchen && GameInfo.IsPreparationTime) {
					return true;
				}
			}

			return false;
		}

		private static void ProcessInput() {
			if (Input.GetKeyDown(KeyCode.C) && !InputConsumed) {
				Debug.Log(typeof(NoClipPatch).Name + ": Pressed C");
				NoClipEnabled = !NoClipEnabled;

				foreach (PlayerClipData ClipData in PlayerDataDict.Values) {
					ClipData.HasChanged = true;
				}

				InputConsumed = true;
			} else {
				InputConsumed = false;
			}
		}

		private static void SetEnabled(bool Enabled) {
			if (Enabled != NoClipEnabled) {
				Debug.Log(typeof(NoClipPatch).Name + ": Auto " + (Enabled ? "enable" : "disable"));

				NoClipEnabled = Enabled;
				foreach (PlayerClipData ClipData in PlayerDataDict.Values) {
					ClipData.HasChanged = true;
				}
			}
		}

		private static void ModifyColliders(PlayerView playerView) {
			Debug.Log(typeof(NoClipPatch).Name + ": " + (NoClipEnabled ? "Disabling" : "Enabling") + " colliders");

			Collider[] colliders = playerView.gameObject.GetComponents<Collider>();

			foreach (Collider collider in colliders) {
				collider.enabled = !NoClipEnabled;
			}
		}
	}
}
