# 주요 설계
## CardData / CardInstance 분리
<details>
<summary>자세히 보기</summary>

- CardData는 원본 데이터를 관리합니다.
- CardInstance는 카드 강화, 과부하 등 전투 중 변경되는 데이터를 관리합니다.
- 원본 데이터를 수정하지 않고 런타임 데이터만 변경하도록 설계했습니다.
</details>

## ICardEffect 기반 카드 효과 분리
- 카드 효과의 재사용을 고려해 인터페이스로 분리했습니다.

## EventBus
- 스크립트 간 직접 참조를 줄여 의존성을 낮추기 위해 EventBus를 사용했습니다.

## ISelectable
<details>
<summary>자세히 보기</summary>

- 초기에는 몬스터만 카드의 대상이었기 때문에 IHealth만 사용했습니다.
- 과부하 카드가 추가되면서 카드 역시 선택 대상이 되어야 했지만 IHealth를 구현하는 것은 맞지 않는다고 판단했습니다.
- 선택 가능 여부만 나타내는 마커 인터페이스인 ISelectable을 만들고 카드의 대상이 될 인터페이스(ICard, IHealth)가 이를 상속하도록 변경했습니다.
- 새로운 선택 대상이 추가되더라도 ISelectable을 기반으로 확장할 수 있도록 설계했습니다.
</details>

## 데이터 관리
- ScriptableObject(몬스터, 플레이어)와 Json(카드, 상태이상)을 사용했습니다.
