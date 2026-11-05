Shader "Campus/FiberPaper" {
 Properties{[PerRendererData]_MainTex("Paper color",2D)="white"{} _Color("Tint",Color)=(1,1,1,1) _Grain("Fiber contrast",Range(0,8))=3.5 _StencilComp("Stencil comparison",Float)=8 _Stencil("Stencil ID",Float)=0 _StencilOp("Stencil operation",Float)=0 _StencilWriteMask("Stencil write mask",Float)=255 _StencilReadMask("Stencil read mask",Float)=255 _ColorMask("Color mask",Float)=15}
 SubShader{Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"} Stencil{Ref[_Stencil] Comp[_StencilComp] Pass[_StencilOp] ReadMask[_StencilReadMask] WriteMask[_StencilWriteMask]} Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask[_ColorMask]
 Pass{CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};struct v2f{float4 vertex:SV_POSITION;fixed4 color:COLOR;float2 uv:TEXCOORD0;};sampler2D _MainTex;fixed4 _Color;float _Grain;
 v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
 fixed4 frag(v2f i):SV_Target{fixed3 sample=tex2D(_MainTex,i.uv).rgb;float fiber=(dot(sample,float3(.299,.587,.114))-.955)*_Grain;float edge=.012*sin(i.uv.x*3.14159);return fixed4(saturate(i.color.rgb+fiber+edge),i.color.a);}
 ENDCG}
 }
}
