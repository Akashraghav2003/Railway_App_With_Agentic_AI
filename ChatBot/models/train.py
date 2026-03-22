from pydantic import BaseModel, Field
from typing import List
from datetime import datetime
from decimal import Decimal

class TrainClassDTO(BaseModel):
    className: str = Field(..., max_length=50, description="className is required")
    totalSeat: int = Field(..., ge=1, description="totalSeat must be at least 1")
    fare: float = Field(..., ge=0, description="fare must be a positive value")

class TrainDTO(BaseModel):
    trainName: str = Field(..., max_length=100, description="trainName is required")
    sourceStation: str = Field(..., max_length=100, description="sourceStation is required")
    destinationStation: str = Field(..., max_length=100, description="destinationStation is required")
    departureTime: str = Field(..., description="departureTime is required (ISO format)")
    arrivalTime: str = Field(..., description="arrivalTime is required (ISO format)")
    totalSeats: int = Field(..., ge=1, description="totalSeats must be at least 1")
    trainClass: List[TrainClassDTO] = Field(..., min_items=1, description="At least one trainClass is required")