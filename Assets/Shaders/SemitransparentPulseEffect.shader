Shader "Unlit/SemitransparentPulseEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Transparency("Transparency", Range(0.0,0.99)) = 0.5
        _PulseSpeed("Pulse Speed", Float) = 1
        [Toggle] _WindDirection("Going Down", Float) = 0
        _Frequency("Pulse Frequency", Float) = 200
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 100

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Transparency;
            fixed _WindDirection;
            fixed _PulseSpeed;
            fixed _Frequency;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 stripes = cos((i.uv.y + _Time.x * _PulseSpeed * (_WindDirection * 2 - 1)) * _Frequency) * 0.5 + 0.5;
                col += stripes/4;
                col.a = _Transparency * stripes;

                return col;
            }
            ENDCG
        }
    }
}
