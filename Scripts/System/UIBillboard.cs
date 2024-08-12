using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 이름 표시 UI 빌보드 처리
public class UIBillboard : MonoBehaviour
{
	void LateUpdate()
	{
		transform.LookAt(transform.position + Camera.main.transform.forward);
	}
}
