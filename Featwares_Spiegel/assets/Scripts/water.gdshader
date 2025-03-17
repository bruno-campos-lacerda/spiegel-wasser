shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D noise1 : repeat_enable;
uniform sampler2D noise2 : repeat_enable;
uniform vec2 scroll1 = vec2(0.5, 0.05);
uniform vec2 scroll2 = vec2(-0.5, -0.05);
uniform float disturbion_strenght : hint_range(-1.0, 1.0) = 0.2;
uniform vec4 tone_color : source_color;

void fragment() {
	float depth = texture(noise1, UV + scroll1 * TIME).r 
		* texture(noise2, UV + scroll2 * TIME).r;
	vec4 screen_col = texture(SCREEN_TEXTURE, 
		SCREEN_UV + disturbion_strenght * vec2(depth));
	COLOR = screen_col * tone_color;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
