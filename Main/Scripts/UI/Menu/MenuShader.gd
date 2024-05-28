extends MenuEffect
class_name MenuShader

class ShaderValue:
	enum ShaderValueType
	{
		VALUE,
		COLOR,
		ALPHA
	}

	@export var shader_value_type: ShaderValueType = ShaderValueType.VALUE
	@export var shader_value_name: String
	@export var shader_value: float
	@export var shader_color: Color

	func lerp(value: ShaderValue, t: float):
		match value.shader_value_type:
			ShaderValueType.VALUE:
				shader_value = lerp(shader_value, value.shader_value, t)
			ShaderValueType.COLOR:
				shader_color = shader_color.lerp(value.shader_color, t)
			ShaderValueType.ALPHA:
				shader_value = lerp(shader_value, value.shader_value, t)

@export var selected_value: Resource
@export var deselected_value: Resource

var shader_value: ShaderValue

var mat: ShaderMaterial

func selected_effect():

	super.selected_effect()
	delay_effect()
	tween.tween_property(self, "menu_scale", 1.0, effect_time).set_trans(effect_transition_type)

func deselected_effect():

	super.deselected_effect()
	delay_effect()
	tween.tween_property(self, "menu_scale", 0.0, effect_time).set_trans(effect_transition_type)

func set_shader_value(value: ShaderValue):
	mat = material as ShaderMaterial
	if not mat:
		printerr("No ShaderMaterial found!")
		return
		
	shader_value = value
