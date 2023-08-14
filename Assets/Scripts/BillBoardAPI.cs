using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BillBoardAPI : MonoBehaviour
{
    public List<Image> Billboards = new List<Image>();
    public Image VerticalBillboards;

    private SetBillboards billboards;

    private void Start()
    {

        //billboards = GameObject.Find("BlockchainDataManager").GetComponent<SetBillboards>();
        //Debug.LogError("Getting billboars");
        setboards();

    }

    private async void setboards()
    {


/*        switch (transform.name)
        {
            case "Lava":
                if (!string.IsNullOrEmpty(billboards.LavaMapV))
                    await GetRemoteTexture(billboards.LavaMapV, VerticalBillboards);

                for (int i = 0; i < Billboards.Count; i++)
                {

                    if (billboards.LavaMap.Count <= i)
                        await GetRemoteTexture(billboards.LavaMap[i], Billboards[i]);

                }
                break;

            case "Futuristic":
                Debug.LogError("Futuristic billboars " + billboards.FuturisticMap.Count + " : " + Billboards.Count);

                if (!string.IsNullOrEmpty(billboards.FuturisticMapV))
                    await GetRemoteTexture(billboards.FuturisticMapV, VerticalBillboards);
                for (int i = 0; i < Billboards.Count; i++)
                {
                    if (billboards.LavaMap.Count <= i)
                        await GetRemoteTexture(billboards.FuturisticMap[i], Billboards[i]);
                }

                break;

            case "Shoefy":
                return;
                if (!string.IsNullOrEmpty(billboards.NeonMapV))
                    await GetRemoteTexture(billboards.NeonMapV, VerticalBillboards);

                for (int i = 0; i < Billboards.Count; i++)
                {

                    if (billboards.LavaMap.Count <= i)
                        await GetRemoteTexture(billboards.LavaMap[i], Billboards[i]);

                }
                break;
        }*/
    }

    public async Task GetRemoteTexture(string url, Image img)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();


            while (!asyncOp.isDone)
                await Task.Yield();


            if (www.isNetworkError || www.isHttpError)

            {

#if DEBUG
                Debug.LogError($"{www.error}, URL:{www.url}");
#endif


            }
            else
            {
                // return valid results:
                var tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                //  Debug.LogError("Texture is getting added");
                img.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            }
        }
    }
}


