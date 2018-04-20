using System;
using System.Collections;
using UnityEngine;

public class Lever : PullableObject
{

	[SerializeField] private ActivableObject activableObject;
	private Animator animator;
	private bool activatedOnce;

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("Pull") &&
		    Math.Abs(animator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1f) < 0.1f &&
		!activatedOnce)
		{
			if (activableObject.activated)
				activableObject.Deactivate();
			else
				activableObject.Activate();
			activatedOnce = true;
		}
	}

	public override void Pull()
	{
		base.Pull();
		if(animator != null)
			animator.SetBool("isPulling", isPulling);
	}

	public override void Detach()
	{
		base.Detach();
		if(animator != null)
			animator.SetBool("isPulling", isPulling);
		activatedOnce = false;
	}
}
