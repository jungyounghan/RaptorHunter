Shader "Custom/ClippingShader"
{
    Properties
    {
        _MainTexure("Texture", 2D) = "white" {}
        _ClippingMin("Clipping Min", Vector) = (0,0,0,0)
        _ClippingMax("Clipping Max", Vector) = (1,1,1,0)
        _ClippingCenter("Clipping Center", Vector) = (0,0,0,0)
        _RotationRow0("Rotation Row 0", Vector) = (1,0,0,0)
        _RotationRow1("Rotation Row 1", Vector) = (0,1,0,0)
        _RotationRow2("Rotation Row 2", Vector) = (0,0,1,0)
        _EdgeIntensity("Edge Intensity", Float) = 0.1
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _MainTexure;
                float4 _ClippingMin;
                float4 _ClippingMax;
                float4 _ClippingCenter;
                float4 _RotationRow0;
                float4 _RotationRow1;
                float4 _RotationRow2;
                float _EdgeIntensity;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // ȸ�� ��� ����
                    float3x3 rotationMatrix = float3x3(
                        _RotationRow0.xyz,
                        _RotationRow1.xyz,
                        _RotationRow2.xyz
                    );

                // ���� ��ǥ�� ��ȯ
                float3 localPos = mul((i.worldPos - _ClippingCenter.xyz), rotationMatrix);

                // Ŭ���� ���� �ٱ����� Ȯ��
                bool isInsideClip = all(localPos >= _ClippingMin.xyz) && all(localPos <= _ClippingMax.xyz);

                // Ŭ���� ���� �ٱ��̸� �ε巴�� �����ϰ� ó��
                float distanceToClipEdge = min(
                    min(localPos.x - _ClippingMin.x, _ClippingMax.x - localPos.x),
                    min(localPos.y - _ClippingMin.y, _ClippingMax.y - localPos.y)
                );

                // �ε巴�� ó���� alpha��
                float alpha = smoothstep(_EdgeIntensity, 0.0, distanceToClipEdge);

                // �ؽ�ó�� �ҷ�����, alpha���� ����
                fixed4 texColor = tex2D(_MainTexure, i.uv);
                texColor.a *= alpha;

                return texColor;
            }
            ENDCG
        }
        }
}