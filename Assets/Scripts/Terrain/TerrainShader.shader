Shader "Custom/TerrainShader"
{
    Properties
    {
        _TerrainGradient ("Terrain Gradient", 2D) = "white" {}
        _MinTerrainHeight ("Min Height Threshold", Float) = 0
        _MaxTerrainHeight ("Max Height Threshold", Float) = 1
        _Smoothness ("Surface Smoothness", Range(0,1)) = 0.05
        
        _ColorNoiseScale ("Color Noise Scale", Float) = 0.1
        _ColorNoiseStrength ("Color Noise Strength", Float) = 0.05

        // Micro texture parameters
        _TexNoiseSize ("Tex Noise Size", Float) = 2.0
        _TexNoiseDensity ("Tex Noise Density", Float) = 1.0
        _HueShiftRange ("Hue Shift Range", Range(0, 0.5)) = 0.05
        _BrightnessRange ("Brightness Range", Range(0, 0.5)) = 0.15
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
        
        float _ColorNoiseScale;
        float _ColorNoiseStrength;
        float4 _SeedOffset;

        float _TexNoiseSize;
        float _TexNoiseDensity;
        float _HueShiftRange;
        float _BrightnessRange;

        struct Input
        {
            float3 worldPos; 
        };

        // Utility: Noise generators
        float hash2D(float2 p)
        {
            return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
        }

        float noise2D(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            float2 u = f * f * (3.0 - 2.0 * f);
            return lerp(lerp(hash2D(i + float2(0.0, 0.0)), hash2D(i + float2(1.0, 0.0)), u.x),
                        lerp(hash2D(i + float2(0.0, 1.0)), hash2D(i + float2(1.0, 1.0)), u.x), u.y);
        }

        // Color Space Convertors (RGB <-> HSV)
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

            // Macro-Color blending positioning
            float heightValue = saturate((absoluteY - _MinTerrainHeight) / (_MaxTerrainHeight - _MinTerrainHeight));
            float2 macroNoiseUV = IN.worldPos.xz * _ColorNoiseScale + _SeedOffset.xy;
            float macroNoise = noise2D(macroNoiseUV) * 2.0 - 1.0;
            float modifiedHeight = saturate(heightValue + (macroNoise * _ColorNoiseStrength));

            // Sample raw gradient band color
            float3 baseRGB = tex2D(_TerrainGradient, float2(0.5, modifiedHeight)).rgb;

            // Convert to HSV to safely alter textures without wiping color richness
            float3 hsv = rgb2hsv(baseRGB);

            // Generate localized micro texture noise (High Frequency)
            float2 microNoiseUV = IN.worldPos.xz * _TexNoiseSize * _TexNoiseDensity + (_SeedOffset.xy * 3.14);
            float microNoise = noise2D(microNoiseUV) * 2.0 - 1.0; // Range -1 to 1

            // Inject adjustments based on slider parameters
            hsv.x = frac(hsv.x + (microNoise * _HueShiftRange)); // Hue loop
            hsv.z = saturate(hsv.z + (microNoise * _BrightnessRange)); // Brightness push

            // Convert back to standard RGB engine output
            o.Albedo = hsv2rgb(hsv);
            
            o.Metallic = 0.0;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
