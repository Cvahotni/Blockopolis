Shader "Unlit/UnlitColorAO"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        //_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

        _AOTexture ("AO Texture", 2D) = "white" {}
        _AOColor ("AO Color", Color) = (1, 1, 1, 1)
        _AOPower ("AO Texture Power", Range(0, 3)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _AOTexture;
            uniform fixed4 _AOTexture_ST;
            uniform half3 _AOColor;
            uniform half _AOPower;

            fixed4 _Color;

            static const half3 whiteColor = half3(1, 1, 1);

            struct appdata
            {
                float4 vertex : POSITION;
                half3 normal : NORMAL;
                half3 vColor : COLOR;
                float4 uv0 : TEXCOORD0;
                //float4 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : POSITION;
                //float4 worldPos : TEXCOORD0;
                //float2 uv : TEXCOORD1;
                float2 aouv : TEXCOORD4;


            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                o.aouv = TRANSFORM_TEX(v.uv0, _AOTexture);
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.pos)*_Color;

                // sample color only.
                fixed4 col = _Color;

                // apply AO
                half4 AOTexVar = lerp(half4(_AOColor, 1), half4(whiteColor, 1), lerp(half4(1,1,1,1), tex2D(_AOTexture, i.aouv), _AOPower));
                col *= AOTexVar;

                return col;
            }
            ENDCG
        }
    }
}
