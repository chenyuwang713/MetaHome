from flask import Flask, request, jsonify
import subprocess
import uuid

app = Flask(__name__)

@app.route('/generate_glb', methods=['POST'])
def generate_glb():
    try:
        data = request.get_json()
        prompt = data.get('prompt', '').strip()

        if not prompt:
            return jsonify({'error': 'Missing prompt'}), 400

        print("🛠️ Generating 3D asset for prompt:", prompt)

        # ✅ Generate a unique run_id to avoid filename collisions
        run_id = str(uuid.uuid4())[:8]

        # ✅ Run the generation script
        result = subprocess.run(
            ['python', 'generate_fixed.py', prompt, run_id],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )

        if result.returncode != 0:
            print("❌ Generation failed:\n", result.stderr)
            return jsonify({'error': 'Generation failed', 'details': result.stderr}), 500

        # ✅ Extract all GLB filenames and generate URLs
        glb_urls = []
        for line in result.stdout.splitlines():
            if "✅ Uploaded to GitHub:" in line and ".glb" in line:
                filename = line.split(":")[-1].strip()
                url = f"https://raw.githubusercontent.com/raghava7261/3D-Models/main/{filename}"
                glb_urls.append(url)

        if not glb_urls:
            return jsonify({'error': 'No GLB URLs found in logs'}), 500

        return jsonify({'glb_urls': glb_urls}), 200

    except Exception as e:
        return jsonify({'error': str(e)}), 500

