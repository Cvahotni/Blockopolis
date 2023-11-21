using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoadAttribute]
#endif
public static class PlayStateNotifier
{
	static PlayStateNotifier() {
		#if UNITY_EDITOR
		EditorApplication.playmodeStateChanged += ModeChanged;
		#endif
	}

	static void ModeChanged() {
		#if UNITY_EDITOR

		if (!EditorApplication.isPlayingOrWillChangePlaymode) {
			BlockRegistry.OnDestroy();
		}

		#endif
	}
}