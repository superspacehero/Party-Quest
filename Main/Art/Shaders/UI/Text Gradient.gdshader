shader_type canvas_item;
render_mode unshaded;

uniform float intensity : hint_range(0.0, 20.0, 0.1);

uniform bool x;
uniform vec4 color_horizontal_left: source_color = vec4(1.0);
uniform vec4 color_horizontal_right: source_color = vec4(1.0);

uniform bool y;
uniform vec4 color_vertical_top: source_color = vec4(1.0);
uniform vec4 color_vertical_bottom: source_color = vec4(1.0);

void fragment()
{
	float texture_alpha = texture(TEXTURE, UV).a;
	float alpha = COLOR.a;

	COLOR.a = 1.0;
	
	vec2 uv = UV;

	uv *= alpha;

	if (texture_alpha > 0.0)
	{
		if (x)
		{
			float gradient_x = mod(uv.x * intensity, 1.0);
			COLOR = COLOR * mix(color_horizontal_left, color_horizontal_right, gradient_x);
			COLOR.a *= texture_alpha;
		}
		if (y)
		{
			float gradient_y = mod(uv.y * intensity, 1.0);
			COLOR = COLOR * mix(color_vertical_top, color_vertical_bottom, gradient_y);
			COLOR.a *= texture_alpha;
		}
	}
	else
	{ 
		COLOR = texture(TEXTURE, UV);
	}
}