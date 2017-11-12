/*
* FarseerUnity based on Farseer Physics Engine port:
* Copyright (c) 2012 Gabriel Ochsenhofer https://github.com/gabstv/Farseer-Unity3D
* 
* Original source Box2D:
* Copyright (c) 2011 Ian Qvist http://farseerphysics.codeplex.com/
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/
using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

[AddComponentMenu("FarseerUnity/Dynamics/Joints/Revolute Joint Component")]
public class FSRevoluteJointComponent : FSJointComponent
{
	protected RevoluteJoint joint;
	
	// joint properties
	
	/// <summary>
	/// To alter this value at runtime, use SetLimitEnabled(value)
	/// </summary>
	public bool LimitEnabled = false;
	
	/// <summary>
	/// To alter this value at runtime, use SetLowerLimit(value)
	/// </summary>
	public float LowerLimit = -90f;
	
	/// <summary>
	/// To alter this value at runtime, use SetUpperLimit(value)
	/// </summary>
	public float UpperLimit = 90f;
	
	/// <summary>
	/// To alter this value at runtime, use SetMotorEnabled(value)
	/// </summary>
	public bool MotorEnabled = false;
	/// <summary>
	/// To alter this value at runtime, use SetMaxMotorTorque(value)
	/// </summary>
	public float MaxMotorTorque = 400f;
	/// <summary>
	/// To alter this value at runtime, use SetMotorSpeed(value)
	/// </summary>
	public float MotorSpeed = 0f;
	
	/// <summary>
	/// To alter this value at runtime, use SetLocalAnchorB(value)
	/// </summary>
	public Vector2 LocalAnchorB = Vector2.zero;
	
	public void SetLimitEnabled(bool value)
	{
		LimitEnabled = value;
		joint.LimitEnabled = LimitEnabled;
	}
	
	/// <summary>
	/// Sets the lower limit at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetLowerLimit(float value)
	{
		LowerLimit = value;
		joint.LowerLimit = LowerLimit * Mathf.Deg2Rad;
	}
	
	/// <summary>
	/// Sets the upper limit at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetUpperLimit(float value)
	{
		UpperLimit = value;
		joint.UpperLimit = UpperLimit * Mathf.Deg2Rad;
	}
	
	/// <summary>
	/// Sets the motor enabled at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetMotorEnabled(bool value)
	{
		MotorEnabled = value;
		joint.MotorEnabled = MotorEnabled;
	}
	
	/// <summary>
	/// Sets the max motor torque at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetMaxMotorTorque(float value)
	{
		MaxMotorTorque = value;
		joint.MaxMotorTorque = MaxMotorTorque;
	}
	
	/// <summary>
	/// Sets the motor speed at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetMotorSpeed(float value)
	{
		MotorSpeed = value;
		joint.MotorSpeed = MotorSpeed;
	}
	
	/// <summary>
	/// Sets the local anchor b at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetLocalAnchorB(Vector2 value)
	{
		LocalAnchorB = value;
		joint.LocalAnchorB = FSHelper.Vector2ToFVector2(value);
	}
	
	/// <summary>
	/// Sets the collide connected at runtime.
	/// </summary>
	/// <param name='value'>
	/// Value.
	/// </param>
	public void SetCollideConnected(bool value)
	{
		CollideConnected = value;
		joint.CollideConnected = CollideConnected;
	}
	
	public override void InitJoint ()
	{
		base.InitJoint ();
		//
		Vector3 p0 = BodyB.transform.TransformPoint(new Vector3(LocalAnchorB.x, LocalAnchorB.y, -5f));
		
		joint = JointFactory.CreateRevoluteJoint(FSWorldComponent.PhysicsWorld, BodyA.PhysicsBody, BodyB.PhysicsBody, BodyB.PhysicsBody.GetLocalPoint(FSHelper.Vector3ToFVector2(p0)));
		joint.CollideConnected = CollideConnected;
		joint.LowerLimit = LowerLimit * Mathf.Deg2Rad;
		joint.UpperLimit = UpperLimit * Mathf.Deg2Rad;
		joint.LimitEnabled = LimitEnabled;
		joint.MaxMotorTorque = MaxMotorTorque;
		joint.MotorSpeed = MotorSpeed;
		joint.MotorEnabled = MotorEnabled;
	}
	
	public override void OnDrawGizmos ()
	{
		base.OnDrawGizmos ();
		if(BodyA == null || BodyB == null)
			return;
		// get draw point
		Vector3 p0 = BodyB.transform.TransformPoint(new Vector3(LocalAnchorB.x, LocalAnchorB.y, -5f));
		//Vector3 p0 = FSHelper.LocalTranslatedVec3(new Vector3(LocalAnchorB.x, LocalAnchorB.y, -5f), BodyB.transform);
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(p0, 0.3f);
		// draw limits
		if(LimitEnabled)
		{
			float angLL = LowerLimit * Mathf.Deg2Rad;// - Mathf.PI;
			float angUL = UpperLimit * Mathf.Deg2Rad;// - Mathf.PI;
			float angR = BodyB.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			angLL += angR;
			angUL += angR;
			
			Vector3 pLL, pUL, pR;
			pLL = p0;
			
			pLL.x += Mathf.Cos(angLL) * 3f;
			pLL.y += Mathf.Sin(angLL) * 3f;
			
			pUL = p0;
			
			pUL.x += Mathf.Cos(angUL) * 3f;
			pUL.y += Mathf.Sin(angUL) * 3f;
			
			pR = p0;
			pR.x += Mathf.Cos(angR) * 5f;
			pR.y += Mathf.Sin(angR) * 5f;
			
			Gizmos.color = Color.green;
			Gizmos.DrawLine(p0, pLL);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(p0, pUL);
			Gizmos.color = Color.white;
			Gizmos.DrawLine(p0, pR);
		}
	}
	
	
}
