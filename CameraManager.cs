using System.Collections;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    //スクリプト連携
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera battleCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    //カメラ位置
    [SerializeField] private GameObject battleCameraPos;
    [SerializeField] private GameObject battleCameraLook;

    //戦闘開始時カメラ移動
    IEnumerator cameraMoveIn(Action onComplete)
    {
        battleCamera.Priority = 15;
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.BlendTime);
        onComplete.Invoke();
    }

    //戦闘終了時カメラ移動
    public IEnumerator cameraMoveOut(Character playerChara, Character enemyChara, Action onComplete)
    {
        battleCamera.Priority = 0;
        Vector3 flat = new Vector3(0, 0, 0);
        playerChara.transform.rotation = Quaternion.LookRotation(flat);
        enemyChara.transform.rotation = Quaternion.LookRotation(flat);
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.BlendTime);
        onComplete.Invoke();
    }

    //戦闘用カメラ位置セット
    public void setBattleCamera(Character playerChara, Character enemyChara, Action onComplete)
    {
        setCameraPosition(playerChara, enemyChara);
        StartCoroutine(cameraMoveIn(onComplete));
    }

    //戦闘用カメラ位置取得
    public void setCameraPosition(Character playerChara, Character enemyChara)
    {
        Vector3 center = Vector3.Lerp(playerChara.transform.position, enemyChara.transform.position, 0.5f);
        battleCameraLook.transform.position = center;
        float xPos = Math.Abs(playerChara.xPos - enemyChara.xPos) + 0.5f;
        float zPos = Math.Abs(playerChara.zPos - enemyChara.zPos) + 0.5f;

        if (playerChara.xPos < enemyChara.xPos)
        {
            if (playerChara.zPos == enemyChara.zPos)//正面から見る
            {
                center.z = center.z - xPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
            else if (playerChara.zPos < enemyChara.zPos)//右下から見る
            {
                center.x = playerChara.xPos + xPos;
                center.z = enemyChara.zPos - zPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
            else if (playerChara.zPos > enemyChara.zPos)//左下から見る
            {
                center.x = enemyChara.xPos - xPos;
                center.z = playerChara.zPos - zPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
        }
        else if (playerChara.xPos > enemyChara.xPos)
        {
            if (playerChara.zPos == enemyChara.zPos)//後ろから見る
            {
                center.z = center.z + xPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
            else if (playerChara.zPos < enemyChara.zPos)//右上から見る
            {
                center.x = enemyChara.xPos + xPos;
                center.z = playerChara.zPos + zPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
            else if (playerChara.zPos > enemyChara.zPos)//左上から見る
            {
                center.x = playerChara.xPos - xPos;
                center.z = enemyChara.zPos + zPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
        }
        else if (playerChara.xPos == enemyChara.xPos)
        {
            if (playerChara.zPos < enemyChara.zPos)//右から見る
            {
                center.x = enemyChara.xPos + zPos;
                center.z = enemyChara.zPos - xPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
            else if (playerChara.zPos > enemyChara.zPos)//左から見る
            {
                center.x = enemyChara.xPos - zPos;
                center.z = enemyChara.zPos + xPos;
                battleCameraPos.transform.position = center;
                Vector3 playerRelativeDir = playerChara.transform.position - battleCameraPos.transform.position;
                Vector3 enemyRelativeDir = enemyChara.transform.position - battleCameraPos.transform.position;
                playerChara.transform.rotation = Quaternion.LookRotation(playerRelativeDir);
                enemyChara.transform.rotation = Quaternion.LookRotation(enemyRelativeDir);
            }
        }

    }
}
