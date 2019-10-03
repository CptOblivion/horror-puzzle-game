using UnityEngine;

namespace Plawius
{
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Plawius Screen Space Reflections (SSR)")]
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
#if UNITY_5_4_OR_NEWER
	[ImageEffectAllowedInSceneView]
#endif
public class SSREffect : MonoBehaviour
{
	static class Uniforms
	{
		internal static readonly int _ProjInfo			= Shader.PropertyToID("_ProjInfo");
		internal static readonly int _ProjMatrix		= Shader.PropertyToID("_ProjMatrix");
		internal static readonly int _Cutoff_Start		= Shader.PropertyToID("_Cutoff_Start");
		internal static readonly int _Cutoff_End		= Shader.PropertyToID("_Cutoff_End");
		internal static readonly int _FresnelStart		= Shader.PropertyToID("_FresnelStart");
		internal static readonly int _FaceViewerFactor	= Shader.PropertyToID("_FaceViewerFactor");
		internal static readonly int _LinearStepK		= Shader.PropertyToID("_LinearStepK");
		internal static readonly int _Bias				= Shader.PropertyToID("_Bias");
		internal static readonly int _MaxIter			= Shader.PropertyToID("_MaxIter");
		internal static readonly int _Original			= Shader.PropertyToID("_Original");
	}
	
	SSRSettings m_settings;
	protected SSRSettings settings {
		get {
			if (m_settings == null)
			{
				m_settings = (SSRSettings)FindObjectOfType(typeof(SSRSettings));
				if (m_settings == null)
					m_settings = (new GameObject("SSR_Settings")).AddComponent<SSRSettings>();
			}
			return m_settings;
		} 
	}

	[HideInInspector]
	[SerializeField]
	public Shader blurShader = null;	

	[HideInInspector]
	[SerializeField]
	public Shader ssrShader = null;	
	
	Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {

				m_Material = new Material(ssrShader);
				
				if (prevRenderingPath == RenderingPath.DeferredLighting)
					m_Material.EnableKeyword("LEGACY_DEFERRED");
				else
					m_Material.DisableKeyword("LEGACY_DEFERRED");

				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		} 
	}

	Material m_MaterialBlur = null;
	protected Material materialBlur {
		get {
			if (m_MaterialBlur == null) {
				m_MaterialBlur = new Material(blurShader);
				m_MaterialBlur.hideFlags = HideFlags.DontSave;
			}
			return m_MaterialBlur;
		} 
	}

	Camera m_Camera = null;
	protected Camera thisCamera {
		get {
			if (m_Camera == null) {
				m_Camera = GetComponent<Camera>();
			}
			return m_Camera;
		} 
	}

	private RenderingPath prevRenderingPath;

	RenderingPath CurrentRenderingPath()
	{
		return thisCamera.actualRenderingPath;
	}

	void OnEnable ()
	{
		SSRSettings s = settings;
		if (s == null)
		{
			Debug.LogError("[SSR] SSRSettings is missing");
		}

		prevRenderingPath = CurrentRenderingPath();

		thisCamera.depthTextureMode |= DepthTextureMode.Depth;

		var renderingPath = prevRenderingPath;
		if (renderingPath == RenderingPath.DeferredLighting)
		{
			thisCamera.depthTextureMode |= DepthTextureMode.DepthNormals;

			Debug.LogError("[SSR] Working in legacy mode (Deferred). Unity 5 PBR won't work with SSR!");
		}
		else 
		{
			if (renderingPath != RenderingPath.DeferredShading)
			{
				//thisCamera.renderingPath = RenderingPath.DeferredShading;
				//Debug.Log("[SSR] Camera's rendering path changed to Deferred Shading");
				
				this.enabled = false;
				Debug.LogWarning("[SSR] Camera's rendering path is not Deferred, it's " + renderingPath + ". Disabling SSR");
			}
		}

		if (ssrShader == null)
			ssrShader = Shader.Find ("Hidden/PlawiusSSR");
		if (ssrShader == null)
		{
			enabled = false;
			Debug.LogError("[SSR] Please, import PlawiusSSR shader, I cannot find it");
			return;
		}

		if (blurShader == null)
			blurShader = Shader.Find ("Hidden/BlurEffectConeTap");
		if (blurShader == null)
		{
			enabled = false;
			Debug.LogError("[SSR] Please, import BlurEffectConeTap shader from Standard Unity pack");
			return;
		}
		
		if (!ssrShader || !material.shader.isSupported) {
			enabled = false;
			Debug.Log("[SSR] Screen space reflections is not supported on your videocard");
			return;
		}

		if (!blurShader || !materialBlur.shader.isSupported) {
			enabled = false;
			Debug.Log("[SSR] Blur shader is not supported on your videocard");
			return;
		}
	}
	
