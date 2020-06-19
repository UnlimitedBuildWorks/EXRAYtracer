using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
//using Unity.Jobs.LowLevel.Unsafe;

public class EXRAYtracer : MonoBehaviour
{

    // Start is called before the first frame update
    private string dataPath;

    private static GameObject prefabBox;
    public struct OneBox
    {
        public Vector3 p1;
        public Vector3 p2;
        public float alpha;
        public OneBox(Vector3 pp1, Vector3 pp2, float palpha = 0.3f)
        {
            p1 = pp1; p2 = pp2; alpha = palpha;
        }
    }
    public static ConcurrentQueue<OneBox> queueBox;

    private static GameObject prefabLine;
    public struct OneLine
    {
        public Vector3 p1;
        public Vector3 p2;
        public OneLine(Vector3 pp1, Vector3 pp2)
        {
            p1 = pp1; p2 = pp2;
        }
    }
    public static ConcurrentQueue<OneLine> queueLine;

    private static Texture2D texture;
    public struct OnePixel
    {
        public int x, y;
        public Color clr;

        public OnePixel(int px, int py, Color pclr)
        {
            x = px; y = py; clr = pclr;
        }
    }
    public static ConcurrentQueue<OnePixel> queue;

    private const int ScreenX = 512;
    private const int ScreenY = 512;

    void Start()
    {
        queue = new ConcurrentQueue<OnePixel>();
        queueBox = new ConcurrentQueue<OneBox>();
        queueLine = new ConcurrentQueue<OneLine>();

        prefabBox = (GameObject)Resources.Load("BT_Cube");
        prefabLine = (GameObject)Resources.Load("BT_Line");

        texture = new Texture2D(ScreenX, ScreenY);
        GameObject obj = GameObject.Find("RawImage");
        obj.GetComponent<RawImage>().material.mainTexture = texture;

        dataPath = string.Copy(Application.dataPath);
        //SynchronizationContext currentContext = SynchronizationContext.Current;
        int argc = 2;
        string[] argv = {
            dataPath,
            dataPath + "/robot.ray"
        };

        Debug.Log("Start to execute EXRAY main routine.");

        Task<int> mainTask = Task<int>.Run(() => EXRAY.ray.main(ScreenX, ScreenY, argc, argv));

        //currentContext.Post(_ =>
        //{
        //    MakeBox(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.2f, 0.2f, 1.2f));
        //}, null);
    }

    public static void MakeBox(Vector3 p1, Vector3 p2)
    {
        GameObject box = Instantiate(prefabBox, Vector3.zero, Quaternion.identity);
        BT_Cube_Script s = box.GetComponent<BT_Cube_Script>();
        s.SetSize(p1, p2);
    }

    public static void MakeLine(Vector3 p1, Vector3 p2)
    {
        GameObject line = Instantiate(prefabLine, Vector3.zero, Quaternion.identity);
        BT_Line_Script s = line.GetComponent<BT_Line_Script>();
        s.SetPosition(p1, p2);
    }

    public static void SetPixel(int x, int y, Color c)
    {
        texture.SetPixel(x, y, c);
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        OneBox b;
        while (queueBox.TryDequeue(out b))
        {
            GameObject box = Instantiate(prefabBox, Vector3.zero, Quaternion.identity);
            BT_Cube_Script s = box.GetComponent<BT_Cube_Script>();
            s.SetSize(b.p1, b.p2);
            Material m = box.GetComponent<Renderer>().material;
            m.color = new Color(0.0f, 1.0f, 0.0f, b.alpha);
        }
        OneLine l;
        while (queueLine.TryDequeue(out l))
        {
            GameObject line = Instantiate(prefabLine, Vector3.zero, Quaternion.identity);
            BT_Line_Script s = line.GetComponent<BT_Line_Script>();
            s.SetPosition(l.p1, l.p2);
        }
        OnePixel p;
        bool drawFlag = false;
        while (queue.TryDequeue(out p))
        {
            texture.SetPixel(p.x, ScreenY - 1 - p.y, p.clr);
            drawFlag = true;
        }
        if (drawFlag) texture.Apply();
    }
}
