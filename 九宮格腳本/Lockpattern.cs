using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Lockpattern : MonoBehaviour
{
    public static Lockpattern Instance;
    //����u������
    public GameObject linePrefab;

    //����e��
    public Canvas canvas;

    private Dictionary<int, Circleldentifier> circles;

    private List<Circleldentifier> lines;

    private GameObject lineOnEdit;
    private RectTransform lineOnEditRcTs;
    private Circleldentifier circleOnEdit;

    //�]�߬O�_�i�����ʧ@
    public bool unlocking;

    new bool enabled = true;

    public bool �˼Ʈɶ� = false;

    List<�E�c���D��> ���������D�� = new List<�E�c���D��>();

    public void �K�[�D�� (�E�c���D�� �D��)
    {
        ���������D��.Add(�D��);
        //�ؼа}�C = �E�c�檺����;
        //���w�Ѧҹ�(���׹�);
        //���w�Ѧҹϰ_�I(���׹ϰ_�I);
        �]�w�@�D�D��();
    }

    void �]�w�@�D�D��()
    {
        if (���������D�� != null && ���������D��.Count > 0)
        {
            �ؼа}�C = ���������D��[0].�E�c�浪��;
            ���w�Ѧҹ�(���������D��[0].���׹�);
            ���w�Ѧҹϰ_�I(���������D��[0].���׹ϰ_�I);
        }
    }

    public void �M���D��()
    {
        ���������D��.Clear();
    }

    void Start()
    {
        circles = new Dictionary<int, Circleldentifier>();
        lines = new List<Circleldentifier>();


        for (int i = 0; i < transform.childCount; i++)
        {
            var circle = transform.GetChild(i);

            var identifier = circle.GetComponent<Circleldentifier>();

            identifier.id = i;

            circles.Add(i, identifier);
        }

        �˼Ʈɶ� = false;

    }


    // Update is called once per frame
    void Update()
    {
        if (enabled == false)
        {
            return;
        }

        if (unlocking)
        {
            Vector3 mousePos = canvas.transform.InverseTransformPoint(Input.mousePosition);

            //���u�i�H��۷ƹ���m���e
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(mousePos, circleOnEdit.transform.localPosition));

            //���u�i�H���s
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (mousePos - circleOnEdit.transform.localPosition).normalized);
        }

    }

    IEnumerator Release()
    {
        enabled = false;

        yield return new WaitForSeconds(3);

        foreach (var circle in circles)
        {
            circle.Value.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            circle.Value.GetComponent<Animator>().enabled = false;
        }

        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();

        lineOnEdit = null;
        lineOnEditRcTs = null;
        circleOnEdit = null;

        enabled = true;
    }

    GameObject CreateLine(Vector3 pos, int id)
    {
        var line = GameObject.Instantiate(linePrefab, canvas.transform);

        line.transform.localPosition = pos;

        var lineIdf = line.AddComponent<Circleldentifier>();

        lineIdf.id = id;

        lines.Add(lineIdf);

        return line;
    }


    void TrySetLineEdit(Circleldentifier circle)
    {
        foreach (var line in lines)
        {
            if (line.id == circle.id)
            {
                return;
            }
        }

        lineOnEdit = CreateLine(circle.transform.localPosition, circle.id);
        lineOnEditRcTs = lineOnEdit.GetComponent<RectTransform>();
        circleOnEdit = circle;
    }

    //�B�z�C���ܤưʵe
    void EnableColorFade(Animator anim)
    {
        anim.enabled = true;
        anim.Rebind();
    }

    //�إ߷ƹ��i�J��骺�ƥ�Ĳ�o
    public void OnMouseEnterCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);
        if (unlocking)
        {
            //��u��Ĳ���L����骺�ɭԡA�h���Ͳ��������I�N�ಾ��s����騭�W
            lineOnEditRcTs.sizeDelta = new Vector2(lineOnEditRcTs.sizeDelta.x, Vector3.Distance(circleOnEdit.transform.localPosition, idf.transform.localPosition));
            lineOnEditRcTs.rotation = Quaternion.FromToRotation(Vector3.up, (idf.transform.localPosition - circleOnEdit.transform.localPosition).normalized);

            TrySetLineEdit(idf);
        }
    }

    //�إ߷ƹ����}��骺�ƥ�Ĳ�o
    public void OnMouseExitCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);
    }

    //�إ߷ƹ��I�U��骺�ƥ�Ĳ�o
    public void OnMouseDownCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        //Debug.Log(idf.id);

        unlocking = true;
        �Ѧҹ�.gameObject.SetActive(false);

        TrySetLineEdit(idf);

    }

    //�إ߷ƹ���}��骺�ƥ�Ĳ�o
    public void OnMouseUpCircle(Circleldentifier idf)
    {
        if (enabled == false)
        {
            return;
        }

        ���G�}�C.Clear(); //�M�����

        //Debug.Log(idf.id);

        if (unlocking)
        {
            foreach (var line in lines)
            {
                EnableColorFade(circles[line.id].gameObject.GetComponent<Animator>());
                //Debug.Log(line.id);
                ���G�}�C.Add(line.id); //�K�[���
            }

            Destroy(lines[lines.Count - 1].gameObject);
            lines.RemoveAt(lines.Count - 1);

            foreach (var line in lines)
            {
                EnableColorFade(line.GetComponent<Animator>());

            }

            StartCoroutine(Release());
        }

        unlocking = false;
        �Ѧҹ�.gameObject.SetActive(true);

        //���ĩ�o��
        �ˬd����();
    }
    List<int> ���G�}�C = new List<int>();

    public List<int> �ؼа}�C = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    public bool ���ץ��T = false;
    [SerializeField] GameObject �E�c��;
    [SerializeField] GameObject �ާ@���O;
    public void �ˬd����()
    {
        //�ˬd�ƶq�O�_�۵�
        if (���G�}�C.Count != �ؼа}�C.Count)
        {
            Debug.Log("�ƶq����");



            return;
        }

        //�ΰj���ˬd�C�ӪF��O�_�۵�
        for (int i = 0; i < ���G�}�C.Count; i++)
        {
            if (���G�}�C[i] != �ؼа}�C[i])
            {
                Debug.Log("��" + i + "����F");

                return;
            }
        }

        if (���������D��.Count > 1)
        {
            Debug.Log("�����F�@�D�ٳѤU" + (���������D��.Count - 1) + "�D");
            �����F�@�D�ƥ�.Invoke();

            // �M�����ª��D��
            ���������D��.RemoveAt(0);
            // ���s���D
            �]�w�@�D�D��();
            ����();
            �h�ϧΧP�_��();
        }
        else
        {
            Debug.Log("�E�c�楿�T");
            ���A����();
            ����();
            �M���D��();
            ���������ƥ�.Invoke();
        }
    }
    public UnityEvent �����F�@�D�ƥ� = null;
    public UnityEvent ���������ƥ� = null;

    [SerializeField] Image �Ѧҹ�;
    [SerializeField] Image �Ѧҹϰ_�I;

    public void ���w�Ѧҹ�(Sprite ��J�Ѧҹ�)
    {
        �Ѧҹ�.sprite = ��J�Ѧҹ�;
    }
    public void ���w�Ѧҹϰ_�I(Sprite ��J�Ѧҹϰ_�I)
    {
        �Ѧҹϰ_�I.sprite = ��J�Ѧҹϰ_�I;
    }

    [SerializeField] CanvasGroup �E�c��UI;
    [SerializeField] SwitchCamera ��������;
    [SerializeField] GameObject �k�n��;
    [SerializeField] �E�c��u�X �E�c��u�X���{��;
    public bool �h�ϧ�ø�e���\ = false;
    public void ����()
    {
        �E�c��UI.alpha = 0;
        �E�c��UI.blocksRaycasts = false;
        �ާ@���O.SetActive(true);
        ��������.���w�����ĤT�H��();
        �k�n��.SetActive(true );
        �E�c��u�X���{��.�O�_���b����E�c�� = false;
    }

    public void ���A����()
    {
        if (���ץ��T == false)
        {
            ���ץ��T = true;
            Debug.Log("���L�Ȥw�g����");
        }

        if(�˼Ʈɶ� == false)
        {
            �˼Ʈɶ� = !�˼Ʈɶ�;
        }
    }

    public void �h�ϧΧP�_��()
    {
        if (�h�ϧ�ø�e���\ == false)
        {
            Debug.Log("�h�ϧΧP�_�q�L");
            �h�ϧ�ø�e���\ = true;
        }
        else
        {
            Debug.Log("�h�ϧ��ܦ^�h�F");
            �h�ϧ�ø�e���\ = false;
        }
    }

}

