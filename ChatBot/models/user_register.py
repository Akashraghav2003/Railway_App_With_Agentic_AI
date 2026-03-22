from pydantic import BaseModel, EmailStr, Field, conint

class UserRegister(BaseModel):
    Name: str = Field(min_length=3, max_length=50)
    Gender : str
    Age : int = Field(gt=15, lt=100)
    Address : str = Field(min_length=10, max_length=100)
    Email: EmailStr
    Phone: int = Field(gt=1000000000, lt=9999999999) 
    UserName : str
    Password: str = Field(min_length=6, max_length=20)
    

class login_user(BaseModel):
    EmailOrUserName : str
    Password: str = Field(min_length=6, max_length=20)