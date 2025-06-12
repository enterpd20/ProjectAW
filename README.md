![Image](https://github.com/user-attachments/assets/9d5bd81b-f6f9-4362-a239-c82add5919c2)

# Azur Lane: Another Wave [<img src="https://github.com/user-attachments/assets/a6a32091-a55b-4721-adbb-38c79cea22f3"  width="40" height="30"/>](https://youtu.be/ZQeOSHrXYPw)

## 1. Project Overview [프로젝트 개요]

Unity 3D Personal Project - Azur Lane 모작

개발 기간: 24.08.27 ~ 24.10.22


## 2. Key Features [주요 기능]

### 2.1 Character
- **캐릭터 고유 능력치 및 스킬**: 캐릭터마다 고유한 스탯과 스킬을 보유합니다.
- **캐릭터 획득 및 삭제**: Build 메뉴를 통해 캐릭터를 획득하거나 삭제할 수 있습니다.
- **캐릭터 전투 편성**: 전투를 시작하기 전 원하는 캐릭터를 배치할 수 있습니다.

### 2.2 Battle
- **자동 전투 AI**: 캐릭터가 AI 로직에 따라 자동으로 전투를 수행합니다.
  - **FSM(Finite State Machine)**: FSM을 이용하여 캐릭터의 체력, 시야, 공격 범위를 기반으로 행동이 결정됩니다.
- **일시 정지 기능**: 게임 도중 일시 정지 및 재개가 가능합니다.

### 2.3 User Interface
- **Dock**: 현재 보유 중인 캐릭터와 상세정보를 확인할 수 있습니다.
- **Depot**: 캐릭터에 장착된 장비를 확인할 수 있습니다.
- **Build**: 캐릭터를 새로 얻거나 삭제할 수 있습니다.
- **카메라 이동**: 마우스 드래그로 맵 전체를 자유롭게 확인할 수 있습니다.
- **캐릭터 스킬 버튼**: 버튼을 눌러 캐릭터 스킬을 사용할 수 있고, 스킬 사용 후 쿨타임이 UI에 시각적으로 표시됩니다.

### 2.4 ETC
- **JSON 데이터 관리 방식**
- **실시간 물결 셰이더**: 바다에 물결 효과가 적용된 커스텀 셰이더를 사용합니다.
- **사운드 시스템**: 싱글톤 패턴을 사용하여 Master, BGM, SFX를 분리하여 조절 가능합니다.
