Shader "Custom/SectorClippingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Strength", float) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Brightness ("Brightness", Range(0,1)) = 1.0

        _CenterX ("Center X", float) = 0.0
        _CenterY ("Center Y", float) = 0.0
        _Degree ("Degree", float) = 0.0
        _Angle ("Angle", Range(0, 360)) = 360
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

        float _CenterX;
        float _CenterY;
        float _Degree;
        float _Angle;

       void surf (Input IN, inout SurfaceOutputStandard o)
       {
            // �þ߰� ��� Ŭ���� ���� (��/�Ʒ��� ���̰�, �¿츸 ����)
            float3 viewVec = normalize(IN.worldPos - float3(_CenterX, 0, _CenterY));
            viewVec.y = 0; // Y�� �����Ͽ� ���� ���⸸ ��
            viewVec = normalize(viewVec); // �ٽ� ����ȭ

            // _Degree ���� ������ ��ȯ (����)
            float rad = radians(_Degree);
    
            // ȸ�� ��ȯ�� ���� ���� ���
            float3 clipDir = float3(cos(rad), 0, sin(rad));

            float angle = acos(dot(viewVec, clipDir)) * (360.0 / 3.14159265); // ���� ��ȯ
            if (angle > _Angle) // �þ߰� ����� ����
            {
                discard;
            }

            // �ؽ�ó ���� ��������
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            c.rgb *= _Brightness;

            // �븻�� ���� (���� ����)
            float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            normal.xy *= _BumpScale;
            normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
            o.Normal = normal;

            // ���� ���� ����
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }

    FallBack "Diffuse"
}