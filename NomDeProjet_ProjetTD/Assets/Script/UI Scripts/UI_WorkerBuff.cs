using UnityEngine;

public class UI_WorkerBuff : MonoBehaviour
{
    private Shooter _shooterScript;
    private Shooter_MultiTarget _shooterMultiScript;
    private Shooter_ThreeTargets _shooterThreeScript;
    // When is on then :
    private void ButtonClicked()
    {
        if (_shooterScript != null)
        {
            _shooterScript.BuffDamage();
        }
        else if (_shooterMultiScript != null)
        {
            _shooterMultiScript.BuffDamage();
        }

        else if (_shooterThreeScript != null)
        {
            _shooterThreeScript.BuffDamage();
        }
    }

    // when button is unclicked
    private void ButtonUnclicked()
    {
        if (_shooterScript != null)
        {
            _shooterScript.StopBuff();
        }
        else if (_shooterMultiScript != null)
        {
            _shooterMultiScript.StopBuff();
        }

        else if (_shooterThreeScript != null)
        {
            _shooterThreeScript.StopBuff();
        }
    }
}
