extends CanvasLayer
@onready var resume_btn = $bg_overlay/menu_holder/resume_btn
@onready var quit_btn = $bg_overlay/menu_holder/quit_btn
@onready var animator = $animator


func _ready():
	visible = false
 
func _unhandled_input(event):
	if event.is_action_pressed("ui_cancel"):
		visible = true
		animator.play("pause_game")
		get_tree().paused = true
		resume_btn. grab_focus()
