# -*- coding: utf-8 -*-

import os
import json
from typing import Dict, Any, Optional
import openai

OPENAI_KEY = os.getenv("OPENAI_API_KEY")
MODEL = os.getenv("OPENAI_MODEL", "gpt-4o-mini")
MOCK_MODE = os.getenv("MOCK_MODE", "true").lower() in ("1","true","yes")

def parse_message_mock(message: str) -> Dict[str, Any]:
    # Reglas simples de prueba
    m = message.lower()
    if any(w in m for w in ["tienen", "hay", "horario", "env�o","envios","pago","pagos","precio"]):
        return {"type":"faq","reply":"S�, tenemos. �Qu� necesitas exactamente?","product":None,"quantity":None,"address":None}
    # detectar un n�mero y palabra "tornillo" o "martillo"
    if "tornillo" in m or "tornillos" in m or "martillo" in m:
        qty = 1
        import re
        r = re.search(r"(\d+)", m)
        if r: qty = int(r.group(1))
        # simplifico product
        product = "tornillo 8mm" if "tornillo" in m else "martillo"
        return {"type":"order","reply":f"Confirm�s {qty} x {product}? Dame direcci�n y tel�fono.","product":product,"quantity":qty,"address":None}
    return {"type":"unknown","reply":"No entend�, te paso con un agente humano.","product":None,"quantity":None,"address":None}

def call_openai_and_parse(message: str) -> Dict[str, Any]:
    if OPENAI_KEY is None:
        raise RuntimeError("OPENAI_API_KEY no definido")
    openai.api_key = OPENAI_KEY

    prompt = f"""
    Eres un asistente para una ferreter�a. Analiza el mensaje:
    \"{message}\"

    Devuelve SOLO JSON con las claves:
      type -> "faq" o "order" o "unknown"
      reply -> breve texto para el cliente (resumen/confirmaci�n)
      product -> nombre del producto si es pedido, o null
      quantity -> entero o null
      address -> texto si detecta direcci�n, o null

    Responde con JSON v�lido estrictamente.
    """

    resp = openai.ChatCompletion.create(
        model=MODEL,
        messages=[{"role":"user","content":prompt}],
        temperature=0
    )

    text = resp.choices[0].message["content"]
    try:
        data = json.loads(text)
    except Exception:
        # si el modelo no devolvi� JSON v�lido, devolver fallback
        return {"type":"unknown","reply":"No pude procesar el pedido, te paso con un agente.","product":None,"quantity":None,"address":None}
    return data

def analyze_message(message: str) -> Dict[str, Any]:
    if MOCK_MODE:
        return parse_message_mock(message)
    return call_openai_and_parse(message)
