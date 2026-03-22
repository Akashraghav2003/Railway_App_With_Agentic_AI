from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from Routes import document_loader, AuthRoute, graph_routes, checkpoint_routes

app = FastAPI(
    title="FastAPI capstone chat bot",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:4200", "http://127.0.0.1:4200", "*"],
    allow_credentials=True,
    allow_methods=["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    allow_headers=["*"],
)

@app.get("/")
def read_root():
    return {"message": "Railway AI Chatbot API is running"}

@app.get("/health")
def health_check():
    return {"status": "healthy"}

app.include_router(document_loader.router)
app.include_router(AuthRoute.router)
app.include_router(graph_routes.router)
app.include_router(checkpoint_routes.router)

