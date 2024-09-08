using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    // 상태가 시작될 때 호출
    // 상태로 전환될 때 초기 설정을 수행
    public void EnterState();

    // 매 프레임마다 호출
    // 상태가 활성 중일 때 실행되는 로직
    public void UpdateState();

    // 상태가 종료될 때 호출
    // 상태를 빠져나올 때 필요한 정리 작업을 수행
    public void ExitState();
}
