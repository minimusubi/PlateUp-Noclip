using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace NoClip {
	public class NoClipPatcher : MonoBehaviour {
		private readonly Harmony harmony = new Harmony("musubi.plateup.noclip");

		private void Awake() {
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
