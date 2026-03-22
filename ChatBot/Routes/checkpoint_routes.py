from fastapi import APIRouter, HTTPException
from Repository.checkpoint_service import CheckpointService

router = APIRouter(prefix="/checkpoints", tags=["checkpoints"])
checkpoint_service = CheckpointService()

@router.get("/latest")
async def get_latest_checkpoint(thread_id: str):
    try:
        checkpoint = await checkpoint_service.get_latest_checkpoint(thread_id)
        if checkpoint is None:
            raise HTTPException(status_code=404, detail="Checkpoint not found")
        return checkpoint
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/latest_tuple")
async def get_latest_checkpoint_tuple(thread_id: str):
    try:
        checkpoint_tuple = await checkpoint_service.get_latest_checkpoint_tuple(thread_id)
        if checkpoint_tuple is None:
            raise HTTPException(status_code=404, detail="Checkpoint not found")
        return {"latest_checkpoint_tuple": checkpoint_tuple}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/list")
async def get_checkpoint_tuples(thread_id: str):
    try:
        checkpoint_tuples = await checkpoint_service.get_checkpoint_tuples(thread_id)
        return {"checkpoints": checkpoint_tuples, "count": len(checkpoint_tuples)}
    except Exception as e:
        print(f"Checkpoint list error: {e}")
        return {"checkpoints": [], "count": 0}

@router.get("/messages")
async def get_filtered_messages(thread_id: str):
    try:
        messages = await checkpoint_service.get_filtered_messages(thread_id)
        return {
            "checkpoint_data": messages,
            "thread_id": thread_id
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.delete("/thread/{thread_id}")
async def delete_by_thread_id(thread_id: str):
    try:
        result = await checkpoint_service.delete_by_thread_id(thread_id)
        if result["total"] == 0:
            raise HTTPException(status_code=404, detail=f"No documents found with thread_id '{thread_id}'")
        return {
            "message": f"{result['total']} document(s) with thread_id '{thread_id}' have been deleted.",
            "breakdown": {
                "checkpoints": result["checkpoints"],
                "checkpoint_writes": result["checkpoint_writes"]
            }
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/threads")
async def get_user_threads():
    try:
        threads = await checkpoint_service.get_user_threads()
        return {"threads": threads}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))