extends ColorRect

# 导出变量，用于指定背景精灵的路径
@export_dir var BackgroundSpritesPath:String
# 存储加载的纹理数组
var Textures:Array

# 场景准备函数，加载背景精灵的纹理
func _ready() -> void:
	# 获取指定路径下的所有文件
	var sprites = DirAccess.get_files_at(BackgroundSpritesPath)
	for sprite in sprites:
		# 加载每个文件为纹理并添加到数组中
		var texture = load(BackgroundSpritesPath+'/'+sprite)
		Textures.append(texture)

# 定时器超时信号处理函数，触发新的精灵动画
func _on_timer_timeout() -> void:
	new_sprite_animation()

# 创建新的精灵动画
func new_sprite_animation() -> void:
	# 随机选择一个纹理
	var now_texture = Textures.pick_random()
	# 创建一个新的 Sprite2D 节点
	var sprite2d = Sprite2D.new()
	# 随机生成起始位置
	var start_x = randf_range(1920.,2048)
	var start_pos = Vector2(start_x, 2048+648-start_x)
	# 设置精灵的纹理和初始位置
	sprite2d.texture = now_texture
	sprite2d.position = start_pos
	# 设置初始旋转角度
	sprite2d.rotation_degrees -= 45
	# 将精灵添加为子节点
	add_child(sprite2d)
	# 创建一个 Tween 动画
	var tween = create_tween()
	# 设置动画属性：从起始位置开始
	tween.tween_property(sprite2d,"position",start_pos,0)
	# 设置动画属性：移动到目标位置
	tween.tween_property(sprite2d,"position",-start_pos,10)
	# 动画完成后释放精灵节点
	tween.tween_callback(sprite2d.queue_free)
