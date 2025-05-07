from flask import Flask, request, jsonify
import torch
from transformers import AutoTokenizer, AutoModelForCausalLM, pipeline

MODEL_NAME = "meta-llama/Llama-2-7b-chat-hf"

# Load tokenizer and model with authentication
print("🔁 Loading tokenizer and model...")
tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME, use_auth_token=True)
model = AutoModelForCausalLM.from_pretrained(
    MODEL_NAME,
    device_map="auto",
    torch_dtype=torch.float16,
    use_auth_token=True
)

# Create pipeline
llama_pipeline = pipeline("text-generation", model=model, tokenizer=tokenizer)

app = Flask(__name__)

@app.route('/query', methods=['POST'])
def generate_reply():
    data = request.get_json()
    prompt = data.get("query", "")

    if not prompt:
        return jsonify({"error": "Missing 'query' parameter"}), 400

    print("📥 Prompt received:", prompt)

    result = llama_pipeline(prompt, max_new_tokens=100, do_sample=True, temperature=0.7)[0]["generated_text"]
    response = result[len(prompt):].strip()

    print("📤 Response:", response)
    return jsonify({"response": response})

if __name__ == "__main__":
    print("✅ LLaMA 2 API is ready!")
    app.run(host='0.0.0.0', port=5001)



