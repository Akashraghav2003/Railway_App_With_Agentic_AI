from httpx import AsyncClient
from models.train import TrainDTO
from langchain_core.tools import tool
from langchain_core.runnables import RunnableConfig

client = AsyncClient(base_url="http://localhost:5063/api/Train")

@tool()
async def add_train(train_data: TrainDTO, config: RunnableConfig = None):
    """Add a new train. Only authenticated admin users can add trains."""
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    token = user_info.get("token")
    role = user_info.get("role")

    # print(f"token : {token}")
    # print(f"role : {role}")

    if not token:
        return "User not authenticated. Please login first."
    
    if not role or role.lower() != "admin":
        return "Only admin users are allowed to add trains."
    
    headers = {"Authorization": f"Bearer {token}"}
    response = await client.post("/AddTrain", headers=headers, json=train_data.model_dump())
    
    if response.status_code == 200:
        result = response.json()
        return result.get("Message", "Train added successfully!")
    return f"Failed to add train: {response.text}"

@tool()
async def update_train(train_id: int, train_data: TrainDTO, config: RunnableConfig = None):
    """Update an existing train. Only authenticated admin users can update trains."""
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    token = user_info.get("token")
    role = user_info.get("role")
    
    if not token:
        return "User not authenticated. Please login first."
    
    if not role or role.lower() != "admin":
        return "Only admin users are allowed to update trains."
    
    headers = {"Authorization": f"Bearer {token}"}
    response = await client.put(f"/UpdateTrain?trainID={train_id}", headers=headers, json=train_data.model_dump())
    
    if response.status_code == 200:
        result = response.json()
        return result.get("Message", "Train updated successfully!")
    return f"Failed to update train: {response.text}"

@tool()
async def cancel_train(train_id: int, config: RunnableConfig = None):
    """Cancel/delete a train. Only authenticated admin users can cancel trains."""
    user_info = config.get("configurable", {}).get("user_info", {}) if config else {}
    token = user_info.get("token")
    role = user_info.get("role")

    if not token:
        return "User not authenticated. Please login first."
    
    if not role or role.lower() != "admin":
        return "Only admin users are allowed to cancel trains."
    
    headers = {"Authorization": f"Bearer {token}"}
    response = await client.delete(f"/CancelTrain/{train_id}", headers=headers)
    
    if response.status_code == 200:
        result = response.json()
        return result.get("Message", "Train cancelled successfully!")
    return f"Failed to cancel train: {response.text}"

@tool()
async def get_all_trains():
    """
    Get all trains. Public endpoint - no authentication required.
    """
    response = await client.get("/GetAllTrains")
    
    if response.status_code == 200:
        result = response.json()
        # Return the full response for debugging
        return result
    else:
        return f"Failed to get trains: {response.text}"