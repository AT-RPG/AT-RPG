
Shader "AT_RPG/Building/Dissolve"
{
    Properties
    {
        [Header(Building Option)]
        [Space(10)]
        _MainTex ("MainTexture", 2D) = "white" {}
        
        [Header(Dissolve Option)]
        [Space(10)]
        _DissolveTex ("DissolveTexture", 2D) = "white" {}
        _DissolveThreshold ("DissolveThreshold", Range(0.0, 1.0)) = 0.1
        [HideInInspector]_DissolveEdgeColor ("DissolveEdgeColor", Color) = (0, 0, 0, 0)
        [HideInInspector] _DissolveEdgeWidth ("DissolveEdgeWidth", float) = 0.1
    }
    SubShader
    {
        Tags
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
        	Lighting Off
            ZWrite Off

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

            sampler2D _MainTex;
            sampler2D _DissolveTex;
            float4 _MainTex_ST;
            float _DissolveThreshold;
            float4 _DissolveEdgeColor;
            float _DissolveEdgeWidth;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
                float noise = tex2D(_DissolveTex, i.uv).r;
    
                // Calculate the edge width zone
                float edgeLower = _DissolveThreshold - _DissolveEdgeWidth;
                float edgeUpper = _DissolveThreshold + _DissolveEdgeWidth;

                if (noise >= _DissolveThreshold && noise < edgeUpper)
                {
                    if (_DissolveThreshold <= 0.0) 
                    {
                        discard;
                    }

                    // Calculate edge color based on proximity to threshold
                    float edgeFactor = 1.0 - (noise - _DissolveThreshold) / (_DissolveEdgeWidth * 0.02);
                    col = lerp(col, _DissolveEdgeColor, edgeFactor);
                }
                else if (noise >= edgeUpper)
                {
                    // Set full transparency beyond the upper edge
                    col.a = 0.0;
                }

                return col;
            }
            ENDCG
        }
    }
}
