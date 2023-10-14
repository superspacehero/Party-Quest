shader_type sky;
render_mode use_quarter_res_pass;

// Originaly based on https://godotshaders.com/shader/stylized-sky-shader-with-clouds/ but there's not much left

group_uniforms sky;
	uniform vec3 day_top_color : source_color = vec3( 0.1, 0.6, 1.0 );
	uniform vec3 day_bottom_color : source_color = vec3( 0.4, 0.8, 1.0 );
	uniform vec3 sunset_top_color : source_color = vec3( 0.7, 0.75, 1.0 );
	uniform vec3 sunset_bottom_color : source_color = vec3( 1.0, 0.5, 0.7 );
	uniform vec3 night_top_color : source_color = vec3( 0.02, 0.0, 0.04 );
	uniform vec3 night_bottom_color : source_color = vec3( 0.1, 0.0, 0.2 );
    uniform sampler2D voronoi_texture : filter_linear_mipmap, hint_default_black;

group_uniforms horizon;
	uniform vec3 horizon_color : source_color = vec3( 0.0, 0.7, 0.8 );
	uniform float horizon_blur : hint_range( 0.0, 1.0, 0.01 ) = 0.05;

group_uniforms sun; // First DirectionalLight3D will be the sun
	uniform vec3 sun_color : source_color = vec3( 10.0, 8.0, 1.0 );
	uniform vec3 sun_sunset_color : source_color = vec3( 10.0, 0.0, 0.0 );
	uniform float sun_size : hint_range( 0.01, 1.0 ) = 0.2;
	uniform float sun_blur : hint_range( 0.01, 20.0 ) = 10.0;

group_uniforms moon; // Second DirectionalLight3D will be the moon
	uniform vec3 moon_color : source_color = vec3( 0.945098, 0.6738669, 0.1209726 );
	uniform float moon_size : hint_range( 0.01, 1.0 ) = 0.06;
	uniform float moon_blur : hint_range( 0.01, 10.0 ) = 0.1;

group_uniforms clouds;
	// Replaced by noise functions, unncomment if you want to use graphical textures
//	uniform sampler2D clouds_top_texture : filter_linear_mipmap, hint_default_black;
//	uniform sampler2D clouds_middle_texture : filter_linear_mipmap, hint_default_black;
//	uniform sampler2D clouds_bottom_texture : filter_linear_mipmap, hint_default_black;
	uniform vec3 clouds_color : source_color = vec3( 1.0, 1.0, 1.00 );
	uniform float clouds_speed : hint_range( 0.0, 20.0, 0.01 ) = 2.0;
	uniform float clouds_direction : hint_range( -1.0, 1.0, 1.0 ) = 1.0;
	uniform float clouds_density = 2.0;
	uniform float clouds_roughness = 5.0;
	uniform float clouds_cutoff : hint_range( 0.0, 1.0, 0.01 ) = 0.3;
	uniform float clouds_fuzziness : hint_range( 0.0, 2.0, 0.01 ) = 0.5;
	// More weight is simply a darker color, usefull for rain/storm
	uniform float clouds_weight : hint_range( 0.0, 1.0, 0.01 ) = 0.0;
	uniform float clouds_fade_point : hint_range(0.0, 1.0, 0.01) = 0.5;
	uniform float clouds_blur : hint_range( 0.0, 1.0, 0.01 ) = 0.25;

group_uniforms stars;
	// Stars should be at black background
	uniform float stars_speed : hint_range( 0.0, 20.0, 0.01 ) = 1.0;

group_uniforms settings;
	uniform float overwritten_time = 0.0;

////////////////////////////////////////////////////////////////////////////////////////////////////

// Function needed to calculate the phase of the moon
// Source: https://kelvinvanhoorn.com/2022/03/17/skybox-tutorial-part-1/
float sphere_intersect( vec3 view_dir, vec3 sphere_pos, float radius )
{
    float b = dot( -sphere_pos, view_dir );
    float c = dot( -sphere_pos, -sphere_pos ) - pow( radius, 2 );
    float h = pow( b, 2 ) - c;
    return h < 0.0 ? -1.0 : -b - sqrt( h );
}

