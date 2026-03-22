import os
from typing import Callable, Literal
from langchain_core.messages import ToolMessage
from langchain_core.runnables import RunnableLambda

from langgraph.graph import StateGraph, START, END
from langgraph.prebuilt import tools_condition, ToolNode


from Graph.message_state import State
from Graph.assistant import (
    Assistant, CompleteOrEscalate, ToReservationAssistant, ToTrainAssistant,
    reservation_runnable, train_runnable, user_runnable,
    reservation_tool, trains_tool, user_tool
)

from config import database_name, mongo_uri
from pymongo import MongoClient
from langgraph.checkpoint.mongodb import MongoDBSaver

def create_entry_node(assistant_name: str, new_dialog_state: str) -> Callable:
    def entry_node(state: State) -> dict:
        tool_call_id = state["messages"][-1].tool_calls[0]["id"]
        return {
            "messages": [
                ToolMessage(
                    content=f"The assistant is now the {assistant_name}. Reflect on the above conversation between the host assistant and the user."
                    f" The user's intent is unsatisfied. Use the provided tools to assist the user. Remember, you are {assistant_name},"
                    " and the reservation, train or any other action is not complete until after you have successfully invoked the appropriate tool."
                    " If the user changes their mind or needs help for other tasks, call the CompleteOrEscalate function to let the primary host assistant take control."
                    " Do not mention who you are - just act as the proxy for the assistant.",
                    tool_call_id=tool_call_id,
                )
            ],
            "dialog_state": new_dialog_state,
        }
    return entry_node

def handle_tool_error(state) -> dict:
    error = state.get("error")
    tool_calls = state["messages"][-1].tool_calls
    return {
        "messages": [
            ToolMessage(
                content=f"Error: {repr(error)}\n please fix your mistakes.",
                tool_call_id=tc["id"],
            )
            for tc in tool_calls
        ]
    }

def create_tool_node_with_fallback(tools: list) -> dict:
    return ToolNode(tools).with_fallbacks(
        [RunnableLambda(handle_tool_error)], exception_key="error"
    )

def pop_dialog_state(state: State) -> dict:
    """Pop the dialog stack and return to the main assistant."""
    messages = []
    if state["messages"][-1].tool_calls:
        messages.append(
            ToolMessage(
                content="Resuming dialog with the host assistant. Please reflect on the past conversation and assist the user as needed.",
                tool_call_id=state["messages"][-1].tool_calls[0]["id"],
            )
        )
    return {
        "dialog_state": "pop",
        "messages": messages,
    }

def user_info(state: State):
    # Preserve existing user_info if it exists, otherwise initialize empty
    return {"user_info": state.get("user_info", {})}

# Build the graph
builder = StateGraph(State)

# Add user info fetching node
builder.add_node("fetch_user_info", user_info)
builder.add_edge(START, "fetch_user_info")

# Reservation Assistant
builder.add_node("enter_reservation", create_entry_node("Reservation Assistant", "reservation"))
builder.add_node("reservation", Assistant(reservation_runnable))
builder.add_edge("enter_reservation", "reservation")
builder.add_node("reservation_tools", create_tool_node_with_fallback(reservation_tool))

def route_reservation(state: State):
    route = tools_condition(state)
    if route == END:
        return END
    tool_calls = state["messages"][-1].tool_calls
    did_cancel = any(tc["name"] == CompleteOrEscalate.__name__ for tc in tool_calls)
    if did_cancel:
        return "leave_skill"
    return "reservation_tools"

builder.add_edge("reservation_tools", "reservation")
builder.add_conditional_edges(
    "reservation",
    route_reservation,
    ["reservation_tools", "leave_skill", END]
)

# Train Assistant
builder.add_node("enter_train", create_entry_node("Train Assistant", "train"))
builder.add_node("train", Assistant(train_runnable))
builder.add_edge("enter_train", "train")
builder.add_node("train_tools", create_tool_node_with_fallback(trains_tool))

def route_train(state: State):
    route = tools_condition(state)
    if route == END:
        return END
    tool_calls = state["messages"][-1].tool_calls
    did_cancel = any(tc["name"] == CompleteOrEscalate.__name__ for tc in tool_calls)
    if did_cancel:
        return "leave_skill"
    return "train_tools"

builder.add_edge("train_tools", "train")
builder.add_conditional_edges(
    "train",
    route_train,
    ["train_tools", "leave_skill", END]
)

# Leave skill node (shared)
builder.add_node("leave_skill", pop_dialog_state)
builder.add_edge("leave_skill", "primary_assistant")

# Primary Assistant
builder.add_node("primary_assistant", Assistant(user_runnable))
builder.add_node("primary_assistant_tools", create_tool_node_with_fallback(user_tool))

def route_primary_assistant(state: State):
    route = tools_condition(state)
    if route == END:
        return END
    tool_calls = state["messages"][-1].tool_calls
    if tool_calls:
        if tool_calls[0]["name"] == ToReservationAssistant.__name__:
            return "enter_reservation"
        elif tool_calls[0]["name"] == ToTrainAssistant.__name__:
            return "enter_train"
    return "primary_assistant_tools"

builder.add_conditional_edges(
    "primary_assistant",
    route_primary_assistant,
    ["primary_assistant_tools", "enter_reservation", "enter_train", END],
)
builder.add_edge("primary_assistant_tools", "primary_assistant")

def route_to_workflow(state: State) -> Literal["primary_assistant", "reservation", "train"]:
    """Route to the appropriate assistant based on dialog state."""
    dialog_state = state.get("dialog_state")
    if not dialog_state:
        return "primary_assistant"
    return dialog_state[-1]

builder.add_conditional_edges("fetch_user_info", route_to_workflow)

# Setup async memory and compile graph
# Create MongoDB client for checkpointer
try:
    sync_mongo_client = MongoClient(mongo_uri)
    async_memory = MongoDBSaver(
        client=sync_mongo_client,
        db_name=database_name
    )
    print("Checkpoint memory initialized successfully")
except Exception as e:
    print(f"Checkpoint initialization failed: {e}")
    async_memory = None

async_graph = builder.compile(checkpointer=async_memory)

# Generate and save graph visualization
try:
    mermaid_png = async_graph.get_graph().draw_mermaid_png()
    
    script_dir = os.path.dirname(os.path.abspath(__file__))
    file_name = "railway_graph.png"
    file_path = os.path.join(script_dir, file_name)

    with open(file_path, "wb") as f:
        f.write(mermaid_png)

    print(f"Graph visualization saved to {file_path}")
except Exception as e:
    print(f"An error occurred while generating graph visualization: {e}")