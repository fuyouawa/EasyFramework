using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public class EditorIconUtility
    {
        private static readonly string iconShader = @"
Shader ""Hidden/EasyToolKit/Core/Editor/GUIIcon""
{
	Properties
	{
        _MainTex(""Texture"", 2D) = ""white"" {}
        _Color(""Color"", Color) = (1,1,1,1)
	}
    SubShader
	{
        Blend SrcAlpha Zero
        Pass
        {
            CGPROGRAM
                " + "#" + @"pragma vertex vert
                " + "#" + @"pragma fragment frag
                " + "#" + @"include ""UnityCG.cginc""

                struct appdata
                {
                    float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

                struct v2f
                {
                    float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

                sampler2D _MainTex;
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
				{
                    // drop shadow:
                    // float texelSize = 1.0 / 34.0;
                    // float2 shadowUv = clamp(i.uv + float2(-texelSize, texelSize * 2), float2(0, 0), float2(1, 1));
                    // fixed4 shadow = fixed4(0, 0, 0, tex2D(_MainTex, shadowUv).a); 

					fixed4 col = _Color;
					col.a *= tex2D(_MainTex, i.uv).a;

                    // drop shadow:
                    // col = lerp(shadow, col, col.a);

					return col;
				}
			ENDCG
		}
	}
}
";

        private static Material s_iconMaterial;
        
        public static Texture2D RenderIcon(Texture2D originTexture, Color color)
        {
            if (s_iconMaterial == null || s_iconMaterial.shader == null)
            {
                s_iconMaterial = new Material(ShaderUtil.CreateShaderAsset(iconShader));
            }

            s_iconMaterial.SetColor("_Color", color);

            var prevSRGB = GL.sRGBWrite;
            GL.sRGBWrite = true;
            RenderTexture prev = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(originTexture.width, originTexture.height, 0);
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(1, 1, 1, 0));
            Graphics.Blit(originTexture, rt, s_iconMaterial);

            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            texture.filterMode = FilterMode.Bilinear;
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.alphaIsTransparency = true;
            texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = prev;
            GL.sRGBWrite = prevSRGB;
            return texture;
        }
    }
}
