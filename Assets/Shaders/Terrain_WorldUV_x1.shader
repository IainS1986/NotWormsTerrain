Shader "Terrain/WorldUV/1 Texure" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Scale ("Scale", Float) = 0.001
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Scale;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 worldUV;
			worldUV.x = IN.worldPos.x * _Scale;
			worldUV.y = IN.worldPos.y * _Scale;
			half4 c = tex2D (_MainTex, worldUV);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
