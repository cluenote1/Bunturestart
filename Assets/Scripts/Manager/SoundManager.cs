using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private Transform soundGroup;  // ���� �׷� ������Ʈ�� �ν����Ϳ��� ������ �� �ֽ��ϴ�.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �ÿ��� �� ������Ʈ�� �����մϴ�.
        }
        else
        {
            Destroy(gameObject);  // �ߺ��Ǵ� �ν��Ͻ��� �ı��մϴ�.
        }
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");  // �� ������Ʈ�� �����մϴ�.
        go.transform.parent = soundGroup;  // ���� �׷� ������Ʈ�� �ڽ����� �����մϴ�.
        AudioSource audioSource = go.AddComponent<AudioSource>();  // AudioSource ������Ʈ�� �߰��մϴ�.
        audioSource.volume = PlayerPrefs.GetFloat("SFXScale", 1f);  // ����� SFX ������ ����մϴ�.
        audioSource.clip = clip;  // Ŭ�� ����
        audioSource.Play();  // ���带 ����մϴ�.

        Destroy(go, clip.length);  // ���尡 ������ ������Ʈ�� �����մϴ�.
    }
}
