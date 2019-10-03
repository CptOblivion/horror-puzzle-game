using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Plawius
{
[CustomEditor(typeof(SSREffect))]
public class SSREditor : Editor
{
	SSRSettings m_settings;
	protected SSRSettings settings {
		get {
			if (m_settings == null)
			{
				m_settings = (SSRSettings)FindObjectOfType(typeof(SSRSettings));
			}
			return m_settings;
		} 
	}
	
	SerializedObject obj;
	SerializedProperty downscale, 
					   iterations, 
					   blurSpread, 
					   cutOffStart, 
					   cutOffEnd, 
					   fresnelFactorStart, 
					   faceViewerFactor,
					   linearCoefficient,
					   zBias,
					   maxRaymarchIterations,
					   showOnlyReflections;
	void OnEnable()
	{
		obj = new UnityEditor.SerializedObject(settings);
		downscale = obj.FindProperty("downscale");
		iterations = obj.FindProperty("iterations");
		blurSpread = obj.FindProperty("blurSpread");
		
		cutOffStart = obj.FindProperty("cutOffStart");
		cutOffEnd = obj.FindProperty("cutOffEnd");
		fresnelFactorStart = obj.FindProperty("fresnelFactorStart");
		faceViewerFactor = obj.FindProperty("faceViewerFactor");
		
		linearCoefficient = obj.FindProperty("linearCoefficient");
		zBias = obj.FindProperty("zBias");
		maxRaymarchIterations = obj.FindProperty("maxRaymarchIterations");

		showOnlyReflections = obj.FindProperty("showOnlyReflections");
	}
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		
		obj.Update();
		serializedObject.Update();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.LowerCenter};
		EditorGUILayout.LabelField("Plawius Screen Space Reflections", style, GUILayout.ExpandWidth(true));

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Blur", EditorStyles.boldLabel);
		
		EditorGUILayout.IntSlider(downscale, 1, 4, new GUIContent("Downscale", "tooltip"));
		EditorGUILayout.IntSlider(iterations, 0, 8, new GUIContent("Iterations", "Number of iterations in one pass"));
		EditorGUILayout.Slider(blurSpread, 0.1f, 1.0f, "Blur Spread");

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Fade factors", EditorStyles.boldLabel);

		EditorGUILayout.Slider(cutOffStart, -1.0f, 1.0f, new GUIContent("Cut-off start", "Cut-off start"));
		EditorGUILayout.Slider(cutOffEnd, -1.0f, 1.0f, new GUIContent("Cut-off end", "Cut-off end"));
		EditorGUILayout.Slider(fresnelFactorStart, 0.0f, 1.0f, new GUIContent("Fresnel Factor", "Fresnel Factor R0"));
		EditorGUILayout.Slider(faceViewerFactor, 0.0f, 1.0f, new GUIContent("Face Viewer Factor", "Direction fade amount"));
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Tweak these to fix artifacts", EditorStyles.boldLabel);

		EditorGUILayout.Slider(linearCoefficient, 1.0f, 100.0f, new GUIContent("Linear Coefficient", "Raymarch step size"));
		EditorGUILayout.Slider(zBias, 0.0f, 0.2f, new GUIContent("Z bias", "Z difference for collision detection during raymarching"));

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Try to keep this number small", EditorStyles.boldLabel);
		EditorGUILayout.IntSlider(maxRaymarchIterations, 8, 256, new GUIContent("Max Raymarch Iterations", "Raymarching iterations. The more you set, the better result will be (and the slower perf)"));

		EditorGUILayout.PropertyField(showOnlyReflections, new GUIContent("Show only reflections", "Debug option to see only your reflections. Editor only"));

		obj.ApplyModifiedProperties();
		serializedObject.ApplyModifiedProperties();
	}
}
}