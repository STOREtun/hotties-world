using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class MobileNativePopupsPostProcessor {

	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS) {
			if (PlayerSettings.iOS.sdkVersion == iOSSdkVersion.SimulatorSDK) {
				Debug.LogWarning("MobileNativePlugins: Please do all the testing on a real device. Plugins in general do not work in the Simulator.");
			}
		}
	}
}


