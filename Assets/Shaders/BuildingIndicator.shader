// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "AT_RPG/Building/Building_Indicator"
{
    Properties
    {
        _BuildingAlpha ("BuildingAlpha", Range(0, 1)) = 0.5
        _BuildingStatusColor ("BuildingStatusColor", color) = (0, 0, 0, 0)
        _BuildingTex ("BuildingTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        // 건물
        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _BuildingAlpha;
            float4 _BuildingStatusColor;
            sampler2D _BuildingTex;
            float4 _BuildingTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 color = _BuildingStatusColor;
                color.w = _BuildingAlpha;

                return color;
            }
            ENDCG
        }
    }
}
