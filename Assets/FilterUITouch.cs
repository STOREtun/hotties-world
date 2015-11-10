using UnityEngine;
using System.Collections;

using TouchScript.Gestures;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TouchScript;

// original idea for integrating NGUI and TouchScript came from here:
// LINK https://github.com/InteractiveLab/TouchScript/issues/6
public class FilteruUITouch : MonoBehaviour {
    void Start() {
        //Assert.NotNull(TouchScript.TouchManager.Instance);
        TouchScript.TouchManager.Instance.TouchesBegan += TouchManagerBegan;
        TouchScript.TouchManager.Instance.TouchesMoved += TouchManagerMoved;
        TouchScript.TouchManager.Instance.TouchesEnded += TouchManagerEnded;
        TouchScript.TouchManager.Instance.TouchesCancelled += TouchManagerCancelled;
    }

    private void TouchManagerBegan(object sender, TouchEventArgs eventArgs) {
        TouchManagerChanged(sender, eventArgs, TouchPhase.Began);
    }

    private void TouchManagerMoved(object sender, TouchEventArgs eventArgs) {
        TouchManagerChanged(sender, eventArgs, TouchPhase.Moved);
    }

    private void TouchManagerEnded(object sender, TouchEventArgs eventArgs) {
        TouchManagerChanged(sender, eventArgs, TouchPhase.Ended);
    }

    private void TouchManagerCancelled(object sender, TouchEventArgs eventArgs) {
        TouchManagerChanged(sender, eventArgs, TouchPhase.Canceled);
    }

    private void TouchManagerChanged(object sender, TouchEventArgs eventArgs, TouchPhase touchPhase) {
        if (touchPhase != TouchPhase.Began)
            return;

        TouchScript.TouchManager gesture = sender as TouchScript.TouchManager;

        // to make sure this works for mouse input and touch-devices, starting from -1 (mouse) loop through  
        // each 'touch' in Input.touches and pass in the id to 'IsPointerOverEventSystemObject'
        // LINK http://forum.unity3d.com/threads/ispointerovereventsystemobject-always-returns-false-on-mobile.265372/
        for (int i = -1, il = Input.touches.Length; i < il; i++) {
            if (EventSystem.current.IsPointerOverGameObject(i)) {
                Debug.Log("jkasdjkjkasdjkasdasd");
            }
            //if (EventSystemManager.currentSystem.IsPointerOverEventSystemObject(i)) {
            //    foreach (TouchScript.TouchPoint touchPoint in eventArgs.TouchPoints)
            //        gesture.CancelTouch(touchPoint.Id);
            //    return;
            //}
        }
    }
}