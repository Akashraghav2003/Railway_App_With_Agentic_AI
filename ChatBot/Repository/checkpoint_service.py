from langgraph.checkpoint.mongodb import MongoDBSaver
from config import async_client, mongo_uri, database_name
from pymongo import MongoClient

class CheckpointService:
    def __init__(self):
        try:
            sync_client = MongoClient(mongo_uri)
            self.async_memory = MongoDBSaver(
                client=sync_client,
                db_name=database_name
            )
        except Exception as e:
            print(f"Checkpoint service init error: {e}")
            self.async_memory = None
        
        self.db = async_client[database_name]
        self.checkpoint_collection = self.db["checkpoints"]
        self.checkpoint_writes_collection = self.db["checkpoint_writes"]
    
    async def get_latest_checkpoint(self, thread_id: str):
        if not self.async_memory:
            return None
        config = {"configurable": {"thread_id": thread_id}}
        try:
            return await self.async_memory.aget(config)
        except:
            return None
    
    async def get_latest_checkpoint_tuple(self, thread_id: str):
        if not self.async_memory:
            return None
        config = {"configurable": {"thread_id": thread_id}}
        try:
            return await self.async_memory.aget_tuple(config)
        except:
            return None
    
    async def get_checkpoint_tuples(self, thread_id: str):
        if not self.async_memory:
            return []
        config = {"configurable": {"thread_id": thread_id}}
        try:
            checkpoints = []
            async for item in self.async_memory.alist(config):
                checkpoints.append(item)
            return checkpoints
        except Exception as e:
            print(f"Error getting checkpoint tuples: {e}")
            return []
    
    async def get_filtered_messages(self, thread_id: str):
        latest_checkpoint = await self.get_latest_checkpoint_tuple(thread_id)
        
        if not latest_checkpoint or len(latest_checkpoint) < 2:
            return []
        
        checkpoint_data = latest_checkpoint[1]
        if "channel_values" not in checkpoint_data or "messages" not in checkpoint_data["channel_values"]:
            return []
        
        raw_messages = checkpoint_data["channel_values"]["messages"]
        formatted_messages = []
        
        for msg in raw_messages:
            formatted_msg = self._format_message(msg)
            if formatted_msg:
                formatted_messages.append(formatted_msg)
        
        formatted_messages.sort(key=lambda x: x.get('timestamp') or 0, reverse=True)
        return formatted_messages
    
    async def delete_by_thread_id(self, thread_id: str):
        result1 = await self.checkpoint_collection.delete_many({"thread_id": thread_id})
        result2 = await self.checkpoint_writes_collection.delete_many({"thread_id": thread_id})
        return {
            "checkpoints": result1.deleted_count,
            "checkpoint_writes": result2.deleted_count,
            "total": result1.deleted_count + result2.deleted_count
        }
    
    async def get_user_threads(self):
        try:
            pipeline = [
                {"$group": {"_id": "$thread_id", "count": {"$sum": 1}}},
                {"$sort": {"count": -1}}
            ]
            
            cursor = self.checkpoint_collection.aggregate(pipeline)
            threads = [{
                "thread_id": doc["_id"],
                "last_updated": None
            } async for doc in cursor]
            return threads
        except Exception as e:
            print(f"Error getting user threads: {e}")
            return []
    
    def _format_message(self, msg):
        if hasattr(msg, 'type') and msg.type == 'tool':
            return None
        
        if hasattr(msg, 'type') and hasattr(msg, 'content'):
            content = msg.content
            if isinstance(content, str) and "Source: {" in content:
                return None
            return {
                "type": msg.type,
                "content": content,
                "timestamp": getattr(msg, 'timestamp', None)
            }
        elif isinstance(msg, dict):
            content = msg.get('content', '')
            if isinstance(content, str) and "Source: {" in content:
                return None
            return {
                "type": msg.get('type', 'unknown'),
                "content": content,
                "timestamp": msg.get('timestamp')
            }
        return None