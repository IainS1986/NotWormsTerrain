Shader "Terrain/WorldUV/2 Texure Blend" {
	Properties {
		_TexA ("Base (RGB)", 2D) = "white" {}
		_ScaleA ("Scale", Float) = 0.1
		_TexB ("Base (RGB)", 2D) = "white" {}
		_ScaleB ("Scale", Float) = 0.1
		
		_BlendY ("Blend Height", Float) = 20
		_BlendS ("Blend Scale", Float) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _TexA;
		sampler2D _TexB;
		
		float _ScaleA;
		float _ScaleB;
		
		float _BlendY;
		float _BlendS;

		struct Input {
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 worldUVA;
			worldUVA.x = IN.worldPos.x * _ScaleA;
			worldUVA.y = IN.worldPos.y * _ScaleA;
			
			float2 worldUVB;
			worldUVB.x = IN.worldPos.x * _ScaleB;
			worldUVB.y = IN.worldPos.y * _ScaleB;
			
			half4 cA = tex2D (_TexA, worldUVA);
			half4 cB = tex2D (_TexB, worldUVB);
			
			half4 cBlend;
			
			float minY = _BlendY - (_BlendS / 2);
			float maxY = _BlendY + (_BlendS / 2);
			
			if(IN.worldPos.y<minY) cBlend = cA;
			else if(IN.worldPos.y>maxY) cBlend = cB;
			else
			{
				float blendAmount = (IN.worldPos.y - minY) / _BlendS;
				
				cBlend = (cB * blendAmount) + (cA * (1-blendAmount));
			}
			
			o.Albedo = cBlend.rgb;
			o.Alpha = cBlend.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
