from typing import Dict
import httpx
from langchain_core.tools import tool
from config import ATLAS_VECTOR_SEARCH_INDEX_NAME, DOCUMENTS_COLLECTION, MongoDBAtlasVectorSearch, sync_db, models
from models.user_register import UserRegister, login_user
from Repository.auth import jwt_validator
from langchain_core.runnables import RunnableConfig

client = httpx.AsyncClient(base_url="http://localhost:5063/RailwayTicketBooking")
train_client = httpx.AsyncClient(base_url="http://localhost:5063")

# Vector store for retrieval
vector_store = MongoDBAtlasVectorSearch(
    embedding=models["azure"]["embeddings"],
    collection=sync_db[DOCUMENTS_COLLECTION],
    index_name=ATLAS_VECTOR_SEARCH_INDEX_NAME,
    relevance_score_fn="cosine",
)
llm = models["azure"]["llm"]

@tool()
async def register_user(user_data: UserRegister):
    """Register a new user."""
    result = await client.post("/UserRegister", json=user_data.model_dump())
    return result.json()

@tool()
async def loginUser(login_data: login_user):
    """Login a user."""
    result = await client.post("/login", json=login_data.model_dump())
    return result.json()

@tool()
async def search_trains(source: str, destination: str, date: str):
    """Search for trains between source and destination on a specific date."""
    
    params = {"source": source, "destination": destination, "date": date}
    result = await train_client.get("/api/Train/search", params=params)
    return result.json()

@tool()
async def get_trains():
    """Get all available trains for today."""
    result = await train_client.get("/api/Train/GetTrains")
    return result.json()
    
@tool(response_format="content_and_artifact")
def retrieve(query: str):
    """Retrieve information related to a query from knowledge base. information related to railway management system."""
    retrieved_docs = vector_store.similarity_search(query, k=4)
    
    serialized = "\n\n".join(
        doc.page_content
        for doc in retrieved_docs
    )
    return serialized, retrieved_docs



    
    