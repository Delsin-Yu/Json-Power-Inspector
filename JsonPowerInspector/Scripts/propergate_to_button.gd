extends Label;

@export var button : Button;

func _ready():
	mouse_filter = MOUSE_FILTER_STOP;

func _gui_input(event):
	
	var mouse_button_event := event as InputEventMouseButton;
	
	if (
		mouse_button_event == null ||
		mouse_button_event.button_index != 1 ||
		!mouse_button_event.is_pressed()
	) : return;
	
	button.button_pressed = !button.button_pressed;

