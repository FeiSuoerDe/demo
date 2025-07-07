extends ColorRect

@export_dir var BackgroundSpritesPath:String
var Textures:Array

func _ready() -> void:
	var sprites = DirAccess.get_files_at(BackgroundSpritesPath)
	for sprite in sprites:
		var texture = load(BackgroundSpritesPath+'/'+sprite)
		Textures.append(texture)

func _on_timer_timeout() -> void:
	new_sprite_animation()

func new_sprite_animation() -> void:
	var now_texture = Textures.pick_random()
	var sprite2d = Sprite2D.new()
	var start_x = randf_range(1000.,1250.)
	var start_pos = Vector2(start_x, 1152+648-start_x)
	sprite2d.texture = now_texture
	sprite2d.position - start_pos
	add_child(sprite2d)
	var tween = create_tween()
	tween.tween_property(sprite2d,"position",start_pos,-start_pos)
