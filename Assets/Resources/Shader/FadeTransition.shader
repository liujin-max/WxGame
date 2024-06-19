Shader "Custom/FadeTransition"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SecondTex ("Second Texture", 2D) = "white" {}
        _Fade ("Fade", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _SecondTex;
        float _Fade;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 c1 = tex2D(_MainTex, IN.uv_MainTex);
            half4 c2 = tex2D(_SecondTex, IN.uv_MainTex);
            o.Albedo = lerp(c1.rgb, c2.rgb, _Fade);
            o.Alpha = lerp(c1.a, c2.a, _Fade);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
