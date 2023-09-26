using Kitchen;
using KitchenMods;
using System;
using UnityEngine;

namespace NoClip {
	internal class NoClipMod : GenericSystemBase, IModSystem {

		protected override void Initialise() {
			if (UnityEngine.Object.FindObjectOfType<NoClipPatcher>() == null) {
				GameObject gameObject = new GameObject("NoClip");
				gameObject.AddComponent<NoClipPatcher>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}

		protected override void OnUpdate() {
		}
	}
}
