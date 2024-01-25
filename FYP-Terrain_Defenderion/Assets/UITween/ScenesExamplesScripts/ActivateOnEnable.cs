using UnityEngine;
using System.Collections;

public class ActivateOnEnable : MonoBehaviour {

	public EasyTween EasyTweenStart;
	[SerializeField] private float delayTime = 0f;
	private IEnumerator Start () 
	{
		yield return new WaitForEndOfFrame();
		if(delayTime > 0f) { yield return new WaitForSeconds(delayTime); }
		EasyTweenStart.OpenCloseObjectAnimation();
	}
}
