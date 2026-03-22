from fastapi import APIRouter, UploadFile, File, HTTPException
from models.document import DocumentUploadRequest, DocumentUploadResponse
from Repository.document_loader import DocumentLoaderService

router = APIRouter(prefix="/documents", tags=["documents"])
loader_service = DocumentLoaderService()

@router.post("/upload-pdf", response_model=DocumentUploadResponse)
async def upload_pdf(user_id: str, file: UploadFile = File(...)):
    if not file.filename.endswith('.pdf'):
        raise HTTPException(status_code=400, detail="Only PDF files are allowed")
    
    pdf_bytes = await file.read()
    
    result = await loader_service.load_pdf_from_bytes(pdf_bytes, file.filename,user_id)
    
    return DocumentUploadResponse(
        message=result["message"],
        user_id=user_id
    )