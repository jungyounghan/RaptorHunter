Shader "Custom/CustomShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Mode("Rendering Mode", Range(0, 2)) = 0
        _Cull("Cull Mode", Int) = 2
        _ZWrite("ZWrite", Float) = 1
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : TEXCOORD1;
                };

                // 텍스처 및 색상
                sampler2D _MainTex;
                float4 _Color;
                // 메탈릭, 스무딩
                float _Metallic;
                float _Smoothness;
                // 노멀맵
                sampler2D _BumpMap;

                // 정점 셰이더
                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal)); // 표면 법선 벡터
                    return o;
                }

                // 프래그먼트 셰이더
                half4 frag(v2f i) : SV_Target
                {
                    // 텍스처와 색상 혼합
                    half4 texColor = tex2D(_MainTex, i.uv) * _Color;

                    // 노멀 맵 적용
                    half3 normal = tex2D(_BumpMap, i.uv).xyz * 2.0 - 1.0; // 노멀 맵의 값 범위 -1~1로 변환
                    normal = normalize(normal);

                    // 기본적인 조명 계산 (디퓨즈)
                    half3 lightDir = normalize(float3(0.577, 0.577, 0.577));  // 간단한 하향광
                    half diffuse = max(0, dot(i.normal, lightDir));
                    half3 diffuseColor = texColor.rgb * diffuse;

                    // 메탈릭 및 스무딩에 따른 반사광 계산
                    half reflection = pow(max(0, dot(i.normal, lightDir)), _Smoothness);
                    half3 specular = reflection * _Metallic;

                    // 최종 색상
                    half3 finalColor = diffuseColor + specular;

                    // 최종 색상 반환
                    return half4(finalColor, texColor.a);
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}