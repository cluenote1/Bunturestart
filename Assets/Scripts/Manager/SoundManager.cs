using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private Transform soundGroup;  // 사운드 그룹 오브젝트를 인스펙터에서 연결할 수 있습니다.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 이 오브젝트를 유지합니다.
        }
        else
        {
            Destroy(gameObject);  // 중복되는 인스턴스는 파괴합니다.
        }
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");  // 새 오브젝트를 생성합니다.
        go.transform.parent = soundGroup;  // 사운드 그룹 오브젝트의 자식으로 설정합니다.
        AudioSource audioSource = go.AddComponent<AudioSource>();  // AudioSource 컴포넌트를 추가합니다.
        audioSource.volume = PlayerPrefs.GetFloat("SFXScale", 1f);  // 저장된 SFX 볼륨을 사용합니다.
        audioSource.clip = clip;  // 클립 설정
        audioSource.Play();  // 사운드를 재생합니다.

        Destroy(go, clip.length);  // 사운드가 끝나면 오브젝트를 삭제합니다.
    }
}
