import os
import sys
import yaml
import torch
import trimesh

from shap_e.models.download import load_model
from shap_e.util.notebooks import decode_latent_mesh
from shap_e.diffusion.sample import sample_latents
from shap_e.diffusion.gaussian_diffusion import diffusion_from_config

# ✅ Prompt + ID from command-line
if len(sys.argv) < 3:
    print("❌ Usage: python generate_fixed.py \"your prompt here\" run_id")
    sys.exit(1)

prompt = sys.argv[1]
run_id = sys.argv[2]
print(f"📝 Using prompt: {prompt}")
print(f"🆔 Run ID: {run_id}")

# ✅ Create output directory
os.makedirs("results", exist_ok=True)

# ✅ Set device
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

# ✅ Load models
xm = load_model("transmitter", device=device)
model = load_model("text300M", device=device)

# ✅ Load diffusion config
config_path = "shap-e/configs/diffusion/text300M.yaml"
with open(config_path, 'r') as f:
    diffusion_config = yaml.safe_load(f)

diffusion = diffusion_from_config(diffusion_config)

# ✅ Sample latents
latents = sample_latents(
    batch_size=1,
    model=model,
    diffusion=diffusion,
    guidance_scale=15.0,
    model_kwargs=dict(texts=[prompt]),
    progress=True,
    clip_denoised=True,
    use_fp16=True,
    use_karras=True,
    karras_steps=64,
    sigma_min=0.3,
    sigma_max=1.0,
    s_churn=0,
)

# ✅ Export to .obj file
safe_prompt = ''.join(c if c.isalnum() else '_' for c in prompt.lower())

for i, latent in enumerate(latents):
    raw_mesh = decode_latent_mesh(xm, latent)

    mesh = trimesh.Trimesh(
        vertices=raw_mesh.verts.cpu().numpy(),
        faces=raw_mesh.faces.cpu().numpy()
    )

    obj_filename = f"{safe_prompt}_{run_id}.{i}.obj"
    obj_path = os.path.join("results", obj_filename)
    mesh.export(obj_path)

    print(f"✅ Saved OBJ: {obj_path}")
