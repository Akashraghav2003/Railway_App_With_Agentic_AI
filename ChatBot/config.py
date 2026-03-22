from dotenv import load_dotenv
import os
from langchain_openai import AzureChatOpenAI, AzureOpenAIEmbeddings
from langchain_google_genai import ChatGoogleGenerativeAI, GoogleGenerativeAIEmbeddings
from langchain_mongodb import MongoDBAtlasVectorSearch
from motor.motor_asyncio import AsyncIOMotorClient
from pymongo import MongoClient

load_dotenv()

mongo_uri = os.getenv("mongo_uri")
database_name = os.getenv("DB_NAME", "capstone_project")
collection_name = os.getenv("DOCUMENTS_COLLECTION", "documents")

# Async Database connection for user/workspace operations and checkpoints
try:
    async_client = AsyncIOMotorClient(
        mongo_uri,
        serverSelectionTimeoutMS=5000,
        connectTimeoutMS=10000
    )
    client = async_client
    db = client[database_name]
    users_collection = db["users"]
    workspaces_collection = db["workspaces"]
    
    # Sync Database connection for vector operations
    sync_client = MongoClient(
        mongo_uri,
        serverSelectionTimeoutMS=5000,
        connectTimeoutMS=10000
    )
    sync_db = sync_client[database_name]
    print("MongoDB connection established successfully")
except Exception as e:
    print(f"MongoDB connection failed: {e}")
    # Fallback to None - handle gracefully in routes
    async_client = None
    sync_client = None
    db = None
    sync_db = None

GOOGLE_API_KEY = os.getenv("GOOGLE_API_KEY")
MONGO_URI = os.getenv("MONGO_URI")
DB_NAME = os.getenv("DB_NAME", "new_project")
DOCUMENTS_COLLECTION = os.getenv("DOCUMENTS_COLLECTION", "documents")
ATLAS_VECTOR_SEARCH_INDEX_NAME = os.getenv("ATLAS_VECTOR_SEARCH_INDEX_NAME", "vector_capstone")
models = {
    "azure": {
        "llm": AzureChatOpenAI(
            api_version=os.getenv("AZURE_API_VERSION"),
            azure_endpoint=os.getenv("AZURE_ENDPOINT"),
            api_key=os.getenv("AZURE_API_KEY"),
            azure_deployment=os.getenv("AZURE_DEPLOYMENT"),
            temperature=0.3,
            top_p=0.8,
        ),
        "embeddings": AzureOpenAIEmbeddings(
            azure_deployment=os.getenv("AZURE_EMBEDDING_DEPLOYMENT"),
            azure_endpoint=os.getenv("AZURE_ENDPOINT"),
            api_key=os.getenv("AZURE_API_KEY"),
            model=os.getenv("AZURE_EMBEDDING_MODEL"),
        )
    },
    "gemini": {
        "llm": ChatGoogleGenerativeAI(
            model=os.getenv("GOOGLE_LLM_MODEL"),
            google_api_key=GOOGLE_API_KEY,
            temperature=0.3,
            top_p=0.8
        ),
        "embeddings": GoogleGenerativeAIEmbeddings(
            model=os.getenv("GOOGLE_EMBEDDING_MODEL"),
            google_api_key=GOOGLE_API_KEY
        )
    }
}
