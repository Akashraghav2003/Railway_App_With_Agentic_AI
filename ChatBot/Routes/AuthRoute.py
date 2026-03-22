from fastapi import HTTPException, APIRouter, UploadFile, File
from Repository.auth import jwt_validator

router = APIRouter(prefix = "/auth", tags = ["Authentication"])

@router.post("/validate-token")
async def validate_token(token: str):
    try:
        payload = jwt_validator(token)
        return {"valid": True, "payload": payload}
    except HTTPException as e:
        raise HTTPException(status_code=e.status_code, detail=e.detail)
    
