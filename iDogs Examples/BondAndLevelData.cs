using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondAndLevelData : MonoBehaviour
{
	#region Properties
	public int BondWithPlayer;
	public int PlayerLevel;
	#endregion

	#region Unity Functions
	void Start ()
	{
		BondWithPlayer = 0;
		PlayerLevel = 1;
	}
	#endregion
}
