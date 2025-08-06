# AstroDefender Pro - 사용 가이드

## 🎮 게임 실행 방법

### 1. 빌드 및 실행
```bash
cd AstroDefenderPro
dotnet build
cd AstroDefenderPro.Console
dotnet run
```

### 2. 게임 조작법

#### 기본 조작
- **←/→ 화살표**: 우주선 좌우 이동
- **SPACE**: 기본 무기 발사
- **Z**: 특수 무기 발사 (파워업 필요)
- **X**: 실드 활성화 (파워업 필요)
- **C**: 슬로우 모션 토글
- **P**: 일시정지
- **Q**: 게임 종료

#### AI 조작
- **A**: Azure OpenAI AI 모드 토글
- **O**: Ollama AI 모드 토글

#### 기타
- **F**: FPS 표시 토글
- **S**: 스크린샷 저장
- **1-4**: 게임 속도 선택 (시작 시)

### 3. 게임 시스템

#### 적 타입
- **기본 적 (><)**: 체력 1, 점수 10
- **빠른 적 (»«)**: 체력 1, 점수 20
- **강한 적 (███)**: 체력 3, 점수 50
- **보스 적**: 체력 15+, 점수 500

#### 파워업 종류
- **D (DoubleShot)**: 동시에 2발 발사
- **T (TripleShot)**: 동시에 3발 발사
- **L (Laser)**: 관통 레이저 빔
- **S (Shield)**: 일시적 무적
- **H (Health)**: 체력 회복
- **F (Speed)**: 이동속도 증가

#### 무기 타입
- **Basic**: 기본 총알
- **Double**: 이중 발사
- **Triple**: 삼중 발사
- **Laser**: 레이저 빔
- **Missile**: 유도 미사일
- **Spread**: 산탄형 발사

### 4. AI 설정

#### Azure OpenAI 설정
```bash
cd AstroDefenderPro.Console
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-endpoint"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "your-model-name"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your-api-key"
```

#### Ollama 설정
1. [Ollama 설치](https://ollama.com/download)
2. 모델 다운로드: `ollama pull llama3.2`
3. Ollama 실행 (자동으로 http://localhost:11434에서 실행)

### 5. 게임 전략

#### 기본 전략
1. **적 총알 회피**: 빨간색 총알을 피하세요
2. **파워업 수집**: 특히 체력과 실드 파워업 우선
3. **적절한 포지셔닝**: 적의 공격 범위를 벗어나면서 공격
4. **보스 전투**: 안전한 거리를 유지하고 지속적으로 공격

#### AI 전략
- AI는 위험 요소를 분석하고 최적의 행동을 선택합니다
- 파워업 우선순위를 자동으로 판단합니다
- 보스 패턴을 인식하고 대응합니다
- 체력 관리를 자동으로 수행합니다

### 6. 트러블슈팅

#### 게임이 실행되지 않을 때
1. .NET 9가 설치되어 있는지 확인
2. 콘솔 창 크기가 충분한지 확인
3. 터미널이 UTF-8 인코딩을 지원하는지 확인

#### AI가 작동하지 않을 때
1. Azure OpenAI 설정이 올바른지 확인
2. Ollama 서비스가 실행 중인지 확인
3. 네트워크 연결 상태 확인

### 7. 향후 개선 사항

- 멀티플레이어 지원
- 사운드 효과 추가
- 더 많은 적 타입
- 커스텀 레벨 에디터
- 온라인 리더보드

---

**즐거운 게임 되세요! 🚀✨**
