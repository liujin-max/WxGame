Shader "UI/SpriteFill"
{
    Properties
    {
        _BgTex("Background Texture", 2D) = "white" {}
        _ProgressTex("Progress Texture",2D) = "gray"{}

        _ProgressValue("Progress Value",Range(0,1))=0

        _Color("Tint", Color) = (1,1,1,1)

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile __ UNITY_UI_CLIP_RECT
                #pragma multi_compile __ UNITY_UI_ALPHACLIP

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    OUT.worldPosition = v.vertex;
                    OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                    OUT.texcoord = v.texcoord;

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                sampler2D _BgTex;
                sampler2D _ProgressTex;
                half _ProgressValue;

                fixed4 frag(v2f IN) : SV_Target
                {
                    half mask = step(_ProgressValue,IN.texcoord.y);//如果IN.texcoord.y大于_ProgressValue，返回1，小于0.01返回0，用于代替if ,判断是否透明

                    fixed4 bg_color = tex2D (_BgTex, IN.texcoord);

                    fixed gray = dot(bg_color.rgb, float3(0.298912, 0.586611, 0.114478));
                    bg_color.rgb = lerp(bg_color.rgb, fixed3(gray, gray, gray), 1);

                    half4 color = (bg_color * mask+ tex2D(_ProgressTex, IN.texcoord)*(1-mask) + _TextureSampleAdd) * IN.color;

                    // half4 color = (tex2D(_BgTex, IN.texcoord)*mask+ tex2D(_ProgressTex, IN.texcoord)*(1-mask) + _TextureSampleAdd) * IN.color;

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    return color;
                }
            ENDCG
            }
        }
}