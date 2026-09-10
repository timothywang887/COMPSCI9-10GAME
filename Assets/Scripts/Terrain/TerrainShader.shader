Shader "Custom/TerrainShader"
{
    Properties
    {
        // Exposing these properties lets the C# script talk to the shader
        _TerrainGradient ("Terrain Gradient", 2D) = "white" {}
        _MinTerrainHeight ("Min Terrain Height", Float) = 0
        _MaxTerrainHeight ("Max Terrain Height", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        // Variables match the property block above
        sampler2D _TerrainGradient;
        float _MinTerrainHeight;
        float _MaxTerrainHeight;

        struct Input
        {
            float3 worldPos; // Removed unused uv_MainTex
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calculate a 0 to 1 value representing where this vertex is between min and max height
            float heightValue = saturate((IN.worldPos.y - _MinTerrainHeight) / (_MaxTerrainHeight - _MinTerrainHeight));

            // Sample the gradient texture using our 0-1 height mapping
            // (Passed into the V coordinate since our texture is 1px wide by 100px tall)
            o.Albedo = tex2D(_TerrainGradient, float2(0.5, heightValue)).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
