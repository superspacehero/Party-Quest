shader_type spatial;
render_mode unshaded;

group_uniforms general;
	uniform bool lock_aspect = true;
	uniform float aspect_ratio = 1.0;
	uniform vec2 fov = vec2(180.0, 90.0);

// First texture (background)
group_uniforms background;
	uniform sampler2D background_texture : source_color;
	uniform float background_movement_pan = 0.0015;
	uniform float background_scroll_speed = 0.01;
	uniform ivec2 background_tiling = ivec2(1, 1);

// Second texture (foreground)
group_uniforms foreground;
	uniform sampler2D foreground_texture : source_color;
	uniform float foreground_movement_pan = 0.002;
	uniform float foreground_scroll_speed = 0.0;
	uniform ivec2 foreground_tiling = ivec2(1, 1);

group_uniforms noise;
    uniform sampler2D noise_texture : source_color;

    uniform float background_alpha_distortion : hint_range(0.0, 1.0) = 1.0;
    uniform vec2 background_noise_speed = vec2(0.025, 0.025);

    uniform float foreground_alpha_distortion : hint_range(0.0, 1.0) = 0.0;
    uniform vec2 foreground_noise_speed = vec2(0.025, 0.025);

varying vec4 BG_COORDS;
varying vec2 BG_SCALE;

void vertex() {
    //Camera YX rotation per Basis.get_euler source code
    float y = atan(VIEW_MATRIX[0][2], VIEW_MATRIX[2][2]);
    float x = asin(VIEW_MATRIX[1][2]);

    //Map rotation to screen space
    BG_COORDS = vec4(y * 0.5, -x, y * 0.5, -x) * -(1.0 / PI);
    BG_COORDS.yw += 0.5;

    BG_SCALE.y = fov.y * (1.0 / 180.0);
    BG_SCALE.x = !lock_aspect ?
        fov.x * (1.0 / 360.0) :
        VIEWPORT_SIZE.x / (VIEWPORT_SIZE.y * aspect_ratio) * BG_SCALE.y;

    // Similarly pan the background based on the camera's position
    BG_COORDS.x -= VIEW_MATRIX[3][0] * background_movement_pan;
	BG_COORDS.z -= VIEW_MATRIX[3][0] * foreground_movement_pan;

	BG_COORDS.x -= TIME * background_scroll_speed;
	BG_COORDS.z -= TIME * foreground_scroll_speed;

    //Keep background centered vertically when FOV changes
    BG_COORDS.yw *= BG_SCALE.y > 1.0 ? 0.0 : 1.0 - BG_SCALE.y;
}

// Function to distort the alpha channel of the texture over time
float alpha_distortion(float input_alpha, vec2 uv, float amount, vec2 speed) {
    vec2 noise_uv = (uv * BG_SCALE) + (speed * TIME);
    float distortion = texture(noise_texture, noise_uv).r;
    return input_alpha * clamp(mix(input_alpha, input_alpha * distortion, -amount * distortion), 0.0, 1.0);
}

void fragment() {
    vec2 noise_uv = SCREEN_UV * BG_SCALE;

    vec2 uv = (SCREEN_UV) * BG_SCALE + BG_COORDS.xy;
    uv *= vec2(background_tiling);
    vec4 color_background = texture(background_texture, uv).rgba;
    color_background.a = alpha_distortion(color_background.a, noise_uv,
        background_alpha_distortion, background_noise_speed);

    // Second texture calculations
    vec2 uv2 = (SCREEN_UV) * BG_SCALE + BG_COORDS.zw;
    uv2 *= vec2(foreground_tiling);
    vec4 color_foreground = texture(foreground_texture, uv2).rgba;
    color_foreground.a = alpha_distortion(color_foreground.a, noise_uv,
        foreground_alpha_distortion, foreground_noise_speed);

    // Combine the two textures
    // Note: You might want to adjust the blending based on your specific needs
	ALBEDO = mix(color_background.rgb, color_foreground.rgb, color_foreground.a);
	ALPHA = clamp(color_background.a + color_foreground.a, 0.0, 1.0);
}
