using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    //プレイヤーの位置
    [SerializeField]
    Transform player;

	// プレイヤーとカメラの距離
    [SerializeField, Min(0)]
    float distance = 10;

    //距離補正
    [SerializeField, Min(0)]
    float distanceCorrection = 0.1f;

    // マウス感度
    [SerializeField] 
    float mouseSensitivity = 2.0f;

    //縦の角度
    float verticalRtt = 0.0f;
    //横の角度
    public float horizontalRtt = 0.0f;

    //縦の下限角度
    [SerializeField]
    float minVerticalRtt = -60.0f;
    //縦の上限角度
    [SerializeField]
    float maxVerticalRtt = 80.0f;

    void FixedUpdate()
	{
        CameraControl();

        CameraMove();
	}

    // カメラ操作
    void CameraControl()
    {
        //マウスが移動した距離を取得
        var mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouse *= mouseSensitivity;

        //横方向にカメラを動かす
        horizontalRtt += mouse.x;
        //0~360以内に変換
        horizontalRtt = Mathf.Repeat(horizontalRtt, 360.0f);

        verticalRtt -= mouse.y;
        verticalRtt = Mathf.Clamp(verticalRtt, minVerticalRtt, maxVerticalRtt);
    }

    //カメラを動かす。
    void CameraMove()
    {
        //プレイヤーの位置にカメラを移動
        transform.position = player.position;

        //(マウスの上下,マウスの左右,0)
        transform.localEulerAngles = new Vector3(verticalRtt, horizontalRtt, 0);

        //プレイヤーとカメラの位置関係を調整
        transform.Translate(Vector3.back * distance);

        //後ろに行き過ぎた分前に動かす。
        transform.Translate(Vector3.forward * (distance - RayDistance()));
    }

    /// <summary>
    /// プレイヤーと壁の距離を求める
    /// </summary>
    /// <returns>プレイヤーと壁の距離</returns>
    float RayDistance()
    {
        //ヒットしたらプレイヤーと当たった場所の距離を求める。
        if (Physics.Linecast(player.position, transform.position, out var hit))
            return Vector3.Distance(player.position, hit.point) - distanceCorrection;
        //なければ最大値を返す。
        else
            return distance;
    }
}
