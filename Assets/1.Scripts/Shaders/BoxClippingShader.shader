Shader "Custom/BoxClippingShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {} 
        _BumpScale ("Normal Strength", float) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Brightness ("Brightness", Range(0,1)) = 1

        _ClipMin ("Clip Min", Vector) = (0, 0, 0, 0)
        _ClipMax ("Clip Max", Vector) = (1, 1, 1, 0)
        _ClipCenter ("Clip Center", Vector) = (0, 0, 0, 0)
        _ClipRotRow0 ("Rotation Row 0", Vector) = (1, 0, 0, 0)
        _ClipRotRow1 ("Rotation Row 1", Vector) = (0, 1, 0, 0)
        _ClipRotRow2 ("Rotation Row 2", Vector) = (0, 0, 1, 0)
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _BumpScale;
        float _Metallic;
        float _Smoothness;
        float _Brightness;

        float4 _ClipMin;
        float4 _ClipMax;
        float4 _ClipCenter;
        float4 _ClipRotRow0;
        float4 _ClipRotRow1;
        float4 _ClipRotRow2;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 회전 행렬 구성
            float3x3 rotationMatrix = float3x3(
                _ClipRotRow0.xyz,
                _ClipRotRow1.xyz,
                _ClipRotRow2.xyz
            );

            // 월드 좌표를 클리핑 박스의 로컬 공간으로 변환
            float3 localPos = mul((IN.worldPos - _ClipCenter.xyz), rotationMatrix);

            // 클리핑 영역 내부인지 확인
            if (all(localPos >= _ClipMin.xyz) && all(localPos <= _ClipMax.xyz))
            {
                discard;
            }

            // 텍스처 색상 가져오기
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            c.rgb *= _Brightness;

            // 노말맵 적용 (강도 조절)
            float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            normal.xy *= _BumpScale;  // XY 축에 강도 적용
            normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy))); // Z 보정
            o.Normal = normal;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}