	protected void OnDisable() {
		if( m_Material ) {
			DestroyImmediate( m_Material );
			m_Material = null;
		}
		if( m_MaterialBlur ) {
			DestroyImmediate( m_MaterialBlur );
			m_MaterialBlur = null;
		}
		prevRenderingPath = RenderingPath.VertexLit;
	}	
	
	// --------------------------------------------------------
	
	protected void Start()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			Debug.Log("[SSR] Image Effects is not supported");
			return;
		}
	}

	// Performs one blur iteration.
	public void FourTapCone (RenderTexture source, RenderTexture dest, int iteration)
	{
		float off = 0.5f + iteration * settings.blurSpread;
		Graphics.BlitMultiTap (source, dest, materialBlur,
							   new Vector2(-off, -off),
							   new Vector2(-off,  off),
							   new Vector2( off,  off),
							   new Vector2( off, -off)
							   );
	}
	
	// Downsamples the texture to a quarter resolution.
	private void DownSample4x (RenderTexture source, RenderTexture dest)
	{
		float off = 1.0f;
		Graphics.BlitMultiTap (source, dest, materialBlur,
							   new Vector2(-off, -off),
							   new Vector2(-off,  off),
							   new Vector2( off,  off),
							   new Vector2( off, -off)
							   );
	}

	
	// Called by the camera to apply the image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		#if UNITY_EDITOR
		if (Application.isEditor)
		{
			if (prevRenderingPath != CurrentRenderingPath())
			{
				this.enabled = false;
				this.enabled = true;
			}
		}
		#endif

		Matrix4x4 P = thisCamera.projectionMatrix; 

		bool d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
		
		if (d3d)	
		{
			// Scale and bias from OpenGL -> D3D depth range
			for (int i = 0; i < 4; i++) 	
			{
				P[2,i] = P[2,i]*0.5f + P[3,i]*0.5f;	
			}	
		}

		var width = thisCamera.pixelWidth;
		var height = thisCamera.pixelHeight;

		var projInfo = new Vector4(
				-2.0f / (width * P[0]),
				-2.0f / (height * P[5]),
				(1.0f - P[2]) / P[0],
				(1.0f + P[6]) / P[5]
				);

		this.material.SetVector (Uniforms._ProjInfo, projInfo); 
		this.material.SetMatrix (Uniforms._ProjMatrix,P);

		if (settings.cutOffStart >= settings.cutOffEnd)
		{
			settings.cutOffStart = settings.cutOffEnd;
		}

		this.material.SetFloat (Uniforms._Cutoff_Start, settings.cutOffStart);
		this.material.SetFloat (Uniforms._Cutoff_End, settings.cutOffEnd);

		this.material.SetFloat (Uniforms._FresnelStart, settings.fresnelFactorStart);
		this.material.SetFloat (Uniforms._FaceViewerFactor, settings.faceViewerFactor);

		this.material.SetFloat (Uniforms._LinearStepK, settings.linearCoefficient);
		this.material.SetFloat (Uniforms._Bias, settings.zBias);
		this.material.SetInt (Uniforms._MaxIter, settings.maxRaymarchIterations);
		// ---

		int rtW = source.width;
		int rtH = source.height;

		if (settings.iterations == 0 && settings.downscale <= 1)
		{
			var pass = 2;
			
			#if UNITY_EDITOR
			if (settings.showOnlyReflections)
				pass = 0;
			#endif
			
			Graphics.Blit(source, destination, this.material, pass);
		}
		else
		{
			#if UNITY_5_6_OR_NEWER
			bool allowHdr = thisCamera.allowHDR;
			#else
			bool allowHdr = thisCamera.hdr;
			#endif

			var fbFormat = allowHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

			RenderTexture blurredMain2 = RenderTexture.GetTemporary(rtW , rtH, 0, fbFormat);
			RenderTexture blurredMain = RenderTexture.GetTemporary(rtW / settings.downscale, rtH / settings.downscale, 0, fbFormat);

			Graphics.Blit(source, blurredMain2, this.material, 0);
			//Copy source to the 4x4 smaller texture.
			DownSample4x (blurredMain2, blurredMain);
			RenderTexture.ReleaseTemporary(blurredMain2);

			//Blur the small texture
			for(int i = 0; i < settings.iterations; i++)
			{
				RenderTexture buffer2 = RenderTexture.GetTemporary(rtW / settings.downscale, rtH / settings.downscale, 0, fbFormat);
				FourTapCone (blurredMain, buffer2, i);
				RenderTexture.ReleaseTemporary(blurredMain);
				blurredMain = buffer2;
			}

			#if UNITY_EDITOR
			if (settings.showOnlyReflections)
			{
				Graphics.Blit(blurredMain, destination);
			}
			else
			#endif
			{
				this.material.SetTexture (Uniforms._Original, source);
				Graphics.Blit(blurredMain, destination, this.material, 1);
			}

			RenderTexture.ReleaseTemporary(blurredMain);
		}

	}	
}
}