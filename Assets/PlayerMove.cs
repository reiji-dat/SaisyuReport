using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    TPSCameraController tpsCamera;
    //当たり判定
    CharacterController cc;

    // 現在の落下速度
    float velY;

    // 移動速度
    [SerializeField]
    float moveSpeed = 5.0f;
    // 回転速度
    [SerializeField] 
    float charaRotationSpeed = 20.0f;
    //ジャンプ力
    [SerializeField]
    float jumpingForce = 5.0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    // 移動処理
    void Movement()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        //地面についていてジャンプしていればジャンプする
        if (cc.isGrounded && Input.GetButton("Jump")) velY = jumpingForce;
        //地面についてたらこれ以上落ちないようにする
        velY += Physics.gravity.y * Time.fixedDeltaTime;
        velY = cc.isGrounded && !Input.GetButton("Jump") ? 0.0f : velY;

        //速度は最後にまとめて処理する。
        if(input == Vector2.zero)
        {
            cc.Move(Vector3.up * velY * Time.fixedDeltaTime);
        }
        else
        {
            //カメラを基準に上下左右動くようにしたら良いかも？
            cc.Move(transform.forward * Time.fixedDeltaTime * moveSpeed + Vector3.up * velY * Time.fixedDeltaTime);
            Rotation(input);
        }
    }

    //※以下の処理はターゲットが右にあれば右回転、左にあれば左回転という処理に置き換えられそう。
    //回転処理
    void Rotation(Vector2 input)
    {
        var targetHorizontalRtt = tpsCamera.horizontalRtt + Angle(input);
        targetHorizontalRtt = Mathf.Repeat(targetHorizontalRtt, 360.0f);

        var playerHorizontalRtt = Mathf.Repeat(transform.localEulerAngles.y, 360.0f);

        //右側判定最大の値
        var rightMax = Mathf.Repeat(targetHorizontalRtt + 180, 360);

        //誤差修正
        if (Mathf.Abs(playerHorizontalRtt - targetHorizontalRtt) <= charaRotationSpeed)
            transform.localEulerAngles = Vector3.up * targetHorizontalRtt;
        else if (playerHorizontalRtt != targetHorizontalRtt)
        {
            //目標より右の最大が大きければ
            if (targetHorizontalRtt < rightMax)
            {
                //範囲が内側にあれば右向きに回転
                if (targetHorizontalRtt < playerHorizontalRtt && rightMax > playerHorizontalRtt)
                    transform.localEulerAngles = Vector3.up * (playerHorizontalRtt - charaRotationSpeed);
                else
                    transform.localEulerAngles = Vector3.up * (playerHorizontalRtt + charaRotationSpeed);
            }
            else
            {
                //目標より大きいか右の最大より小さければ右向きに回転
                if (targetHorizontalRtt < playerHorizontalRtt || rightMax > playerHorizontalRtt)
                    transform.localEulerAngles = Vector3.up * (playerHorizontalRtt - charaRotationSpeed);
                else
                    transform.localEulerAngles = Vector3.up * (playerHorizontalRtt + charaRotationSpeed);
            }
        }
    }

    //Mathfでは360度に対応していないため自分で作った。
    float Angle(Vector2 v2)
    {
        float rad = Mathf.Atan2(v2.x, v2.y);
        return rad * Mathf.Rad2Deg;
    }
}
