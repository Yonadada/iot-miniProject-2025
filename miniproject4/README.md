# 미니프로젝트 4

## Python AI (API 연동) + ASP.NET Core 연동 프로젝트

### Python

#### 파이썬 웹 애플리케이션 프레임워크
1. **Django**  
   - 대규모 웹 애플리케이션 개발에 사용되는 풀스택 프레임워크  
   - 구조화가 잘 되어 있으나 상대적으로 무거움  

2. **Flask**  
   - 소규모 웹 애플리케이션 개발에 적합한 마이크로 프레임워크  
   - 가볍지만 필요한 기능을 개발자가 직접 구현해야 함  

3. **Uvicorn**  
   - 초경량 ASGI 서버, FastAPI와 함께 주로 사용됨  
   - 매우 가볍고 빠른 성능 제공  

---

#### FastAPI
- RESTful API를 손쉽게 만들 수 있는 웹 프레임워크  
- **Uvicorn**과 함께 실행하여 높은 성능과 비동기 지원 제공  

#### 패키지 설치
```shell
pip install fastapi uvicorn
```

[소스](./pythonAI/step1/main01.py)

#### 데이터 유효성 검사 패키지 pydantic

[소스](./pythonAI/step1/main02.py)

#### 전체 통합 

[소스](./pythonAI/step1/main03.py)

---

#### ASP.NET Core
- 파이썬에서 만들어져서 uvicorn으로 전달되는 데이터를 수신받아서 표현하는 웹앱
- ASP.NET.Core 비어있음으로 생성, MVC 생성 시 필요없는 파일이 다수 발생하기 때문
- HTTPS를 선택 해제

[소스](./backend/ASPWebSolution/TestWebApp/Program.cs)

#### 파이썬 웹서버 송신 데이터 처리
- html에서 javaScript로 처리
- ASP.NET Core API 경유 처리


### 파이썬 AI Server 구현

#### 필요 라이브러리
- fastapi
- uvicorn
- pydantic
- Pillow : 이미지 열기, 저장 
- numpy: 수치 연산
- requests: HTTP 로 요청
- opencv-python : 이미지, 비디오 처리
- python-multipart : 멀티파트(이미지, 비디오) 파싱
- ultralytics : YOLO 이미지, 동영상 객체 탐지

```shell
> pip install Pillow numpy requests opencv-python python-multipart
```

- ultralytics : YOLO 이미지, 동영상 객체탐지
    - ultralytics 
```shell
> pip install torch torchvision --index-url https://download.pytorch.org/whl/cu126
```
<img src="../image/mp0019.png" width="600">


#### AI Server
- 웹 서버 실행

[소스](./pythonAI/step2/main01.py)
