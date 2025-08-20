# 통합
from fastapi import FastAPI, HTTPException      # HTTPException는 웹상에서 발생하는 예외처리 클래스
from pydantic import BaseModel

app = FastAPI()

class Item(BaseModel):          #클라이언트로 전송할 데이터 형식 클래스 
    name: str
    desc: str = None
    price: float
    tax: float = None

# 가상 DB
items = {}          # 정보를 담을 딕셔너리 <==> json

# 기본 URL 확인 메세지 
@app.get('/')
async def getRoot():
    return {'Greeting' : 'Hello FastAPI'}


# post에서 저장한 데이터를 확인하는 함수 
@app.get('/items/{id}')
async def getItem(id: int):
    if id not in items:
        raise HTTPException(status_code=404, detail='Item not found')
    
    return items[id]

# 데이터 생성
@app.post('/items')
async def setItem(item: Item):
    id = len(items) + 1         # 0부터 시작하니까 + 1 더해서 id 생성
    items[id] = item
    
    # ** 딕셔너리를 키 = 값 , 쌍의 형태로 풀어서 함수전달 또는 새로운 딕셔너리 만들때 사용
    # *args는 위치 인자를 튜플로 받음 
    # **kwargs는 키워드 인자를 딕셔너리로 받음  
    return {'id' : id, **item.model_dump()}