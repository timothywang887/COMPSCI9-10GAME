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

        struct Input
        {
            float3 worldPos; 
        };

        // Pseudo-random 2D noise algorithm for the shader color variation
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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Extract clean localized height relative to the object transform position
            float absoluteY = IN.worldPos.y - unity_ObjectToWorld._m13;

            // 1:1 perfect gradient fit using the true min/max heights from C#
            float heightValue = saturate((absoluteY - _MinTerrainHeight) / (_MaxTerrainHeight - _MinTerrainHeight));

            // Generate procedural color noise using X and Z horizontal space coordinates
            float2 noiseUV = IN.worldPos.xz * _ColorNoiseScale + _SeedOffset.xy;
            float n = noise2D(noiseUV) * 2.0 - 1.0; // Puts noise in a -1 to 1 range

            // Apply the color noise directly to twist the height value sampling slightly 
            float modifiedHeight = saturate(heightValue + (n * _ColorNoiseStrength));

            // Sample the gradient color map using our newly offset height value
            o.Albedo = tex2D(_TerrainGradient, float2(0.5, modifiedHeight)).rgb;
            
            o.Metallic = 0.0;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
