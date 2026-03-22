from httpx import AsyncClient
from models.book_ticket import BookTicket
from models.cancellation import CancellationDTO
from langchain_core.tools import tool
from langchain_core.runnables import RunnableConfig

client = AsyncClient(base_url="http://localhost:5063/api/Train")

@tool()
async def get_reservation(config: RunnableConfig = None):
    """Get reservation details for a specific user."""
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    userId = user_info.get("UserId")
    
    if not userId:
        return "Failed to get reservation: User not authenticated"
    
    try:
        response = await client.get(f"/Reservatrion/{int(userId)}")
        return response.json()
    except Exception as e:
        return f"Failed to get reservation: {str(e)}"

@tool()
async def book_ticket(ticket_data: BookTicket, config: RunnableConfig = None):
    """
    LLM-safe version of Book Ticket tool.
    Returns strings or dicts instead of raising exceptions.
    """
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    token = user_info.get("token")
    role = user_info.get("role")
    user_id = user_info.get("UserId")

    if not token or not user_id:
        return {"status": "error", "message": " User not authenticated. Please login first."}

    if not role or role.lower() != "user":
        return {"status": "error", "message": " Only users are allowed to book tickets."}

    ticket_data.UserID = int(user_id)

    headers = {"Authorization": f"Bearer {token}"}
    response = await client.post(
        "/AddReservation",
        headers=headers,
        json=ticket_data.model_dump()
    )
    try:
        if response.status_code != 200:
            return f"Booking failed: Status {response.status_code} - {response.text}"
        return {
            "status": "success",
            "message": " Ticket booked successfully!",
            "data": response.json()
        }
    except Exception as e:
        return f"Booking failed: {str(e)}"

@tool()
async def cancel_ticket(cancellation_data: CancellationDTO, config: RunnableConfig = None):
    """
    Cancel a ticket reservation. Only authenticated users can cancel their tickets.
    """
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    token = user_info.get("token")
    role = user_info.get("role")
    
    if not token:
        return "User not authenticated. Please login first."
    
    if not role or role.lower() != "user":
        return "Only users are allowed to cancel tickets."
    
    headers = {"Authorization": f"Bearer {token}"}
    response = await client.post(
        "/CancelReservation",
        headers=headers,
        json=cancellation_data.model_dump()
    )
    
    try:
        if response.status_code != 200:
            return f"Cancellation failed: Status {response.status_code} - {response.text}"
        return f"Ticket cancelled successfully! {response.json()}"
    except Exception as e:
        return f"Cancellation failed: {str(e)}"