from fastapi import FastAPI
from pydantic import BaseModel
from dotenv import load_dotenv
from utils.nlp import analyze_message
from fastapi.middleware.cors import CORSMiddleware

load_dotenv()  # carga .env si existe

app = FastAPI(title="AI Service for Ferreteria", version="1.0")

# Si necesitás que Angular llame directo: configurar CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],   # en prod restringir
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

class MessageRequest(BaseModel):
    message: str
    customer_phone: str | None = None

class ProcessResponse(BaseModel):
    type: str
    reply: str
    product: str | None = None
    quantity: int | None = None
    address: str | None = None

@app.get("/health")
async def health():
    return {"status":"ok"}

@app.post("/process", response_model=ProcessResponse)
async def process(req: MessageRequest):
    """
    Endpoint principal: recibe mensaje, devuelve JSON con clasificación y entidades.
    """
    result = analyze_message(req.message)
    # aseguramos las claves mínimas
    return ProcessResponse(
        type = result.get("type","unknown"),
        reply = result.get("reply",""),
        product = result.get("product"),
        quantity = result.get("quantity"),
        address = result.get("address"),
    )
