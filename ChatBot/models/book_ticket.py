from pydantic import BaseModel, Field
from typing import List, Optional

class PassengerDTO(BaseModel):
    Name: str = Field(..., max_length=100, description="Name is required")
    Age: int = Field(..., ge=0, le=120, description="Age must be between 0 and 120")
    Gender: str = Field(...,description="Gender must be Male, Female, or Other")
    AdharCard: int = Field(..., description="AdharCard is required")

class BookTicket(BaseModel):
    UserID: int = Field(..., description="UserID is required")
    TrainId: int = Field(..., description="TrainId is required")
    TravelDate: str = Field(..., description="TravelDate is required (ISO format)")
    ClassId: int = Field(..., description="ClassId is required")
    NoOfSeats: int = Field(..., ge=1, description="NoOfSeats must be at least 1")
    Quota: str = Field(..., max_length=50, description="Quota is required")
    BankName: Optional[str] = Field(None, max_length=100, description="BankName can't be longer than 100 characters")
    Passenger: List[PassengerDTO] = Field(..., min_items=1, description="At least one passenger is required")