from typing import List
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.document_loaders import PyPDFLoader, WebBaseLoader
from langchain_community.vectorstores import MongoDBAtlasVectorSearch
from langchain.schema import Document
from config import models, DOCUMENTS_COLLECTION, ATLAS_VECTOR_SEARCH_INDEX_NAME, MONGO_URI, DB_NAME, sync_db
import tempfile
import os
from itertools import islice

class DocumentLoaderService:
    def __init__(self):
        self.text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=1000,
            chunk_overlap=200,
            length_function=len
        )
    
    def batch_iterable(self, iterable, batch_size):
        """Yield successive batches from the iterable"""
        iterator = iter(iterable)
        for first in iterator:
            yield [first] + list(islice(iterator, batch_size - 1))
    
    def process_pdf(self, pdf_path: str, user_id: str, split: bool = True):
        try:
            if not pdf_path:
                raise ValueError("No PDF path provided")

            loader = PyPDFLoader(pdf_path)
            pages = loader.load()

            if not pages:
                raise ValueError("No pages found in the PDF")

            # Add metadata
            for page in pages:
                page.metadata["user_id"] = user_id
                page.metadata["source_type"] = "pdf"
                page.metadata["source_path"] = pdf_path

            docs = pages
            if split:
                docs = self.text_splitter.split_documents(pages)

            if not docs:
                raise ValueError("No documents generated after splitting")

            batch_size = 15
            for batch in self.batch_iterable(docs, batch_size):
                MongoDBAtlasVectorSearch.from_documents(
                    documents=batch,
                    embedding=models["azure"]["embeddings"],
                    collection=sync_db[DOCUMENTS_COLLECTION],
                    index_name=ATLAS_VECTOR_SEARCH_INDEX_NAME
                )

            return {"status": "success", "message": "PDF uploaded in batches"}

        except (ValueError, IOError) as e:
            raise RuntimeError(f"Error processing PDF: {e}") from e
        except Exception as e:
            raise RuntimeError(f"Unexpected error while processing PDF: {str(e)}") from e


    async def load_pdf_from_bytes(self, pdf_bytes: bytes, filename: str,user_id: str):
        """Load PDF from bytes and return result"""
        with tempfile.NamedTemporaryFile(delete=False, suffix=".pdf") as tmp_file:
            tmp_file.write(pdf_bytes)
            tmp_path = tmp_file.name
        
        try:
            result = self.process_pdf(tmp_path, user_id)
            return result
        finally:
            os.unlink(tmp_path)

    async def load_url(self, url: str,  user_id: str):
        """Load URL content and return result"""
        return self.process_url(url, user_id)