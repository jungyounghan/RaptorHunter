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

                // �ؽ�ó �� ����
                sampler2D _MainTex;
                float4 _Color;
                // ��Ż��, ������
                float _Metallic;
                float _Smoothness;
                // ��ָ�
                sampler2D _BumpMap;

                // ���� ���̴�
                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal)); // ǥ�� ���� ����
                    return o;
                }

                // �����׸�Ʈ ���̴�
                half4 frag(v2f i) : SV_Target
                {
                    // �ؽ�ó�� ���� ȥ��
                    half4 texColor = tex2D(_MainTex, i.uv) * _Color;

                    // ��� �� ����
                    half3 normal = tex2D(_BumpMap, i.uv).xyz * 2.0 - 1.0; // ��� ���� �� ���� -1~1�� ��ȯ
                    normal = normalize(normal);

                    // �⺻���� ���� ��� (��ǻ��)
                    half3 lightDir = normalize(float3(0.577, 0.577, 0.577));  // ������ ���Ɽ
                    half diffuse = max(0, dot(i.normal, lightDir));
                    half3 diffuseColor = texColor.rgb * diffuse;

                    // ��Ż�� �� �������� ���� �ݻ籤 ���
                    half reflection = pow(max(0, dot(i.normal, lightDir)), _Smoothness);
                    half3 specular = reflection * _Metallic;

                    // ���� ����
                    half3 finalColor = diffuseColor + specular;

                    // ���� ���� ��ȯ
                    return half4(finalColor, texColor.a);
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}