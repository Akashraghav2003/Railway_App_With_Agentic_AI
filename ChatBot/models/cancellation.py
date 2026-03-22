from pydantic import BaseModel, Field

class CancellationDTO(BaseModel):
    PNRNumber: int = Field(..., description="Enter the PNR Number")
    Reason: str = Field(..., min_length=1, description="Enter the Reason")
    ReservationId: int = Field(..., description="Enter the reservation number")