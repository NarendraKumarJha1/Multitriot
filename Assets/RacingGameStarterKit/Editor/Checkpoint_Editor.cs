﻿// using System.Collections;
// using RGSK;
// using UnityEditor;
// using UnityEngine;


// [CustomEditor (typeof (Checkpoint)), CanEditMultipleObjects]
// public class Checkpoint_Editor : Editor {

// 	Checkpoint m_target;

// 	public void OnEnable () {
// 		m_target = (Checkpoint) target;
// 	}

// 	public override void OnInspectorGUI () {

// 		GUILayout.BeginVertical ("Box");
// 		GUILayout.Box ("Checkpoint Settings", EditorStyles.boldLabel);
// 		EditorGUILayout.Space ();

// 		m_target.checkpointType = (Checkpoint.CheckpointType) EditorGUILayout.EnumPopup ("Checkpoint Type", m_target.checkpointType);

// 		if (m_target.checkpointType == Checkpoint.CheckpointType.TimeCheckpoint) {
// 			m_target.timeToAdd = EditorGUILayout.FloatField ("Time To Add", m_target.timeToAdd);
// 		}

// 		GUILayout.EndVertical ();

// 	}
// }