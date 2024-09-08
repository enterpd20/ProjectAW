using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    // ���°� ���۵� �� ȣ��
    // ���·� ��ȯ�� �� �ʱ� ������ ����
    public void EnterState();

    // �� �����Ӹ��� ȣ��
    // ���°� Ȱ�� ���� �� ����Ǵ� ����
    public void UpdateState();

    // ���°� ����� �� ȣ��
    // ���¸� �������� �� �ʿ��� ���� �۾��� ����
    public void ExitState();
}
