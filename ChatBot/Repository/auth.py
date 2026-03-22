import typing
from fastapi import HTTPException, status
import jwt
from jwt import PyJWTError
import os 

SECRET_KEY = os.getenv("Key")
ALGORITHM = os.getenv("ALGORITHM", "HS256")
ISSUER = os.getenv("Issuer")
AUDIENCE = os.getenv("Audience")


def jwt_validator(token: str) -> dict:
    try:
        payload = jwt.decode(
            token, 
            SECRET_KEY, 
            algorithms=[ALGORITHM],
            issuer=ISSUER,
            audience=AUDIENCE
        )
        return {
            "valid": True,
            "UserId": payload.get("UserId"),
            "email": payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"),
            "role": payload.get("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),
            "token": token
        }
    
    except PyJWTError as e:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Could not validate credentials",
            headers={"WWW-Authenticate": "Bearer"},
        ) 