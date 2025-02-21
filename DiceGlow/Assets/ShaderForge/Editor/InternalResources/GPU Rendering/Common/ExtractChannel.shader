Shader "Hidden/Shader Forge/ExtractChannel"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Channel ("Channel", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _Channel;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID  // inserted by FixShadersRightEye.cs
			};

			struct v2f{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO  // inserted by FixShadersRightEye.cs
			};

			sampler2D _MainTex;
			
			v2f vert (appdata v){
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);  // inserted by FixShadersRightEye.cs
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);  // inserted by FixShadersRightEye.cs
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float sc = tex2D(_MainTex, i.uv)[_Channel];		
				return float4(sc,sc,sc,sc);
			}

			ENDCG
		}
	}
}
