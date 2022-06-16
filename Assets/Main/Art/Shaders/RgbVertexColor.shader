// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RgbVertexColor"
{
	Properties
	{
		_RedColor("Red Color", Color) = (0,0,0,0)
		_GreenColor("Green Color", Color) = (0,0,0,0)
		_BlueColor("Blue Color", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		uniform float4 _RedColor;
		uniform float4 _GreenColor;
		uniform float4 _BlueColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 break1_g1 = i.vertexColor;
			float4 lerpResult5_g1 = lerp( ( _RedColor * break1_g1.r ) , ( _GreenColor * break1_g1.g ) , break1_g1.g);
			float4 lerpResult8_g1 = lerp( lerpResult5_g1 , ( _BlueColor * break1_g1.b ) , break1_g1.b);
			float4 temp_output_2_0 = lerpResult8_g1;
			o.Albedo = temp_output_2_0.rgb;
			o.Emission = ( temp_output_2_0 * float4( 0.25,0.25,0.25,1 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18934
40;214;1024;484;1787.771;220.2239;1.93373;True;True
Node;AmplifyShaderEditor.ColorNode;3;-976.5861,141.0249;Inherit;False;Property;_RedColor;Red Color;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-973.3862,317.025;Inherit;False;Property;_GreenColor;Green Color;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-973.386,488.2251;Inherit;False;Property;_BlueColor;Blue Color;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1;-959.7999,-44.5;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;2;-578.1864,29.42249;Inherit;False;RgbColor;0;;1;581b0de0e430ca3c6b3332a36da1a1c3;0;4;11;COLOR;0,0,0,0;False;12;COLOR;0,0,0,0;False;13;COLOR;0,0,0,0;False;14;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-302.9533,142.6044;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.25,0.25,0.25,1;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RgbVertexColor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;11;1;0
WireConnection;2;12;3;0
WireConnection;2;13;4;0
WireConnection;2;14;5;0
WireConnection;6;0;2;0
WireConnection;0;0;2;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=9190592F57E8933520AFB3077F69C36318705A34