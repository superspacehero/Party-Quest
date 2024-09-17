shader_type spatial;
render_mode unshaded, fog_disabled;

group_uniforms general;
	uniform bool lock_aspect = true;
	uniform float aspect_ratio = 1.0;
	uniform vec2 fov = vec2(180.0, 60.0);
    uniform float vertical_offset = 0.6;
	uniform sampler2D screen_texture : hint_screen_texture;

// Sky information
group_uniforms sky;
	uniform sampler2D sky_time_gradient : source_color, repeat_disable;
    uniform float sky_time : hint_range(0.0, 1.0) = 0.5;

// First texture (background)
group_uniforms background;
    uniform vec4 background_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
	uniform sampler2D background_texture : source_color;
	uniform float background_movement_pan = 0.0015;
	uniform float background_scroll_speed = 0.01;
	uniform ivec2 background_tiling = ivec2(1, 1);
    uniform int background_blend_mode : hint_range(0, 3) = 0;

// Second texture (foreground)
group_uniforms foreground;
    uniform vec4 foreground_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
	uniform sampler2D foreground_texture : source_color;
	uniform float foreground_movement_pan = 0.002;
	uniform float foreground_scroll_speed = 0.0;
	uniform ivec2 foreground_tiling = ivec2(1, 1);
    uniform int foreground_blend_mode : hint_range(0, 3) = 0;

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
    // Whatever vertical_offset is set to, lerp from it to 0.5 the closer to straight up/down the camera looks
    BG_COORDS.yw += mix(0.5, vertical_offset, CAMERA_DIRECTION_WORLD.z);

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

vec4 blend_texture(vec4 base_texture, vec4 new_texture, int blend_mode) {
    switch (blend_mode) {
        case 0: // Mix
            return mix(base_texture, new_texture, new_texture.a);
        case 1: // Add
            return base_texture + new_texture;
        case 2: // Subtract
            return base_texture - new_texture;
        case 3: // Multiply
            return base_texture * new_texture;
    }
}

void fragment() {
    vec2 noise_uv = SCREEN_UV;
	// noise_uv.x *= VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;
    vec4 color = texture(screen_texture, SCREEN_UV);

    // Background texture calculations
    vec2 uv = (SCREEN_UV) * BG_SCALE + BG_COORDS.xy;
    uv *= vec2(background_tiling);
    vec4 texture_vector = texture(background_texture, uv).rgba;
    texture_vector.a = alpha_distortion(texture_vector.a, noise_uv,
        background_alpha_distortion, background_noise_speed);
    texture_vector *= background_color;
    color = blend_texture(color, texture_vector, background_blend_mode);

    // Foreground texture calculations
    uv = (SCREEN_UV) * BG_SCALE + BG_COORDS.zw;
    uv *= vec2(foreground_tiling);
    texture_vector = texture(foreground_texture, uv).rgba;
    texture_vector.a = alpha_distortion(texture_vector.a, noise_uv,
        foreground_alpha_distortion, foreground_noise_speed);
    texture_vector *= foreground_color;
    color = blend_texture(color, texture_vector, foreground_blend_mode);

    // Sky color calculations
    color *= texture(sky_time_gradient, vec2(sky_time, sky_time)).rgba;

	ALBEDO = color.rgb;
	ALPHA = color.a;
}
