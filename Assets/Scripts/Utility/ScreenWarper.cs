using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWarper : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float horizontalSize = 71;
	[SerializeField] private float verticalSize = 40;
	[SerializeField] private bool rotateToCenter;
	[SerializeField] private int framesToWaitBeforeWarp = 5;

	private bool hasWarpedHoriz;
	private bool hasWarpedVert;

	private void FixedUpdate()
	{
		if (!hasWarpedHoriz && Mathf.Abs(transform.position.x) > horizontalSize)
		{
			transform.position = new Vector3(-transform.position.x, 0, transform.position.z);
			hasWarpedHoriz = true;

			// skipa alguns frames antes de fzr warp pra evitar um bug de ficar teleportando varias vezes
			StartCoroutine(DoAfterFrames(() => hasWarpedHoriz = false));
			RotateToCenter();
		}
		
		if (!hasWarpedVert && Mathf.Abs(transform.position.z) > verticalSize)
		{
			transform.position = new Vector3(transform.position.x, 0, -transform.position.z);
			hasWarpedVert = true;

			// skipa alguns frames antes de fzr warp pra evitar um bug de ficar teleportando varias vezes
			StartCoroutine(DoAfterFrames(() => hasWarpedVert = false));
			RotateToCenter();
		}
	}

	private IEnumerator DoAfterFrames(Action action)
	{
		for (int i = 0; i < framesToWaitBeforeWarp; i++)
			yield return null;

		action?.Invoke();
	}

	private void RotateToCenter()
	{
		// rotaciona pro centro pros inimigos não travarem num warp infinito

		if (!rotateToCenter) return;
		transform.LookAt(Vector3.zero);
	}
}
