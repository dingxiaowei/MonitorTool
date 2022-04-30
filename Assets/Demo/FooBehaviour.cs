using System;
using UnityEngine;

class FooBehaviour : MonoBehaviour 
{
	void Start()
	{
		// AOT0003: Reflection only works for looking up existing types
		Type.GetType("");

		// UEA0002: Using string methods can lead to code that is hard to maintain
		SendMessage("");

		// UEA0006: Use of coroutines cause some allocations
		StartCoroutine("");
	}

	// UEA0001: Using OnGUI causes allocations and GC spikes
	void OnGUI()
	{

	}

	// UEA0003: Empty MonoBehaviour methods are executed and incur a small overhead
	void FixedUpdate()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		// UEA0004: Using CompareTag for tag comparison does not cause allocations
		if (other.tag == "")
		{

		}
	}

	void Update()
	{
		// UEA0005: Warning to cache the result of find in Start or Awake
		GameObject.Find("");
	}
}