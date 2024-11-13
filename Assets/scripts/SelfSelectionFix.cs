using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class SelfSelectionFix : MonoBehaviour
{
	private List<HandGrabInteractable> grabs;
	private bool active = true;

	[SerializeField] private List<GameObject> ins;
	
	void Start()
	{
		// foreach (Transform child in gameObject.transform)
		// {
		// 	if (child.gameObject.name.Contains("SnapPosition"))
		// 		ins.Add(child.gameObject);
		// }
	}

	// Update is called once per frame
	void Update()
	{
		if ((GetComponentsInChildren<HandGrabInteractable>()[0].State == InteractableState.Select || GetComponentsInChildren<HandGrabInteractable>()[1].State == InteractableState.Select) && active == true)
		{
			foreach (var input in ins)
				input.SetActive(false);
			active = false;
		}
		else if (GetComponentsInChildren<HandGrabInteractable>()[0].State == InteractableState.Normal && GetComponentsInChildren<HandGrabInteractable>()[1].State == InteractableState.Normal && active == false) { 
			StartCoroutine(SetAct()); }
	}

	private IEnumerator SetAct()
	{
		yield return new WaitForSeconds(1);
		foreach (var input in ins)
			input.SetActive(true);

		active = true;
	}
}