void sky()
{
	float time = overwritten_time != 0.0 ? overwritten_time : TIME;

	//////////////////// SKY ///////////////////////////////////////////////////////////////////////
	float _eyedir_y = abs( sin( EYEDIR.y * PI * 0.5 ));

	// The day color will be our base color
	vec3 _sky_color = mix( day_bottom_color, day_top_color, _eyedir_y );
	_sky_color = mix( _sky_color, vec3( 0.0 ), clamp(( 0.7 - clouds_cutoff ) * clouds_weight, 0.0, 1.0 ));

	float _sunset_amount = clamp( 0.5 - abs( LIGHT0_DIRECTION.y ), 0.0, 0.5 ) * 2.0;
	// The sky should be more red around the west, on the opposite side you don't see it as much
	float _sunset_distance = clamp( 1.0 - pow( distance( EYEDIR, LIGHT0_DIRECTION ), 2 ), 0.0, 1.0 );
	vec3 _sky_sunset_color = mix( sunset_bottom_color, sunset_top_color, _eyedir_y + 0.5 );
	_sky_sunset_color = mix( _sky_sunset_color, sunset_bottom_color, _sunset_amount * _sunset_distance );
	_sky_color = mix( _sky_color, _sky_sunset_color, _sunset_amount );

	float _night_amount = clamp( -LIGHT0_DIRECTION.y + 0.7, 0.0, 1.0 );
	vec3 _sky_night_color = mix( night_bottom_color, night_top_color, _eyedir_y );
	_sky_color = mix( _sky_color, _sky_night_color, _night_amount );

	// Final sky color
	COLOR = _sky_color;

	//////////////////// HORIZON ///////////////////////////////////////////////////////////////////
	float _horizon_amount = 0.0;
	if( EYEDIR.y < 0.0 )
	{
		_horizon_amount = clamp( abs( EYEDIR.y ) / horizon_blur, 0.0, 1.0 );
		// Mixing with the color of the night sky to make the horizon darker
		vec3 _horizon_color = mix( horizon_color, _sky_color, _night_amount * 0.9 );
		// And if ther are many dark clouds, we also make the horizon darker
		_horizon_color = mix( _horizon_color, vec3( 0.0 ), ( 1.0 - clouds_cutoff ) * clouds_weight * 0.7 );
		COLOR = mix( COLOR, _horizon_color, _horizon_amount );
	}

	//////////////////// MOON //////////////////////////////////////////////////////////////////////
	float _moon_amount = 0.0;
	if( LIGHT1_ENABLED )
	{
		// Bigger moon near the horizon
		float _moon_size = moon_size + cos( LIGHT1_DIRECTION.y * PI ) * moon_size * 0.25;
		float _moon_distance = distance( EYEDIR, LIGHT1_DIRECTION ) / _moon_size;
		// Finding moon disc and edge blur
		_moon_amount = clamp(( 1.0 - _moon_distance ) / moon_blur, 0.0, 1.0 );
		if( _moon_amount > 0.0 )
		{
			// Moon illumination depending on the position of the sun
			float _moon_intersect = sphere_intersect( EYEDIR, LIGHT1_DIRECTION, _moon_size );
			vec3 _moon_normal = normalize( LIGHT1_DIRECTION - EYEDIR * _moon_intersect );
			// Power on the result gives a better effect
			float _moon_n_dot_l = pow( clamp( dot( _moon_normal, -LIGHT0_DIRECTION ), 0.05, 1.0 ), 2 );
			// Hiding the moon behind the horizon
			_moon_amount *= 1.0 - _horizon_amount;
			COLOR = mix( COLOR, moon_color, _moon_n_dot_l * _moon_amount );
		}
	}

	//////////////////// SUN ///////////////////////////////////////////////////////////////////////
	float _sun_distance = 0.0;
	if( LIGHT0_ENABLED )
	{
		_sun_distance = distance( EYEDIR, LIGHT0_DIRECTION );
		// Bigger sun near the horizon
		float _sun_size = sun_size + cos( LIGHT0_DIRECTION.y * PI ) * sun_size * 0.25;
		// Finding sun disc and edge blur
		float _sun_amount = clamp(( 1.0 - _sun_distance / _sun_size ) / sun_blur, 0.0, 1.0 );
		if( _sun_amount > 0.0 )
		{
			// Changing color of the sun during sunset
			float _sunset_amount = 1.0;
			if( LIGHT0_DIRECTION.y > 0.0 )
				_sunset_amount = clamp( cos( LIGHT0_DIRECTION.y * PI ), 0.0, 1.0 );
			vec3 _sun_color = mix( sun_color, sun_sunset_color, _sunset_amount );
			// Hiding the sun behind the moon
			_sun_amount = clamp( _sun_amount * ( 1.0 - _moon_amount ), 0.0, 1.0 );
			// Hiding the sun behind the horizon
			_sun_amount *= 1.0 - _horizon_amount;
			// Leveling the "glow" in color
			if( _sun_color.r > 1.0 || _sun_color.g > 1.0 || _sun_color.b > 1.0 )
				_sun_color *= _sun_amount;
			COLOR = mix( COLOR, _sun_color, _sun_amount );
		}
	}

	//////////////////// STARS /////////////////////////////////////////////////////////////////
	vec2 _sky_uv = EYEDIR.xy / ( EYEDIR.z );
	if( EYEDIR.y > -0.01 && LIGHT0_DIRECTION.y < 0.0  )
	{
		// Stars UV rotation
		float _stars_speed_cos = cos( stars_speed * time * 0.005 );
		float _stars_speed_sin = sin( stars_speed * time * 0.005 );
		vec2 _stars_uv = vec2(
			_sky_uv.x * _stars_speed_cos - _sky_uv.y * _stars_speed_sin,
			_sky_uv.x * _stars_speed_sin + _sky_uv.y * _stars_speed_cos
		);
		// Stars texture
		float _stars_value = texture( voronoi_texture, _stars_uv ).r * -LIGHT0_DIRECTION.y;

		_stars_value = smoothstep( 0.249, 0.25, _stars_value );
		
		// Hiding stars behind the moon
		_stars_value *= 1.0 - _moon_amount;
		COLOR += _stars_value;
	}

	//////////////////// CLOUDS ////////////////////////////////////////////////////////////////
	// Clouds UV movement direction
	float _clouds_speed = time * clouds_speed * 0.01;
	float _sin_x = sin( clouds_direction * PI * -0.5 );
	float _cos_y = cos( clouds_direction * PI );

	// The speed of movement (and direction a little) is different for the illusion of the changing shape of the clouds.
	vec2 _clouds_movement = vec2( _sin_x * _clouds_speed, 0.0 );
	float _noise_main = texture(voronoi_texture, (_sky_uv + _clouds_movement) * clouds_density).r;
	_noise_main = smoothstep( 0.0, 0.5, _noise_main );

	_clouds_movement = vec2( 0.0, _cos_y * _clouds_speed );
	float _noise_edges = texture(voronoi_texture, (_sky_uv + _clouds_movement) * clouds_density * clouds_roughness).r;
	_noise_edges = 1.0 - smoothstep( 0.0, 2.0, 1.0 - _noise_edges );

	float _clouds_amount = _noise_main + _noise_edges;
	_clouds_amount = smoothstep( 0.0, 2.5, _clouds_amount );
	
	// Fading clouds near the horizon
	_clouds_amount += 1.0 - clamp( (EYEDIR.y - (clouds_fade_point - 1.0)) / clouds_blur, 0.0, 1.0 );

	// Posterize the clouds
	float _posterize_amount = 3.0;
	_clouds_amount = floor(_clouds_amount * _posterize_amount) / (_posterize_amount - 1.0);

	vec3 _clouds_color = mix( COLOR, clouds_color, _clouds_amount );

	// The sun passing through the clouds effect
	_clouds_color = mix( _clouds_color, clamp( sun_color, 0.0, 1.0 ), pow( 1.0 - clamp( _sun_distance, 0.0, 1.0 ), 5 ));
	// Color combined with sunset condition
	_clouds_color = mix( _clouds_color, sunset_bottom_color, _sunset_amount * 0.75 );
	// Color depending on the "progress" of the night.
	_clouds_color = mix( _clouds_color, _sky_color, clamp( _night_amount, 0.0, 0.98 ));
	_clouds_color = mix( _clouds_color, vec3( 0.0 ), clouds_weight * 0.9 );
	COLOR = mix( COLOR, _clouds_color, _clouds_amount );
}