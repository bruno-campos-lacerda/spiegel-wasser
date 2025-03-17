extends Control
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_animation_player_animation_finished(anim_name):
	if anim_name == "fade_out":
		$AnimationPlayer.play("fade_in")
		
	if anim_name == "fade_in":
		get_tree().change_scene_to_file("res://assets_menu/MenuInicial/title_screen.tscn")
