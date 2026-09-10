Shader "Custom/TerrainShader"
{
    Properties
    {
        _TerrainGradient ("Terrain Gradient", 2D) = "white" {}
        _MinTerrainHeight ("Min Height Threshold", Float) = 0
        _MaxTerrainHeight ("Max Height Threshold", Float) = 1
        _Smoothness ("Surface Smoothness", Range(0,1)) = 0.05

        _TexFrequency ("Texture Frequency/Density", Float) = 4.0
        _HueRand ("Hue Randomization Range", Range(0, 0.3)) = 0.04
        _SatRand ("Saturation Randomization Range", Range(0, 0.5)) = 0.1
        _ValRand ("Brightness Randomization Range", Range(0, 0.5)) = 0.15
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _TerrainGradient;
        float _MinTerrainHeight;
        float _MaxTerrainHeight;
        float _Smoothness;
        
        float4 _SeedOffset;
        float _TexFrequency;
        float _HueRand;
        float _SatRand;
        float _ValRand;

        struct Input
        {
            float3 worldPos; 
        };

        // Pseudo-random vector rotation mapping to scramble matrix coordinates
        float3 hash3D(float3 p)
        {
            p = float3(dot(p, float3(127.1, 311.7, 74.7)),
                       dot(p, float3(269.5, 183.3, 246.1)),
                       dot(p, float3(113.5, 271.9, 124.6)));
            return frac(sin(p) * 43758.5453123) * 2.0 - 1.0;
        }

        // Color conversions
        float3 rgb2hsv(float3 c)
        {
            float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
            float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
            float d = q.x - min(q.w, q.y);
            float e = 1.0e-10;
            return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
        }

        float3 hsv2rgb(float3 c)
        {
            float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
            return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float absoluteY = IN.worldPos.y - unity_ObjectToWorld._m13;

            // Compute standard raw gradient base color
            float heightValue = saturate((absoluteY - _MinTerrainHeight) / (_MaxTerrainHeight - _MinTerrainHeight));
            float3 baseRGB = tex2D(_TerrainGradient, float2(0.5, heightValue)).rgb;
            float3 hsv = rgb2hsv(baseRGB);

            // STOCHASTIC TEXTURE SHEARING MAPPING:
            // Scaled positioning space vector
            float3 scaledPos = IN.worldPos * _TexFrequency;

            // Inject skew parameters to alter matrix alignments based on your random seeds
            float3 gridSkew = floor(scaledPos + (hash3D(floor(scaledPos)) * 0.35));
            
            // Re-hash the randomized skewed spaces to create non-linear texture cell variations
            float3 textureNoise = hash3D(gridSkew + _SeedOffset.xyz); 

            // Apply texture color randomization ranges
            hsv.x = frac(hsv.x + (textureNoise.x * _HueRand));
            hsv.y = saturate(hsv.y + (textureNoise.y * _SatRand));
            hsv.z = saturate(hsv.z + (textureNoise.z * _ValRand));

            o.Albedo = hsv2rgb(hsv);
            o.Metallic = 0.0;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
