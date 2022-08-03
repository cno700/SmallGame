using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody; // ���ǵĸ������
    public float forceMin; // �Ե���ʩ��һ����
    public float forceMax;

    float lifetime = 4; // ��lifetime֮��ʼ����
    float fadetime = 2;

    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(forceMin, forceMax); // Random.Range����Ϊ���������������
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force); // ���һ��Ť��/ת�أ�ʹ����ת

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColour = mat.color;


        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
