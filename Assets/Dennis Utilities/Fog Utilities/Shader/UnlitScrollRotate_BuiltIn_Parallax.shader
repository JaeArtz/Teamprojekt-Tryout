Shader "Custom/UnlitScrollRotate_BuiltIn_Parallax"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _UVRotation ("UV Rotation (Radians)", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _UVRotation;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // rotate around center
                uv -= 0.5;
                float s = sin(_UVRotation);
                float c = cos(_UVRotation);
                uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                uv += 0.5;

                fixed4 col = tex2D(_MainTex, uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}