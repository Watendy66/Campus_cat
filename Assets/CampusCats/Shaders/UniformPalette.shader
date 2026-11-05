Shader "Campus/UniformPalette" {
 Properties { _MainTex("Original clothing texture",2D)="white"{} _UniformColor("Uniform colour",Color)=(.2,.42,.62,1) }
 SubShader { Tags {"RenderType"="Opaque"} LOD 200
 CGPROGRAM
 #pragma surface surf Standard fullforwardshadows
 #pragma target 3.0
 sampler2D _MainTex;fixed4 _UniformColor;struct Input {float2 uv_MainTex;};
 void surf(Input IN,inout SurfaceOutputStandard o){fixed4 c=tex2D(_MainTex,IN.uv_MainTex);if(c.b>c.r*1.15&&c.b>c.g*.95)c.rgb=_UniformColor.rgb*(.55+max(c.r,max(c.g,c.b))*.65);o.Albedo=c.rgb;o.Metallic=0;o.Smoothness=.15;o.Alpha=1;}
 ENDCG
 } FallBack "Diffuse"
}
