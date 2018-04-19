using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : ActivableObject
{

	private new Rigidbody rigidbody;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		rigidbody.useGravity = false;
	}

	public override void Activate()
	{
		base.Activate();
		rigidbody.useGravity = true;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
	}
}
