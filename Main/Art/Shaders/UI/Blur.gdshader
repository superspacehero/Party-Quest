shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float blur_amount : hint_range(0.1, 10.0, 0.1) = 2.5;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
		COLOR = textureLod(SCREEN_TEXTURE, SCREEN_UV, COLOR.a * blur_amount) * mix(vec4(1.0), COLOR, COLOR.a);
		COLOR.a = 1.0;
}

void light() {
	// Called for every pixel for every light affecting the CanvasItem.
}
