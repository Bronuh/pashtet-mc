shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform float lod: hint_range(0.0, 5, 0.1) = 2.0;
uniform float brightness: hint_range(0.0, 1.0, 0.1) = 1.0;
uniform float transparency: hint_range(0.0, 1.0, 0.1) = 0.5;

void fragment(){
	vec4 bg_color = texture(SCREEN_TEXTURE, SCREEN_UV, lod);
	vec4 texture_color = COLOR;
	vec4 darkened_color = bg_color * brightness;
	
	darkened_color.a = bg_color.a;
	vec4 resulting_color = mix(texture_color, darkened_color, transparency);
	COLOR = resulting_color;
}