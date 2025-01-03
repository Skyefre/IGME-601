﻿Shader "Hidden/PaletteSwapLookup"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PaletteTex("TargetTexture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off ZTest Always

        Pass
        {
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _PaletteTex;
            float _Alpha;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainColor = tex2D(_MainTex, i.uv);
                float x = mainColor.r;
                float4 paletteColor = tex2D(_PaletteTex, float2(x, 0));
                paletteColor.a = mainColor.a * _Alpha; // Modify the alpha channel
                return paletteColor;
            }

            ENDCG
        }
    }
}