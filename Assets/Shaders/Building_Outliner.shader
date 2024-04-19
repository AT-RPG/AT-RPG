
Shader "AT_RPG/Building/Outliner"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Range(1.0, 3.0)) = 0.0
        _Color ("Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct AppData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Vertex2Fragment
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Width;
            float4 _Color;

            Vertex2Fragment vert (AppData input)
            {
                Vertex2Fragment output;
                output.vertex = input.vertex * _Width;  
                output.vertex = UnityObjectToClipPos(output.vertex);

                output.uv = input.uv;

                return output;
            }

            float4 frag (Vertex2Fragment input) : SV_Target
            {
                float2 uv = input.uv;
                float time = _Time.y;

                float2 center = float2(0.5, 0.5);
                float2 halfSize = float2(0.2, 0.2);
                float2 dist = abs(uv - center);
                float aspect = halfSize.y / halfSize.x;
                float dir = (dist.x * aspect > dist.y) ? -sign(uv.x - center.x) : sign(uv.y - center.y);
                float dash = step(0.5, frac((uv.x + uv.y) * dir * 10.0 + time));
                float col = lerp(1.0, 0.0, dash);

                return float4(_Color.xyz, (int)dash == 1 ? 1.0 : 0.0);
            }

            ENDCG
        }
    }
}
