import bpy
import sys

# Read arguments passed after the double dash (--)
argv = sys.argv
argv = argv[argv.index("--") + 1:]

if len(argv) != 2:
    print("Usage: blender --background --python export_glb.py -- input.obj output.glb")
    sys.exit(1)

input_path = argv[0]
output_path = argv[1]

# Delete all objects
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# Import the .obj file
bpy.ops.import_scene.obj(filepath=input_path)

# Select all imported objects
for obj in bpy.context.scene.objects:
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj

# Export as .glb (glTF 2.0)
bpy.ops.export_scene.gltf(
    filepath=output_path,
    export_format='GLB',
    export_texcoords=True,
    export_normals=True,
    export_materials='EXPORT',
    export_colors=True,
    export_cameras=False,
    export_selected=False,
    use_selection=False
)

print(f"Successfully exported to {output_path}")
