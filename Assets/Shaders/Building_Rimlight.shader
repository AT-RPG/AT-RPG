Shader "AT_RPG/Building/Rimlight"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BumpTex("Normal Texture", 2D) = "bump" {}
		_Color("Rim Color", COLOR) = (1, 1, 1, 1)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue"= "Transparent" }
		LOD 100

		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;

				float3 T : TEXCOORD1;
				float3 B : TEXCOORD2;
				float3 N : TEXCOORD3;

				float3 lightDir : TEXCOORD4;
				float3 viewDir : TEXCOORD5;		
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _BumpTex;
			float4 _BumpTex_ST;

			float4 _Color;



			void Fuc_LocalNormal2TBN(float3 localnormal, float4 tangent, inout float3 T, inout float3  B, inout float3 N)
			{
				float3 fTangentSign = tangent.w * unity_WorldTransformParams.w;
				N = normalize(UnityObjectToWorldNormal(localnormal));
				T = normalize(UnityObjectToWorldDir(tangent.xyz));
				B = normalize(cross(N, T) * fTangentSign);
			}

			float3 Fuc_TangentNormal2WorldNormal(float3 fTangnetNormal, float3 T, float3  B, float3 N)
			{
				float3x3 TBN = float3x3(T, B, N);
				TBN = transpose(TBN);
				return mul(TBN, fTangnetNormal);
			}


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.lightDir = WorldSpaceLightDir(v.vertex);
				o.viewDir = normalize(WorldSpaceViewDir(v.vertex));

				Fuc_LocalNormal2TBN(v.normal, v.tangent, o.T, o.B, o.N);

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				
				float4 col = tex2D(_MainTex, i.uv);
				float3 fTangnetNormal = UnpackNormal(tex2D(_BumpTex, i.uv * _BumpTex_ST.rg));
				float3 worldNormal = Fuc_TangentNormal2WorldNormal(fTangnetNormal, i.T, i.B, i.N) * 2 - 1;

				float rim = saturate(dot(worldNormal, i.viewDir));
				rim = 1 - rim;
				col.a = rim;
				col.rgb = _Color.rgb;

				return col;
			}
			ENDCG
		}
	}
}