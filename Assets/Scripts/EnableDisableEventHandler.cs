using UnityEngine;
using UnityEngine.Events;

public class EnableDisableEventHandler : MonoBehaviour {
	public UnityEvent onEnableEvent;
	public UnityEvent onDisableEvent;
	private void OnEnable () {
		onEnableEvent.Invoke ();
	}
	private void OnDisable () {
		onDisableEvent.Invoke ();
	}
}