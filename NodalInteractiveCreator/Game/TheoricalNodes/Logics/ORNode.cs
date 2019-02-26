using UnityEngine;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using System;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;

[Serializable]
[NodeStyle("OR", PathCategory.LOGICS, "")]
public class ORNode : TheoricalNode {
	
	[NodeConnectStyle("A", ColorStyle.WHITE, "")]
	public NodeReceiver A;

	[NodeConnectStyle("B", ColorStyle.GREEN, "")]
	public NodeReceiver B;
	
	public NodeEmitter Result;

	public bool valueA;
	public bool valueB;

	//Awake Nodal
	public override void Initialize() {
		base.Initialize();
		A.Receive = CheckA;
	   
		B.Receive = CheckB;
	}

	private void CheckA() {
		valueA = true;
		Result.Emit();
	}

	private void CheckB() {
		valueB = true;
		Result.Emit();
	}
}