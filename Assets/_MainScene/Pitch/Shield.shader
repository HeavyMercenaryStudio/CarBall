Shader "Custom/Shield" {
	Properties {
		_ImpactColor ("Impact Color", Color) = (1,1,1,1)
		_Color("Base Color", Color) = (1,1,1,1)
		_Position("World Position", Vector) = (0,0,0,0)
		_Radius("Radius", Range(0,100)) = 0
		_Softness("Sphere Softness", Range(0,100)) = 0
		_EffectTime("Effect Time", Range(0,1)) = 0
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert noshadow alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float3 worldPos;
		};

		fixed4 _ImpactColor;
		fixed4 _Color;
		float4 _Position;
		half _Radius;
		half _Softness;
		half _EffectTime;

		void surf (Input IN, inout SurfaceOutput o) {
			
			half d = distance(_Position, IN.worldPos);
			half sum = saturate((d - _Radius * _EffectTime) / -_Softness);
			fixed4 lerpColor = lerp(_Color, _ImpactColor, sum);
			
			o.Albedo = lerpColor.rgb;
			o.Emission = lerpColor.rgb;
			o.Alpha = lerpColor.a;
		}
		ENDCG
	}
	FallBack "Standard"
}
