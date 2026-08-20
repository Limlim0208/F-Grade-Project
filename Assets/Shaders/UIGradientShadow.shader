Shader "UI/GradientShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Shadow Color", Color) = (0,0,0,0.5)
        _BlurX ("Blur Amount X", Range(0.0, 0.5)) = 0.25
        _BlurY ("Blur Amount Y", Range(0.0, 0.5)) = 0.25
        _SizeX ("Shadow Size X", Range(0.0, 1.0)) = 0.8
        _SizeY ("Shadow Size Y", Range(0.0, 1.0)) = 0.8
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _BlurX;
            float _BlurY;
            float _SizeX;
            float _SizeY;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centered = i.uv - 0.5;

                // x, y 축을 각각 독립적인 Size/Blur로 정규화
                float distX = abs(centered.x) - _SizeX * 0.5;
                float distY = abs(centered.y) - _SizeY * 0.5;

                float alphaX = 1.0 - smoothstep(0.0, _BlurX, max(distX, 0.0));
                float alphaY = 1.0 - smoothstep(0.0, _BlurY, max(distY, 0.0));

                // 두 축의 감쇠를 곱해서 상하좌우 모두 그라데이션 적용
                float alpha = alphaX * alphaY;

                fixed4 col = _Color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}