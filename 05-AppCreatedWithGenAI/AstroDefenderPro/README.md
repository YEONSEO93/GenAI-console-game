# AstroDefender Pro

![AstroDefender Pro](./images/astrodefender-pro-demo.gif)

**AstroDefender Pro**는 원래 SpaceAINet을 기반으로 대폭 업그레이드된 우주 전투 게임입니다. .NET 9로 개발되었으며, 수동 제어 모드와 AI 모드를 모두 지원합니다. 현재는 완전한 수동 제어 모드로 설정되어 있어 플레이어가 직접 게임을 조작할 수 있습니다.

## 🎮 새로운 기능들

### ⭐ 게임플레이 업그레이드
- **수동 제어 모드**: 완전한 플레이어 제어, 적 AI 비활성화
- **안정적인 렌더링**: 화면 스크롤링 없는 부드러운 게임 플레이
- **다양한 적 타입**: 일반 적, 빠른 적, 강한 적, 보스 적 (정적 상태)
- **파워업 시스템**: 더블샷, 트리플샷, 레이저, 실드, 체력 회복
- **다중 무기 시스템**: 기본 총알, 레이저, 미사일, 스프레드샷
- **레벨 시스템**: 점진적 난이도 증가와 새로운 적 패턴
- **보스 전투**: 각 레벨 끝에 등장하는 강력한 보스들
- **체력 시스템**: 플레이어와 적의 체력 관리

### 🤖 AI 기능 (현재 비활성화)
- **수동 제어 우선**: 현재 모든 AI 기능이 비활성화되어 완전한 수동 제어 모드
- **고급 전략 분석**: 파워업 우선순위, 보스 패턴 인식 (향후 재활성화 가능)
- **적응형 난이도**: AI 성능에 따른 동적 난이도 조절 (비활성화)
- **멀티 모델 지원**: Azure OpenAI, Ollama, 로컬 AI 모델들 (비활성화)

### 🎨 시각적 개선
- **컬러풀한 이펙트**: 폭발, 레이저 빔, 파워업 글로우
- **향상된 UI**: 실시간 스탯, 미니맵, 체력바
- **파티클 효과**: 적 파괴시 파티클 애니메이션

## 🏗️ 솔루션 구조

- **AstroDefenderPro.Console**: 메인 콘솔 게임 프로젝트
- **AstroDefenderPro.GameEngine**: 게임 로직, 엔티티, 렌더링 엔진
- **AstroDefenderPro.AI**: AI 모델 통합 및 고급 전략 분석

## 🛠️ 시스템 요구사항

- [.NET 9 이상](https://dotnet.microsoft.com/download/)
- **로컬 AI (Ollama)용:**
  - [Ollama](https://ollama.com/) 로컬 실행 (기본: `http://localhost:11434`)
  - 지원 모델 (예: `ollama run llama3.2`, `ollama run phi4-mini`)
- **클라우드 AI [Azure AI Foundry](https://ai.azure.com/)용:**
  - Azure OpenAI 접근 권한 및 배포된 채팅 모델
  - Azure AI 엔드포인트, 모델명, API 키

## 🚀 설치 및 실행

### 1. 저장소 클론

```bash
git clone <this-repo-url>
cd AstroDefenderPro
```

### 2. Azure OpenAI 설정 (선택사항)

Azure OpenAI를 사용하려면 [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)를 설정하세요:

```bash
cd AstroDefenderPro.Console
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"
```

### 3. Ollama 실행 (선택사항)

로컬 모델을 사용하려면:

- [Ollama 설치](https://ollama.com/download)
- 모델 다운로드: `ollama pull llama3.2`
- Ollama 실행 (기본: `http://localhost:11434`)

### 4. 게임 빌드 및 실행

```bash
dotnet build
cd AstroDefenderPro.Console
dotnet run
```

## 🎮 게임 조작법

| 키 | 동작 |
|---|---|
| **LEFT/RIGHT 화살표** | 우주선 좌우 이동 |
| **SPACE** | 기본 공격 |
| **Z** | 특수 무기 발사 (파워업 필요) |
| **X** | 실드 활성화 (파워업 필요) |
| **C** | 슬로우 모션 (제한적) |
| **P** | 일시정지/재개 |
| **Q** | 게임 종료 |
| **A** | Azure OpenAI AI 모드 토글 (현재 비활성화) |
| **O** | Ollama AI 모드 토글 (현재 비활성화) |
| **F** | FPS 표시 토글 |
| **S** | 스크린샷 저장 |
| **1/2/3/4** | 게임 속도 선택 |
| **ENTER** | 기본 속도로 시작 |

## 🎯 게임 시스템

### 파워업 종류
- **🔫 DoubleShot**: 동시에 2발 발사
- **🔱 TripleShot**: 동시에 3발 발사  
- **⚡ Laser**: 관통 레이저 빔
- **🛡️ Shield**: 일시적 무적
- **❤️ Health**: 체력 회복
- **💨 Speed**: 이동속도 증가

### 적 타입
- **👾 Basic Enemy**: 기본적인 적 (체력: 1, 현재 정적)
- **🏃 Fast Enemy**: 빠르게 움직이는 적 (체력: 1, 현재 정적)
- **💪 Strong Enemy**: 강한 적 (체력: 3, 현재 정적)
- **🔴 Boss Enemy**: 보스 적 (체력: 10+, 현재 정적)

### AI 전략 요소 (현재 비활성화)
현재 수동 제어 모드에서는 모든 AI 기능이 비활성화되어 있습니다:
- ~~적의 위치와 타입 분석~~
- ~~파워업 우선순위 판단~~
- ~~보스 패턴 인식 및 대응~~
- ~~체력 관리 및 위험 회피~~
- ~~점수 최적화 전략~~

플레이어가 직접 모든 전략적 결정을 내리며 게임을 조작합니다.

## 📊 성능 모니터링

게임은 실시간으로 다음 정보를 표시합니다:
- **Game FPS**: 게임 렌더링 프레임율
- **AI FPS**: AI 분석 프레임율
- **Score**: 현재 점수
- **Level**: 현재 레벨
- **Health**: 플레이어 체력
- **AI Decision**: AI의 현재 결정과 이유

## 🤝 기여하기

이슈나 제안사항이 있으시면 저장소의 이슈 트래커에 보고해 주세요.

---

**AstroDefender Pro**로 완전한 수동 제어 레트로 게임 경험을 즐겨보세요! 🚀✨

## 📝 변경 로그

### v1.1.0 - 수동 제어 모드
- ✅ 모든 적 AI 움직임 비활성화
- ✅ 화면 스크롤링 문제 해결
- ✅ 완전한 플레이어 제어 구현
- ✅ 안정적인 렌더링 시스템
- ✅ 향상된 사용자 인터페이스
