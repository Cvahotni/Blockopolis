using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoadAttribute]
#endif
public static class PlayStateNotifier
{
	static PlayStateNotifier() {
		#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += ModeChanged;
		#endif
	}

	#if UNITY_EDITOR
	static void ModeChanged(PlayModeStateChange state) {
		if (state == PlayModeStateChange.ExitingPlayMode) {
			BlockRegistry.OnDestroy();
			FeatureRegistry.OnDestroy();
		}
	}
	#endif
}