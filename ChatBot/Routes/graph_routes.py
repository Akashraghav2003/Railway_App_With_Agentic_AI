from fastapi import APIRouter, HTTPException, Header
from pydantic import BaseModel
from typing import Optional
from Graph.graph import async_graph
from Repository.auth import jwt_validator

router = APIRouter(prefix="/graph", tags=["Graph Operations"])

class QuestionRequest(BaseModel):
    question: str
    thread_id: str

@router.post("/async-agent")
async def process_question(
    request: QuestionRequest,
    authorization: Optional[str] = Header(None)
):
    user_info = {}

    
    if authorization and authorization.startswith("Bearer "):
        token = authorization.split(" ")[1]
        try:
            user_info = jwt_validator(token)
            print(f"Token validated: {user_info.get('email', 'No email')} - Role: {user_info.get('role', 'No role')}")
        except HTTPException as e:
            print(f"Token validation failed: {e.detail}")
            user_info = {}
    else:
        print("No valid authorization header provided")

    config = {
        "configurable": {
            "thread_id": request.thread_id,
            "user_info": user_info,
        }
    }

    try:
        response_events = []

        async for event in async_graph.astream(
            {"messages": [("user", request.question)], "user_info": user_info}, config, stream_mode="values"
        ):
            response_events.append(event)

        if response_events:
            last_event = response_events[-1]
            if "messages" in last_event and last_event["messages"]:
                return {"content": last_event["messages"][-1].content}
            else:
                raise HTTPException(status_code=404, detail="No messages in response")
        else:
            raise HTTPException(status_code=404, detail="No events received")

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))