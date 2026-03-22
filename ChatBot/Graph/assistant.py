from langchain_core.runnables import Runnable, RunnableConfig
from langgraph.graph.message import AnyMessage
from pydantic import BaseModel, Field
from typing import Optional
from config import models
from tools.reservation_tools import book_ticket, cancel_ticket, get_reservation
from tools.train_tools import add_train, update_train, cancel_train, get_all_trains
from tools.user_tools import retrieve, register_user, loginUser, search_trains, get_trains
from Graph.message_state import State

llm = models["azure"]["llm"]
class Assistant:
    def __init__(self, runnable: Runnable):
        self.runnable = runnable
 
    def __call__(self, state: State, config: RunnableConfig):
        # Add user_info to config for tools
        enhanced_config = {
            **config,
            "configurable": {
                **config.get("configurable", {}),
                "user_info": state.get("user_info", {})
            }
        }
        
        while True:
            result = self.runnable.invoke(state, config=enhanced_config)
            if not result.tool_calls and (
                not result.content
                or isinstance(result.content, list)
                and not result.content[0].get("text")
            ):
                messages = state["messages"] + [("user", "Please respond with a proper output.")]
                state = {**state, "messages": messages}
            else:
                break
        return {"messages": [result]}

class CompleteOrEscalate(BaseModel):
    """A tool to mark the current task as completed and/or to escalate control of the dialog to the main assistant,
    who can re-route the dialog based on the user's needs."""

    cancel: bool = True
    reason: str

class ToTrainAssistant(BaseModel):
    """Transfers work to a specialized assistant to handle train management tasks"""
    request: str = Field(
        description="Any necessary follow up questions the train assistant should ask the user before proceeding with the task."
    )

class ToReservationAssistant(BaseModel):
    """Transfers work to a specialized assistant to handle reservation-related tasks"""
    train_id: Optional[str] = Field(
        description="Optional field, train ID for booking or checking reservations"
    )
    reservation_id: Optional[str] = Field(
        description="Optional field, reservation ID for cancelling or checking specific reservation"
    )
    request: str = Field(
        description="Any necessary follow up questions the reservation assistant should ask the user before proceeding with the task."
    )


reservation_system_message_content = (
    "You are the Reservation Assistant for Railway Ticket Booking System"
    "TOOLS AVAILABLE: book_ticket, cancel_ticket, get_reservation, complete_or_escalate. "
    "AUTHENTICATION REQUIRED: Verify user has valid token and 'user' role before any booking/cancellation operations. "
    "STRICT RULES: "
    "- Only use provided tools, never generate fake data "
    "-DO NOT book ticket for PAST date "
     "Do not respond any random question which is not related to application. For Example: who is the president of USA?, Where is the eiffel tower located? etc.. "
    "-only work for rerservation (ticket) related operation otherwise decide for complete or escalate tool"
    "- Check user_info for token and role validation "
    "- If authentication fails, immediately ask user to login "
    "-Convert the input date string to ISO 8601 format (2025-10-06T06:15:44.832Z) for the required backend format."
    "- Complete booking/cancellation only after successful tool execution "
    "- For booking: require trainId, source, destination, date, passengers "
    "- For cancellation: require valid reservationId "
    "\n\nCurrent User Info: {user_info}"
)

reservation_tool = [book_ticket, cancel_ticket, get_reservation]
from langchain_core.prompts import ChatPromptTemplate

reservation_prompt = ChatPromptTemplate.from_messages([
    ("system", reservation_system_message_content),
    ("placeholder", "{messages}")
])
reservation_runnable = reservation_prompt | llm.bind_tools(reservation_tool + [CompleteOrEscalate])




train_system_message_content = (
    "You are the Train Assistant for Railway Ticket Booking System. "
    "TOOLS AVAILABLE: add_train, update_train, cancel_train, get_all_trains, complete_or_escalate. "
    "ADMIN ACCESS ONLY: Verify user has valid token and 'admin' role before any train management operations. "
    "STRICT RULES: "
    "Do not respond any random question which is not related to application. For Example: who is the president of USA?, Where is the eiffel tower located? etc.. "
    "-only work for train related operation other decide for complete or escalate tool"
    "-DO NOT search train for PAST date "
    "- Only use provided tools, never create fake train data "
    "- Check user_info for token and admin role validation "
    "- If user is not admin, deny access and explain admin requirement "
    "- Convert the input date string to ISO 8601 format (2025-10-06T06:15:44.832Z) for the required backend format."
    "- For add_train: take data based on the train schema which is used in the tool "
    "- For update_train: require trainId and updated train details "
    "- For cancel_train: require valid trainId "
    "\n\nCurrent User Info: {user_info}"
)

trains_tool = [add_train, update_train, cancel_train, get_all_trains]
train_prompt = ChatPromptTemplate.from_messages([
    ("system", train_system_message_content),
    ("placeholder", "{messages}")
])
train_runnable = train_prompt | llm.bind_tools(trains_tool + [CompleteOrEscalate])





user_system_message_content = (
    "You are the Primary Assistant for Railway Ticket Booking System. "
    "TOOLS AVAILABLE: register_user, loginUser, search_trains, get_trains, retrieve. "
    "ROUTING: Use ToReservationAssistant for booking/cancellation, ToTrainAssistant for train management. "
    "AUTHENTICATION STATUS: Check user_info to see if user is logged in. If user_info has 'token' and 'role', user is authenticated. "
    "- If user wants to manage tickets : ONLY route to ToReservationAssistant if role is 'user'. If role is 'admin', immediately deny with message: 'Only users with user role can book tickets. You are logged in as admin. Please login with a user account to book tickets.'"
    "- If user wants to manage trains : ONLY route to ToTrainAssistant if role is 'admin'. If role is 'user', deny access. "
    "- If user is not authenticated but wants restricted actions, suggest they login first "
     "Do not respond any random question which is not related to application. For Example: who is the president of USA?, Where is the eiffel tower located? etc.. "
    "RETRIEVAL TOOL RULES: "
    "- Use 'retrieve' tool ONLY for railway system documentation questions "
    "STRICT RULES: "
    "- Use only provided tools, never assume user data "
    "- DO NOT WORK FOR PAST DATES "
    "- For registration: require email, password, name, phone "
    "- For login: require email and password "
    "- For train search: require source, destination, date "
    "- for search train for today use the get_trains tool "
    "\n\nCurrent User Info: {user_info}"
)

user_tool = [retrieve, register_user, loginUser, search_trains, get_trains]
user_prompt = ChatPromptTemplate.from_messages([
    ("system", user_system_message_content),
    ("placeholder", "{messages}")
])
user_runnable = user_prompt | llm.bind_tools(user_tool + [ToTrainAssistant, ToReservationAssistant], parallel_tool_calls=False)


