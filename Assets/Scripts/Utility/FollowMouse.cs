using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Canvas parentCanvas;

	private void Update()
	{
		if (GameManager.IsGameOver) return;

		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out pos);
		transform.position = parentCanvas.transform.TransformPoint(pos);
	}
}
