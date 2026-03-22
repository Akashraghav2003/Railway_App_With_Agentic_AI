from pydantic import BaseModel
from typing import Optional

class DocumentUploadRequest(BaseModel):
    url: Optional[str] = None
    user_id: str
    
class DocumentUploadResponse(BaseModel):
    message: str
    user_id: str