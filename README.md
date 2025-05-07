MetaHome

MetaHome is a Virtual Reality (VR) Smart Home system built in Unity that integrates AI-generated 3D models and interactive voice/text-based control. It uses generative models like OpenShape/Shape-E to produce 3D assets from text prompts and incorporates an LLM-based assistant for dynamic interaction.

Features

Text-to-3D asset generation using Shape-E or OpenShape
Automated OBJ to GLB conversion via Blender
Runtime 3D model import in Unity
Voice or text prompt-based asset spawning
Context-aware floating UI for object manipulation
Real-time stylization of spawned assets
Integration with Meta LLaMA or Gemini API
Backend powered by Flask for prompt processing and model generation
Project Structure

File	Description
| File                  | Description                                                |
| --------------------- | ---------------------------------------------------------- |
| `GLBLoader.cs`        | Downloads and imports GLB models into Unity at runtime     |
| `InputUI.cs`          | Handles prompt input from the Unity interface              |
| `Llama_server.py`     | Flask API to interact with Meta LLaMA                      |
| `Llama.cs`            | Unity-side HTTP client for sending prompts to Flask server |
| `ObjectMover.cs`      | Allows users to move and rotate spawned assets             |
| `Openshape__e.py`     | Generates 3D mesh from prompt using OpenShape/Shape-E      |
| `PromptToGLB.cs`      | Controls GLB download and assignment in Unity              |
| `RuntimeEditorUI.cs`  | Displays object-specific interaction menus in Unity        |
| `floatingmenucode.cs` | UI logic for radial floating menus around 3D objects       |
| `flask_server.py`     | Handles Shape-E or OpenShape prompt generation endpoints   |


User enters a text prompt in Unity
The prompt is sent to a Flask server powered by LLaMA
Flask triggers OpenShape/Shape-E to generate an OBJ model
Blender script (export_glb.py) automatically converts the OBJ to GLB
Unity's GLBLoader.cs downloads and imports the GLB model into the scene
User can then interact with the model using UI buttons or voice
Setup Instructions

Unity Project
Requires Unity 2022 or later with XR Plugin Management and OpenXR enabled
Add WebGL or Android build support for Meta Quest
Clone this repository and open it in Unity Hub
Backend (Flask + Model)
# Activate the correct environment
conda activate shap-e

# Run the Flask server
python flask_server.py

# To use LLaMA-based prompt parsing:
python llama_server.py
Blender Automation
Install Blender 3.2+ and ensure it's accessible via your PATH or direct command:

blender --background --python export_glb.py -- ./results/input.obj ./results/output.glb
Output

All generated GLB files are stored in the results/ directory. These assets can either be dragged manually into Unity or loaded automatically through GLBLoader.cs.

Future Improvements

Add voice recognition with Whisper or WebSpeech
Blockchain metadata for ownership tracking
Dynamic stylization using Text2Mesh integration
WebXR deployment for browser-based access
