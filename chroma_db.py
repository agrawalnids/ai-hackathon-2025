from fastapi import FastAPI, Body
from pydantic import BaseModel
import chromadb
from chromadb.config import Settings


client = chromadb.Client(Settings(
    persist_directory="chroma-store"
))

collection = client.get_or_create_collection("my_collection")

app = FastAPI()

class EmbeddedInput(BaseModel):
    text: str
    metadata: dict
    id: str
    embedding: list
    
class QueryInput(BaseModel):
    embedding: list
    n: int

@app.post("/add")
def add_document(data: EmbeddedInput):
    try:
        collection.add(
            ids=[data.id],
            metadatas=[data.metadata],
            embeddings=[data.embedding],
            documents=[data.text]
        )
        return {"message": "Document added successfully"}
    except ValidationError as e:
        raise HTTPException(status_code=422, detail=e.errors())

    
    
@app.post("/query")
def query_document(data: QueryInput):

    result = collection.query(
        query_embeddings=[data.embedding],
        n_results=data.n
    )
    print(result)
    return result
        